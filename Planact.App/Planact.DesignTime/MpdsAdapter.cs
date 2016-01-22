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
        private PomodoroInventory inventory;

        private OnedriveManager onedriveManager = new OnedriveManager();

        public async Task<IEnumerable<Task>> ImportData(bool force = false, int withinMonthes = 3)
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

        private IEnumerable<Task> AdaptData(IEnumerable<PomodoroTask> filteredData)
        {
            throw new NotImplementedException();
        }

        private async Task<PomodoroInventory> LoadInventory()
        {
            // load data
            var xmlString = await LoadDataFromOnedrive();

            // deserialize data
            return DeserializeDataFromXmlString(xmlString);
        }

        private async Task<string> LoadDataFromOnedrive()
        {
            return await onedriveManager.DownloadContent("PomodoroInventory.dat");
        }

        private PomodoroInventory DeserializeDataFromXmlString(string xmlString)
        {
            return DataContractSerializerHelper.Deserialize<PomodoroInventory>(xmlString);
        }
    }
}
