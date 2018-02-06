using LogViewer.LogViewer.Model;
using WpfUtils;
using WpfUtils.lib;

namespace LogViewer.LogViewer.ViewModel
{
    public class SettingsVM : NotifyPropertyChanged
    {
        private LogView logView;

        public SettingsPreFilterVM SettingsPreFilterVM { get; }
        public SettingsMatcherVM SettingsMatcherVM { get; }
        public SettingsLevelVM SettingsLevelVM { get; }

        public DelegateCommand ApplySettings { get; }

        public SettingsVM(LogView logView)
        {
            this.logView = logView;

            SettingsPreFilterVM = new SettingsPreFilterVM(logView.Filters);
            SettingsMatcherVM = new SettingsMatcherVM(logView.Matchers);
            SettingsLevelVM = new SettingsLevelVM();

            ApplySettings = new DelegateCommand(_ =>
            {
                SettingsPreFilterVM.apply();
                SettingsMatcherVM.apply();
                logView.reparseDirtyFiltersAndMatchers();
            });
        }
    }
}
