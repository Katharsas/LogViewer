using System;
using LogViewer.LogViewer.Filter;
using WpfUtils;
using WpfUtils.lib;

namespace LogViewer.LogViewer.ViewModel
{
    public class FilterVM : NotifyPropertyChanged
    {
        private LineFilter backingFilter = null;
        private readonly LineFilterType type;
        private bool removed = false;

        private string _filterTypeDesc;
        private string _filterPattern = "";

        public string FilterTypeDesc
        {
            get { return _filterTypeDesc; }
            set { _filterTypeDesc = value; }
        }

        public string FilterPattern
        {
            get { return _filterPattern; }
            set
            {
                _filterPattern = value;
                OnPropertyChanged();
            }
        }

        public DelegateCommand RemoveFilter { get; set; }


        public FilterVM(LineFilterType type, Action<FilterVM> removeFromUI)
        {
            this.type = type;
            switch (type)
            {
                case LineFilterType.MatchesRegex:
                    _filterTypeDesc = "Match Regex: ";
                    break;
                case LineFilterType.StartsWith:
                    _filterTypeDesc = "Start with: ";
                    break;
            }

            RemoveFilter = new DelegateCommand(_ =>
            {
                removed = true;
                removeFromUI(this);
            });
        }

        public void propagateToBackingFilters(MyBindingList<LineFilter> filters)
        {
            if (backingFilter == null)
            {
                // if new filter was removed before being applied
                if (removed) return;

                // if filter is new
                switch (type)
                {
                    case LineFilterType.MatchesRegex:
                        backingFilter = new MatchesRegexFilter();
                        break;
                    case LineFilterType.StartsWith:
                        backingFilter = new StartsWithFilter();
                        break;
                }
                backingFilter.Pattern = FilterPattern;
                filters.Add(backingFilter);
            }
            else
            {
                if (removed)
                {
                    // if existing filter is removed
                    filters.Remove(backingFilter);
                }
                else
                {
                    // if existing filter was changed
                    backingFilter.Pattern = FilterPattern;
                }
            }
            
        }
    }
}
