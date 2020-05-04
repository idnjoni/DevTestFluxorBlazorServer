using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fluxor.Blazor.Web.PersistStore.Abstractions;
using Fluxor.Blazor.Web.PersistStore.Actions;
using Fluxor.Blazor.Web.PersistStore.Interop;
using Fluxor.Blazor.Web.PersistStore.Interop.CallbackObjects;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Fluxor.Blazor.Web.PersistStore {
    /// <summary>
    /// Middleware for to persist state accross circuits
    /// </summary>
    internal sealed class PersistStoreMiddleware : WebMiddleware {
        private readonly IFluxorStorage storage;

        private readonly IOptions<PersistStoreMiddlewareOptions> options;

        private readonly FluxorPersistStoreInterop persistStoreInterop;

        private string storeKey;

        private IStore store;

        private bool dispatchLocked;

        /// <summary>
        /// Creates a new instance of the middleware
        /// </summary>
        public PersistStoreMiddleware (
            IFluxorStorage storage,
            IOptions<PersistStoreMiddlewareOptions> options,
            FluxorPersistStoreInterop persistStoreInterop)
        {
            this.storage = storage;
            this.options = options;
            this.persistStoreInterop = persistStoreInterop;
            this.persistStoreInterop.OnSessionKeepAlive = this.OnSessionKeepAlive;
        }

		/// <see cref="IMiddleware.GetClientScripts"/>
		public override string GetClientScripts() => FluxorPersistStoreInterop.GetClientScripts();

        /// <see cref="IMiddleware.InitializeAsync(IStore)"/>
        public async override Task InitializeAsync (IStore store) {
            this.store = store;
            this.storeKey = null;
            this.ScanIgnoredFeatures();

			await this.persistStoreInterop.InitializeAsync();
        }

        /// <see cref="IMiddleware.MayDispatchAction(object)"/>
        public override bool MayDispatchAction (object action)
        {
            if (!(action is PersistStoreActionBase))
            {
                return !this.dispatchLocked || !this.options.Value.IgnoreInitialDispatchesOnRestore;
            }

            return true;
        }

        /// <see cref="IMiddleware.BeforeDispatch(object)"/>
        public override void BeforeDispatch (object action)
        {
            if (action is PersistStoreSetKeyAction setKeyAction)
            {
                this.storeKey = setKeyAction.StoreKey;

                Dictionary<string, string> savedState = this.storage.LoadStateAsync(this.storeKey).Result;
                if (savedState != null)
                {
                   this.SetState(savedState);
                }
            }
            else if (action is PersistStoreLockAction lockAction)
            {
                this.dispatchLocked = lockAction.Locked;
            }
        }

        /// <see cref="IMiddleware.AfterDispatch(object)"/>
        public override void AfterDispatch (object action)
        {
            if (!(action is PersistStoreSetKeyAction))
            {
                if (this.storeKey != null)
                {
                    this.storage.SaveStateAsync(this.storeKey, this.GetState());
                }
            }
        }

        private void ScanIgnoredFeatures()
        {
            Type attributeType = typeof(FluxorNoPersistAttribute);
            foreach (IFeature feature in this.store.Features.Values)
            {
                FluxorNoPersistAttribute[] attributes = (FluxorNoPersistAttribute[])feature.GetType().GetCustomAttributes(attributeType, true);
                if (attributes.Count() > 0)
                {
                    this.options.Value.IgnoredFeatures.Add(feature.GetName());
                }
            }
        }

        private Dictionary<string, string> GetState () {
            var state = new Dictionary<string, string> ();
            foreach (IFeature feature in this.store.Features.Values)
            {
                if (this.options.Value.IgnoredFeatures.Contains(feature.GetName()))
                {
                    continue;
                }

                state[feature.GetName()] = JsonConvert.SerializeObject(feature.GetState());
            }

            return state;
        }

        private void SetState(IDictionary<string, string> newFeatureStates)
        {
            using (this.store.BeginInternalMiddlewareChange ()) {
                foreach (KeyValuePair<string, string> newFeatureState in newFeatureStates) {
                    // Get the feature with the given name
                    if (!this.store.Features.TryGetValue (newFeatureState.Key, out IFeature feature))
                    {
                        continue;
                    }

					object stronglyTypedFeatureState = JsonConvert.DeserializeObject(
						value: newFeatureState.Value,
						type: feature.GetStateType());

                    // Now set the feature's state to the deserialized object
                    feature.RestoreState (stronglyTypedFeatureState);
                }
            }
        }

  		private Task OnSessionKeepAlive(SessionKeepAliveCallback callbackInfo)
		{
            return this.storage.KeepAliveState(callbackInfo.payload.sessionKey);
		}
    }
}