using System;

namespace Fluxor.Blazor.Web.DBreezePersistStore
{
    public class DBreezeStorageOptions
    {
        public string DbFolderPath { get; set; } = $@"{AppDomain.CurrentDomain.BaseDirectory}FluxorStateDB";
    }
}