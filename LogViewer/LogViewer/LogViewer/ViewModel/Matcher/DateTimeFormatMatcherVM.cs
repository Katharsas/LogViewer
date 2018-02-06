using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogViewer.LogViewer.Matcher;

namespace LogViewer.LogViewer.ViewModel.Matcher
{
    public class DateTimeFormatMatcherVM : ISpecializedMatcherVM
    {
        public string DateTimeFormat { get; set; }

        public override bool hasAffixes()
        {
            return true;
        }

        public override bool hasSeparator()
        {
            return true;
        }

        protected override IMatcher<IComparable> createBackingMatcher()
        {
            return new DateTimeFormatMatcher();
        }

        protected override void updateBackingMatcher(IMatcher<IComparable> matcher, GeneralMatcherVM parent)
        {
            DateTimeFormatMatcher backingMatcher = (DateTimeFormatMatcher) matcher;
            backingMatcher.Format = DateTimeFormat;
            backingMatcher.ignoredPrefix = parent.Prefix;
            backingMatcher.ignoredSuffix = parent.Suffix;
            backingMatcher.Separator = parent.Separator;
        }
    }
}
