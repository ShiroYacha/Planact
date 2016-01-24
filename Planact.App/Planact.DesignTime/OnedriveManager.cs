using Microsoft.OneDrive.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Planact.DesignTime
{
    public class OnedriveManager
    {
        private IOneDriveClient client;
        private string text = "";

        public async Task<AccountSession> Initialize()
        {
            // onedrive
            try
            {
                var scopes = new string[] { "wl.basic", "wl.signin", "onedrive.readwrite" };
                client = OneDriveClientExtensions.GetUniversalClient(scopes);
                return await client.AuthenticateAsync();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<string> GetContent(string path)
        {
            var content = "";

            // try get file from local storage
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.TryGetItemAsync(path) as StorageFile;

            if (file == null)
            {
                // get from onedrive
                content = await DownloadContent(path);

                // write to local storage
                file = await storageFolder.CreateFileAsync(path, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, content);
            }
            else
            {
                // read from local storage
                content = await FileIO.ReadTextAsync(file);
            }

            // return result
            return content;
        }

        private async Task<string> DownloadContent(string path)
        {
            // initialize
            if (client == null)
            {
                await Initialize();
            }

            // download
            var content = await client
                             .Drive
                             .Root
                             .ItemWithPath(path)
                             .Content
                             .Request()
                             .GetAsync();
            var downloadContent = "";
            using (var streamReader = new StreamReader(content))
            {
                downloadContent = await streamReader.ReadToEndAsync();
            }
            return downloadContent;
        }
    }
}
