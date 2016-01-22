using Microsoft.OneDrive.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<string> DownloadContent(string path)
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
