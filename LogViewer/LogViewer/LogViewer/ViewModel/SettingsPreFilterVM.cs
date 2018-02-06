using LogViewer.LogViewer.Filter;
using System;
using WpfUtils;
using WpfUtils.lib;

namespace LogViewer.LogViewer.ViewModel
{
    public enum LineFilterType
    {
        StartsWith, MatchesRegex
    }

    public class SettingsPreFilterVM : NotifyPropertyChanged
    {
        private readonly MyBindingList<LineFilter> filters;

        public DelegateCommand AddStartsWithFilter { get; }
        public DelegateCommand AddMatchesRegexFilter { get; }

        public MyBindingList<FilterVM> FilterVMs { get; }
        private MyBindingList<FilterVM> FilterVMsRemoved { get; }

        public bool IsListVisible { get; private set; } = false;

        public SettingsPreFilterVM(MyBindingList<LineFilter> filters)
        {
            this.filters = filters;

            FilterVMs = new MyBindingList<FilterVM>();
            FilterVMsRemoved = new MyBindingList<FilterVM>();

            FilterVMs.ListChanged += (sender, args) =>
            {
                IsListVisible = FilterVMs.Count > 0;
            };

            Action<FilterVM> removeFromUI = filter =>
            {
                FilterVMs.Remove(filter);
                FilterVMsRemoved.Add(filter);
            };

            AddStartsWithFilter = new DelegateCommand(_ =>
            {
                FilterVM filter = new FilterVM(LineFilterType.StartsWith, removeFromUI);
                FilterVMs.Add(filter);
            });
            AddMatchesRegexFilter = new DelegateCommand(_ =>
            {
                FilterVM filter = new FilterVM(LineFilterType.MatchesRegex, removeFromUI);
                FilterVMs.Add(filter);
            });
        }

        public void apply()
        {
            foreach (FilterVM filter in FilterVMs)
            {
                filter.propagateToBackingFilters(filters);
            }
            foreach (FilterVM filter in FilterVMsRemoved)
            {
                filter.propagateToBackingFilters(filters);
            }
            FilterVMsRemoved.Clear();
        }
    }
}
