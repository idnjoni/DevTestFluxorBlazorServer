using System;

namespace Fluxor.Blazor.Web.PersistStore
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class FluxorNoPersistAttribute : Attribute
    {
    }
}