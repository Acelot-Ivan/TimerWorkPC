using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TimerWorkPCFrame.ViewModel;
using YamlDotNet.Serialization;

namespace TimerWorkPCFrame
{
    public class MainWindowViewModel : BaseNotify
    {
        public ObservableCollection<WorkSessionViewModel> ListSessions { get; set; }
            = new ObservableCollection<WorkSessionViewModel>();

        private Timer _timerUpdate;
        private const string Path = "WorkSession.yaml";

        public WorkSessionViewModel SessionViewModel { get; set; } = new WorkSessionViewModel();

        public bool IsWindowMinimize { get; set; }

        //Что бы отключить сериализацию,
        //на время очистки списка и удаления файла.
        private bool _accessPath = true;
        private bool _activeSerialization;

        public WindowState WindowState { get; set; } = WindowState.Normal;
        public bool ShowTaskBar { get; set; } = true;

        public RelayCommand IsMinimizeCommand { get; set; }
        public RelayCommand ClearTimeListCommand { get; set; }

        public MainWindowViewModel()
        {
            IsMinimizeCommand = new RelayCommand(Minimized);

            ClearTimeListCommand = new RelayCommand(async () => { await Task.Run(ClearTimeList); });


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

            _timerUpdate = new Timer(TimeUpdateTick, 0, 0, 5000);
        }

        private void Minimized(object obj = null)
        {
            IsWindowMinimize = !IsWindowMinimize;
            WindowState = ShowTaskBar ? WindowState.Minimized : WindowState.Normal;
            ShowTaskBar = !ShowTaskBar;
        }

        private async Task ClearTimeList()
        {
            await Task.Run(() =>
            {
                _accessPath = false;

                _timerUpdate.Change(0, -1);

                if (Application.Current.Dispatcher != null)
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SessionViewModel = new WorkSessionViewModel();
                        ListSessions.Clear();
                        while (true)
                        {
                            try
                            {
                                File.Delete(Path);
                                break;
                            }
                            catch (Exception e)
                            {
                                Thread.Sleep(100);
                            }  
                        }
                    });

                _timerUpdate.Change(0, 5000);

                _accessPath = true;
            });
        }


        private void TimeUpdateTick(object obj)
        {
            if (ListSessions.Count == 0)
            {
                if (Application.Current.Dispatcher != null)
                    Application.Current.Dispatcher.Invoke(() => { ListSessions.Add(SessionViewModel); });
            }

            SessionViewModel.EndDateTime = DateTime.Now;

            if (_accessPath)
            {
                Serialization();
            }
        }

        #region Serilialization Part

        private void Serialization()
        {
            _activeSerialization = true;
            while (true)
            {
                try
                {
                    var serializer = new Serializer();
                    using var file = File.Open(Path, FileMode.Create);
                    using var writer = new StreamWriter(file);
                    serializer.Serialize(writer, GetWorkSessionSerializations());
                    break;
                }
                catch (Exception e)
                {
                    Thread.Sleep(100);
                }
            }
            _activeSerialization = false;
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