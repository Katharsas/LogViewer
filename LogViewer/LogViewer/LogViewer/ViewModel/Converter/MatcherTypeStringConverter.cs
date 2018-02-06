using LogViewer.LogViewer.ViewModel.Matcher;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LogViewer.LogViewer.ViewModel.Converter
{
    public class MatcherTypeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MatcherType type = (MatcherType) value;
            return type.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string matcherString = (string) value;
            MatcherType type;
            bool success = Enum.TryParse(matcherString, false, out type);
            if (!success)
            {
                throw new InvalidOperationException("Invalid matcher type!");
            }
            return type;
        }
    }
}
