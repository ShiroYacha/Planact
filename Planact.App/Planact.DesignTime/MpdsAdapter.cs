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

        public async Task<IEnumerable<Task>> ImportData(bool force = false)
        {
            // load data
            if(force || inventory == null)
            {
                inventory = await LoadInventory();
            }

            // filter data
            var filteredData = FilterData(inventory.Pomodoros);

            // adapt data
            return AdaptData(filteredData);
        }

        #region Import data

        private async Task<PomodoroInventory> LoadInventory()
        {
            // load data
            var xmlString = await LoadDataFromPictureLibrary();

            // deserialize data
            return DeserializeDataFromXmlString(xmlString);
        }

        private async Task<string> LoadDataFromPictureLibrary()
        {
            var folder = KnownFolders.PicturesLibrary;
            var file = await folder.GetFileAsync("mdps.dat");
            return await FileIO.ReadTextAsync(file);
        }

        private PomodoroInventory DeserializeDataFromXmlString(string xmlString)
        {
            return DataContractSerializerHelper.Deserialize<PomodoroInventory>(xmlString);
        }

        #endregion

        #region Adapt data

        private IEnumerable<PomodoroTask> FilterData(IEnumerable<PomodoroTask> unfilteredData)
        {
            const int withinMonthes = 3;

            return unfilteredData.Where(p => 
                p.CompletedTimestamp > DateTime.Today.AddMonths(-withinMonthes) &&
                p.Deadline.HasValue
            );
        }

        private IEnumerable<Task> AdaptData(IEnumerable<PomodoroTask> filteredData)
        {
            return filteredData.Select(p=>AdaptData(p));
        }

        private Task AdaptData(PomodoroTask pomodoroTask)
        {
            // adapt task
            var task = new Task
            {
                Name = pomodoroTask.Title,
                Description = pomodoroTask.Description,
                Group = pomodoroTask.Group,
                Start = pomodoroTask.StartDate,
                End = pomodoroTask.Deadline.Value,
                Executions = AdaptExecution(pomodoroTask) 
            };

            // assign task reference
            foreach(var execution in task.Executions)
            {
                execution.Task = task;
            }

            return task;
        }

        private IEnumerable<Execution> AdaptExecution(PomodoroTask pomodoroTask)
        {
            var executions = new List<Execution>();

            // adapt finished pomodoros
            executions.AddRange(pomodoroTask.FinishedPomodoros.Select((f) => new Execution { Start = f - TimeSpan.FromMinutes(25), Duration = TimeSpan.FromMinutes(25) }));

            // adapt zen executions
            executions.AddRange(pomodoroTask.ExecutedMinutes.Select((e) => new Execution { Start = e.Item1-TimeSpan.FromMinutes(e.Item2), Duration = TimeSpan.FromMinutes(e.Item2) }));

            return executions;
        }

        #endregion

    }
}
