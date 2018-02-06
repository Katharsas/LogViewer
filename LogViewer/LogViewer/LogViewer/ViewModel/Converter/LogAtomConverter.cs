using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using LogViewer.LogViewer.Matcher;
using LogViewer.LogViewer.Model;

namespace LogViewer.LogViewer.ViewModel.Converter
{
    public class MetaValueVM
    {
        private IComparable _value;

        public IComparable Value
        {
            get { return _value; }
            set
            {
                _value = value;
                TextValue = value.ToString();
            }
        }

        public string TextValue { get; private set; }
        public Brush Brush { get; set; } = Brushes.Black;
    }

    public class RowSettings
    {
        public bool Show { get; set; } = true;
        public Color Color { get; set; } = Colors.Black;
    }

    public class LogAtomConverter : IValueConverter
    {
        private readonly MatcherChain matchers;
        private readonly SettingsVM settingsVM;

        public LogAtomConverter()
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

            Console.WriteLine(levelMeatValueIndex);

            // Rows / Data
            foreach (LogAtom logAtom in atoms)
            {
                DataRow newRow = table.NewRow();

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
                    // cells
                    newRow[0] = new MetaValueVM()
                    {
                        Brush = new SolidColorBrush(settings.Color),
                        Value = logAtom.LineNumber
                    };

                    for (int c = 0; c < columns - 1; c++)
                    {
                        IComparable atomValue;
                        if (c >= logAtom.MetaValues.Count)
                        {
                            atomValue = "Parse Error!";
                        }
                        else
                        {
                            atomValue = logAtom.MetaValues[c];
                        }
                        newRow[c + 1] = new MetaValueVM()
                        {
                            Value = atomValue,
                            Brush = new SolidColorBrush(settings.Color)
                        };
                    }

                    table.Rows.Add(newRow);
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
