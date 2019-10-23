using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace TimerWorkPC
{
    public class MainWindowViewModel : BaseNotify
    {
        public List<WorkSession> ListSessions { get; set; }
            = new List<WorkSession>();


        private Timer _timeUpdate;
        private const string Path = "WorkSession.yaml";

        private WorkSession Session { get; set; } = new WorkSession();

        public MainWindowViewModel()
        {
            var deserialization = Deserialization();
            if (deserialization != null)
            {

                foreach (var session in deserialization)
                {
                    ListSessions.Add(new WorkSession
                    {
                        StartWorkDateTime = session.StartWorkDateTime,
                        EndDateTime = session.EndDateTime,
                    });
                }
            }
             
            ListSessions.Add(Session);

            _timeUpdate = new Timer(TimeUpdate, 0, 0, 1000);
        }

        private void TimeUpdate(object obj)
        {
            Session.EndDateTime = DateTime.Now;
            Serialization();
        }

        private void Serialization()
        {
            var serializer = new Serializer();
            using var file = File.Open(Path, FileMode.Create);
            using var writer = new StreamWriter(file);
            serializer.Serialize(writer, GetWorkSessionSerializations());
        }

        private WorkSessionSerialization[] Deserialization()
        {
            var deserializer = new Deserializer();

            using var file = File.Open(Path, FileMode.OpenOrCreate, FileAccess.Read);
            using var reader = new StreamReader(file);
            try
            {
                var result = deserializer.Deserialize<WorkSessionSerialization[]>(reader);
                return result;
            }
            catch(Exception exception)
            {
                return null;
            }
        }

        private WorkSessionSerialization[] GetWorkSessionSerializations()
        {
            return ListSessions.Select(session => new WorkSessionSerialization
            {
                StartWorkDateTime = session.StartWorkDateTime,
                EndDateTime = session.EndDateTime,
                WorkTime = session.WorkTime
            }).ToArray();
        }
    }

    public class WorkSession : BaseNotify
    {
        public string WorkTime => $"{WorkTimeSpan.Hours:00}" +
                                  $":" +
                                  $"{WorkTimeSpan.Minutes:00}" +
                                  $":" +
                                  $"{WorkTimeSpan.Seconds:00}";

        public DateTime StartWorkDateTime { get; set; } = DateTime.Now;
        public DateTime EndDateTime { get;  set; } = DateTime.Now;
        private TimeSpan WorkTimeSpan => EndDateTime - StartWorkDateTime;
    }

    public class WorkSessionSerialization
    {
        public DateTime StartWorkDateTime { get; set; } 
        public DateTime EndDateTime { get; set; }
        public string WorkTime { get; set; }
    }

    public class BaseNotify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}