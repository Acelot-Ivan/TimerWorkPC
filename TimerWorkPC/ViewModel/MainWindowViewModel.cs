using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using YamlDotNet.Serialization;

namespace TimerWorkPC
{
    public class MainWindowViewModel : BaseNotify
    {
        public List<WorkSessionViewModel> ListSessions { get; set; }
            = new List<WorkSessionViewModel>();


        private Timer _timerUpdate;
        private const string Path = "WorkSession.yaml";

        private WorkSessionViewModel SessionViewModel { get; set; } = new WorkSessionViewModel();

        public MainWindowViewModel()
        {
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

        private void TimeUpdateTick(object obj)
        {
            SessionViewModel.EndDateTime = DateTime.Now;
            Serialization();
        }

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
            catch(Exception)
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
    }

}