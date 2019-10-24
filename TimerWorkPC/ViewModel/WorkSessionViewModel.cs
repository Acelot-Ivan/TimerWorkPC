using System;

namespace TimerWorkPC
{
    public class WorkSessionViewModel : BaseNotify
    {
        public string WorkTime => $"{WorkTimeSpan.Hours:00}" +
                                  ":" +
                                  $"{WorkTimeSpan.Minutes:00}" +
                                  ":" +
                                  $"{WorkTimeSpan.Seconds:00}";

        public DateTime StartWorkDateTime { get; set; } = DateTime.Now;
        public DateTime EndDateTime { get; set; } = DateTime.Now;
        private TimeSpan WorkTimeSpan => EndDateTime - StartWorkDateTime;
    }
}
