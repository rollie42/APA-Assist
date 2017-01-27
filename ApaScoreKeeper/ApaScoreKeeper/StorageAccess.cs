using Newtonsoft.Json;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApaScoreKeeper
{
    public class StorageAccess : IRecursiveJsonReader, IRecursiveJsonWriter
    {
        private const string _folderName = "APA Assist";
        private RecursiveJsonConverter Converter { get; set; }

        public StorageAccess()
        {
            Converter = new RecursiveJsonConverter(this, this);
        }

        public async Task<IUniqueIdentifier> Read(string key, Type type)
        {
            var appFolder = await ApplicationFolder();
            var storageFile = await appFolder.CreateFileAsync(key, CreationCollisionOption.OpenIfExists);
            var serialized = await storageFile.ReadAllTextAsync();
            if (string.IsNullOrEmpty(serialized))
            {
                return (IUniqueIdentifier)Activator.CreateInstance(type);
            }

            return (IUniqueIdentifier)JsonConvert.DeserializeObject(serialized, type, Converter);
        }

        public async Task Write(string key, IUniqueIdentifier obj)
        {
            var appFolder = await ApplicationFolder();
            var storageFile = await appFolder.CreateFileAsync(key, CreationCollisionOption.OpenIfExists);            
            var serialized = await Task.Run(() => JsonConvert.SerializeObject(obj, Converter));

            await storageFile.WriteAllTextAsync(serialized);
        }

        private async Task<IFolder> ApplicationFolder()
        {
            return await FileSystem.Current.LocalStorage.CreateFolderAsync(_folderName, CreationCollisionOption.OpenIfExists);
        }
    }
}
