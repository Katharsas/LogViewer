using LogViewer.LogViewer.Matcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer.LogViewer.ViewModel.Matcher
{
    public class TextMatcherVM : ISpecializedMatcherVM
    {
        public string Separator { get; set; } = "";

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
            return new TextConfigurableMatcher();
        }

        protected override void updateBackingMatcher(IMatcher<IComparable> matcher, GeneralMatcherVM parent)
        {
            TextConfigurableMatcher backingMatcher = (TextConfigurableMatcher) matcher;
            backingMatcher.ignoredPrefix = parent.Prefix;
            backingMatcher.ignoredSuffix = parent.Suffix;
            backingMatcher.Separator = parent.Separator;
        }
    }
}
