using LogViewer.LogViewer.Matcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUtils;

namespace LogViewer.LogViewer.ViewModel.Matcher
{
    public abstract class ISpecializedMatcherVM
    {
        public abstract bool hasAffixes();
        public abstract bool hasSeparator();

        protected abstract IMatcher<IComparable> createBackingMatcher();
        protected abstract void updateBackingMatcher(IMatcher<IComparable> matcher, GeneralMatcherVM parent);

        private IMatcher<IComparable> backingMatcher;
        private bool removed = false;

        public void remove()
        {
            removed = true;
        }

        public Boolean hasBackingMatcher()
        {
            return backingMatcher != null;
        }

        public IMatcher<IComparable> getBackingMatcher()
        {
            return backingMatcher;
        }

        public void propagateToBackingMatchers(MyBindingList<IMatcher<IComparable>> matchers, GeneralMatcherVM parent)
        {
            Console.WriteLine("At matcher");
            if (backingMatcher == null)
            {
                // if new matcher was removed before being applied
                if (removed) return;

                // if matcher is new
                backingMatcher = createBackingMatcher();
                updateBackingMatcher(backingMatcher, parent);
                matchers.Add(backingMatcher);
            }
            else
            {
                if (removed)
                {
                    // if existing matcher is removed
                    matchers.Remove(backingMatcher);
                }
                else
                {
                    // if existing matcher was changed
                    updateBackingMatcher(backingMatcher, parent);
                }
            }
        }
    }
}
