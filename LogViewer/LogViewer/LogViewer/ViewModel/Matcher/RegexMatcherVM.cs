using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogViewer.LogViewer.Matcher;

namespace LogViewer.LogViewer.ViewModel.Matcher
{
    public class RegexMatcherVM : ISpecializedMatcherVM
    {
        public string Pattern { get; set; } = "";

        public override bool hasAffixes()
        {
            return false;
        }

        public override bool hasSeparator()
        {
            return false;
        }

        protected override IMatcher<IComparable> createBackingMatcher()
        {
            return new InlineRegexMatcher();
        }

        protected override void updateBackingMatcher(IMatcher<IComparable> matcher, GeneralMatcherVM parent)
        {
            InlineRegexMatcher backingMatcher = (InlineRegexMatcher) matcher;
            backingMatcher.Pattern = Pattern;
        }
    }
}
