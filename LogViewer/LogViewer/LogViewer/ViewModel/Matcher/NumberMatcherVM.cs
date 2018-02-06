using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogViewer.LogViewer.Filter;
using LogViewer.LogViewer.Matcher;
using WpfUtils;

namespace LogViewer.LogViewer.ViewModel.Matcher
{
    public class NumberMatcherVM : ISpecializedMatcherVM
    {
        public string DecimalSeparator { get; set; } = NumberMatcher.defaultDecimalSeparator.ToString();
        public string GroupSeparator { get; set; } = NumberMatcher.defaultGroupSeparator.ToString();

        public override bool hasAffixes()
        {
            return true;
        }

        public override bool hasSeparator()
        {
            return false;
        }

        protected override IMatcher<IComparable> createBackingMatcher()
        {
            return new NumberMatcher();
        }

        protected override void updateBackingMatcher(IMatcher<IComparable> matcher, GeneralMatcherVM parent)
        {
            NumberMatcher backingMatcher = (NumberMatcher) matcher;
            backingMatcher.ignoredPrefix = parent.Prefix;
            backingMatcher.ignoredSuffix = parent.Suffix;

            if (DecimalSeparator != "")
            {
                backingMatcher.decimalSeparator = DecimalSeparator[0];
            }
            else
            {
                backingMatcher.decimalSeparator = null;
            }

            if (GroupSeparator != "")
            {
                backingMatcher.groupSeparator = GroupSeparator[0];
            }
            else
            {
                backingMatcher.groupSeparator = null;
            }
        }
    }
}
