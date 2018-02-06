using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogViewer.LogViewer.Model;
using Microsoft.Practices.ServiceLocation;
using System.Windows.Data;
using LogViewer.LogViewer.Matcher;

namespace LogViewer.LogViewer.ViewModel.Converter
{
    public class LogAtomDetailsConverter: IValueConverter
    {
        private readonly MatcherChain matchers;
        private readonly SettingsVM settingsVM;

        public LogAtomDetailsConverter() 
        {
            LogView logView = ServiceLocator.Current.GetInstance<LogView>();
            matchers = logView.Matchers;

            MainViewModel vm = ServiceLocator.Current.GetInstance<MainViewModel>();
            settingsVM = vm.SettingsVM;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var atoms = value as IEnumerable<LogAtom>;
            if (atoms == null) return null;

            // static columns: line, message
            int columns = matchers.Matchers.Count + 2;

            DataTable table = new DataTable();
            Type cellType = typeof(MetaValueVM);

            ObservableCollection<string> result = new ObservableCollection<string>();

            // Columns / Headers
            table.Columns.Add(new DataColumn("Line", cellType));

            int levelMeatValueIndex = -1;
            foreach (IMatcher<IComparable> matcher in matchers.Matchers)
            {
                if (matcher == matchers.LevelMatcherRef)
                {
                    levelMeatValueIndex = table.Columns.Count - 1;
                }
                table.Columns.Add(new DataColumn(matcher.Name, cellType));
            }
            table.Columns.Add(new DataColumn(matchers.RemainingLineMatcher.Name, cellType));

            // Rows / Data
            foreach (LogAtom logAtom in atoms)
            {
                // level settings
                RowSettings settings;
                if (levelMeatValueIndex != -1)
                {
                    IComparable levelValue = logAtom.MetaValues[levelMeatValueIndex];
                    settings = getRowSettings(levelValue);
                }
                else
                {
                    settings = new RowSettings();
                }

                if (settings.Show)
                {
                    string details;
                    if (logAtom.RawAdditionalLines != null)
                    {
                        details = String.Join("\n", logAtom.RawAdditionalLines);
                    }
                    else
                    {
                        details = "";
                    }
                    result.Add(details);
                }
            }
            return table.DefaultView;
        }

        private RowSettings getRowSettings(IComparable metaValue)
        {
            foreach (LevelVM vm in settingsVM.SettingsLevelVM.Levels)
            {
                string metaValueString = metaValue.ToString();
                Console.WriteLine(metaValueString + " vs " + vm.Name);
                if (metaValueString.Equals(vm.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return new RowSettings()
                    {
                        Show = vm.Show,
                        Color = vm.Color
                    };
                }
            }
            return new RowSettings();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
