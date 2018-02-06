using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfUtils;
using WpfUtils.lib;

namespace LogViewer.LogViewer.ViewModel
{
    public class SettingsLevelVM
    {
        public MyBindingList<LevelVM> Levels { get; }

        public DelegateCommand AddLevel { get; }

        public SettingsLevelVM()
        {
            Levels = new MyBindingList<LevelVM>();

            Action<LevelVM> onRemove = levelVM =>
            {
                Levels.Remove(levelVM);
            };

            AddLevel = new DelegateCommand(_ =>
            {
                Levels.Add(new LevelVM(onRemove)
                {
                    Color = Colors.Black,
                    Name = "",
                    Show = true
                });
            });

            addDefaultLevels(onRemove);
        }

        private void addDefaultLevels(Action<LevelVM> onRemove)
        {
            Levels.Add(new LevelVM(onRemove)
            {
                Color = Colors.Black,
                Name = "DEBUG",
                Show = true
            });
            Levels.Add(new LevelVM(onRemove)
            {
                Color = Colors.Black,
                Name = "INFO",
                Show = true
            });
            Levels.Add(new LevelVM(onRemove)
            {
                Color = Colors.Black,
                Name = "WARN",
                Show = true
            });
            Levels.Add(new LevelVM(onRemove)
            {
                Color = Colors.DarkRed,
                Name = "ERROR",
                Show = true
            });
        }
    }
}
