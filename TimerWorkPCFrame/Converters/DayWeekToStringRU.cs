using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TimerWorkPCFrame.Converters
{
    public class DayWeekToStringRu : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Чегось?";

            var dateTime = (DateTime) value;
            var dayOfWeek = dateTime.DayOfWeek;
            return _dayOfWeekList[dayOfWeek];

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        private readonly Dictionary<DayOfWeek, string> _dayOfWeekList = new Dictionary<DayOfWeek, string>
        {
            {DayOfWeek.Monday, "Понедельник"},
            {DayOfWeek.Tuesday, "Вторник"},
            {DayOfWeek.Wednesday, "Среда"},
            {DayOfWeek.Thursday, "Четверг"},
            {DayOfWeek.Friday, "Пятница"},
            {DayOfWeek.Saturday, "Суббота"},
            {DayOfWeek.Sunday, "Воскресенье"},
        };
    }
}