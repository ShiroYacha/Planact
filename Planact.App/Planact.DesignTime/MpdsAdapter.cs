using MPDS.Core.DataService.Serialization;
using MPDS.Core.PomodoroSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Planact.DesignTime
{
    public class MpdsAdapter
    {
        private static PomodoroInventory inventory;

        public static async Task<IEnumerable<Task>> ImportData(bool force = false, int withinMonthes = 3)
        {
            // load data
            if(force || inventory == null)
            {
                inventory = await LoadInventory();
            }

            // filter data
            var filteredData = inventory.Pomodoros.Where(p => p.CompletedTimestamp > DateTime.Today.AddMonths(-withinMonthes));

            // adapt data
            return AdaptData(filteredData);
        }

        private static IEnumerable<Task> AdaptData(IEnumerable<PomodoroTask> filteredData)
        {
            throw new NotImplementedException();
        }

        private static async Task<PomodoroInventory> LoadInventory()
        {
            var xmlString = await LoadDataFromSharedFolder();
            return DeserializeDataFromXmlString(xmlString);
        }

        private static async Task<string> LoadDataFromSharedFolder()
        {
            // read from publisher shared folder
            StorageFolder localFolder = ApplicationData.Current.LocalFolder; // change to shared folder
            IStorageFile file = await localFolder.GetFileAsync("PomodoroInventory.dat");
            return await FileIO.ReadTextAsync(file);
        }

        private static PomodoroInventory DeserializeDataFromXmlString(string xmlString)
        {
            return DataContractSerializerHelper.Deserialize<PomodoroInventory>(xmlString);
        }
    }
}
