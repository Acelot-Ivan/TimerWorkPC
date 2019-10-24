using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using TimerWorkPCFrame.ViewModel;
using YamlDotNet.Serialization;

namespace TimerWorkPCFrame
{
    public class MainWindowViewModel : BaseNotify
    {
        public ObservableCollection<WorkSessionViewModel> ListSessions { get; set; }
            = new ObservableCollection<WorkSessionViewModel>();
        public Dictionary<DayOfWeek, string> DayOfWeekList = new Dictionary<DayOfWeek, string>
        {
            {DayOfWeek.Monday, "Понедельник" },
            {DayOfWeek.Tuesday, "Вторник" },
            {DayOfWeek.Wednesday, "Среда" },
            {DayOfWeek.Thursday, "Четверг" },
            {DayOfWeek.Friday, "Пятница" },
            {DayOfWeek.Saturday, "Суббота" },
            {DayOfWeek.Sunday, "Воскресенье" },
        };


        private Timer _timerUpdate;
        private const string Path = "WorkSession.yaml";

        public WorkSessionViewModel SessionViewModel { get; set; } = new WorkSessionViewModel();
        public bool IsWindowMinimize { get; set; }
        public bool ShowTaskBar { get; set; } = true;

        public RelayCommand IsMinimizeCommand { get; set; }
        public RelayCommand ClearTimeListCommand { get; set; }

        public MainWindowViewModel()
        {
            IsMinimizeCommand = new RelayCommand(Minimized);
            ClearTimeListCommand = new RelayCommand(ClearTimeList);

            var deserialization = Deserialization();
            if (deserialization != null)
            {
                foreach (var session in deserialization)
                {
                    ListSessions.Add(new WorkSessionViewModel
                    {
                        StartWorkDateTime = session.StartWorkDateTime,
                        EndDateTime = session.EndDateTime
                    });
                }
            }

            ListSessions.Add(SessionViewModel);

            _timerUpdate = new Timer(TimeUpdateTick, 0, 0, 1000);
        }


        private void Minimized(object obj)
        {
            IsWindowMinimize = !IsWindowMinimize;
            ShowTaskBar = !ShowTaskBar;
        }

        private void ClearTimeList(object obj)
        {
            ListSessions.Clear();
        }


        private void TimeUpdateTick(object obj)
        {
            SessionViewModel.EndDateTime = DateTime.Now;
           //Serialization();
        }

        #region Serilialization Part

        private void Serialization()
        {
            var serializer = new Serializer();
            using var file = File.Open(Path, FileMode.Create);
            using var writer = new StreamWriter(file);
            serializer.Serialize(writer, GetWorkSessionSerializations());
        }

        private WorkSessionModel[] Deserialization()
        {
            var deserializer = new Deserializer();

            using var file = File.Open(Path, FileMode.OpenOrCreate, FileAccess.Read);
            using var reader = new StreamReader(file);
            try
            {
                var result = deserializer.Deserialize<WorkSessionModel[]>(reader);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private WorkSessionModel[] GetWorkSessionSerializations()
        {
            return ListSessions.Select(session => new WorkSessionModel
            {
                StartWorkDateTime = session.StartWorkDateTime,
                EndDateTime = session.EndDateTime,
                WorkTime = session.WorkTime
            }).ToArray();
        }

        #endregion
    }
}