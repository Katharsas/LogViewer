using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogViewer.LogViewer.Filter;
using LogViewer.LogViewer.Matcher;
using LogViewer.LogViewer.Util;
using WpfUtils;
using WpfUtils.lib;
using LogViewer.LogViewer.Model;

namespace LogViewer.LogViewer.ViewModel.Matcher
{
    public enum MatcherType
    {
        DateTime,
        Number,
        Text,
        Regex
    }

    public class GeneralMatcherVM : NotifyPropertyChanged
    {
        public string Name { get; set; } = "";

        public string Prefix { get; set; } = "";
        public string Suffix { get; set; } = "";
        public bool HasAffixes { get; private set; }

        public string Separator { get; set; } = "";
        public bool HasSeparator { get; private set; }

        private ISpecializedMatcherVM _matcherVm;
        public ISpecializedMatcherVM MatcherVM
        {
            get { return _matcherVm; }
            set
            {
                if (_matcherVm != value)
                {
                    _matcherVm = value;
                    OnPropertyChanged();
                }
            }
        }

        public DelegateCommand RemoveMatcher { get; }

        public IEnumerable<MatcherType> AllMatcherTypes { get; }

        private MatcherType _selectedMatcherType;
        public MatcherType SelectedMatcherType
        {
            get { return _selectedMatcherType; }
            set
            {
                if (_selectedMatcherType != value)
                {
                    _selectedMatcherType = value;
                    OnPropertyChanged();
                    OnSelectedMatcherChanged(value);
                }
            }
        }

        private ISpecializedMatcherVM matcherVMRemoved;


        private Action<GeneralMatcherVM> cleanupSetLevel;

        public bool CanBeLevel { get; private set; }

        private bool _isLevel = false;
        public bool IsLevel
        {
            get { return _isLevel; }
            set
            {
                _isLevel = value;
                cleanupSetLevel(this);
            }
        }

        public GeneralMatcherVM(Action<GeneralMatcherVM> removeFromUI, Action<GeneralMatcherVM> cleanupSetLevel)
        {
            AllMatcherTypes = Enum.GetValues(typeof(MatcherType)).Cast<MatcherType>();
            SelectedMatcherType = MatcherType.Text;

            RemoveMatcher = new DelegateCommand(_ =>
            {
                MatcherVM.remove();
                removeFromUI(this);
            });

            this.cleanupSetLevel = cleanupSetLevel;
        }

        private void OnSelectedMatcherChanged(MatcherType newValue)
        {
            if (MatcherVM != null)
            {
                MatcherVM.remove();
                if (MatcherVM.hasBackingMatcher())
                {
                    matcherVMRemoved = MatcherVM;
                }
            }
            MatcherVM = getInstance(newValue);
            HasAffixes = MatcherVM.hasAffixes();
            HasSeparator = MatcherVM.hasSeparator();

            CanBeLevel = canBeLevel(newValue);
            if (!CanBeLevel)
            {
                IsLevel = false;
            }
        }

        public void propagateToBackingMatchers(MatcherChain matcherChain)
        {
            if (matcherVMRemoved != null)
            {
                matcherVMRemoved.propagateToBackingMatchers(matcherChain.Matchers, this);
                matcherVMRemoved = null;
            }
            MatcherVM.propagateToBackingMatchers(matcherChain.Matchers, this);
            if (MatcherVM.hasBackingMatcher())
            {
                MatcherVM.getBackingMatcher().Name = Name;
            }

            if (IsLevel && MatcherVM.hasBackingMatcher())
            {
                matcherChain.LevelMatcherRef = MatcherVM.getBackingMatcher();
            }
        }

        private static ISpecializedMatcherVM getInstance(MatcherType type)
        {
            switch (type)
            {
                case MatcherType.DateTime:
                    return new DateTimeFormatMatcherVM();
                case MatcherType.Number:
                    return new NumberMatcherVM();
                case MatcherType.Text:
                    return new TextMatcherVM();
                case MatcherType.Regex:
                    return new RegexMatcherVM();
                default:
                    throw new System.InvalidOperationException("Invalid MatcherType!");
            }
        }

        private static bool canBeLevel(MatcherType type)
        {
            if (type == MatcherType.DateTime || type == MatcherType.Number)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
