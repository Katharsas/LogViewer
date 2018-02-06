using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogViewer.LogViewer.Model;
using LogViewer.LogViewer.ViewModel.Matcher;
using WpfUtils;
using WpfUtils.lib;

namespace LogViewer.LogViewer.ViewModel
{
    public class SettingsMatcherVM
    {
        private MatcherChain matcherChain;

        public DelegateCommand AddMatcher { get; }

        public MyBindingList<GeneralMatcherVM> MatcherVMs { get; }
        public MyBindingList<GeneralMatcherVM> MatcherVMsRemoved { get; }

        public SettingsMatcherVM(MatcherChain matcherChain)
        {
            this.matcherChain = matcherChain;

            MatcherVMs = new MyBindingList<GeneralMatcherVM>();
            MatcherVMsRemoved = new MyBindingList<GeneralMatcherVM>();

            Action<GeneralMatcherVM> removeFromUI = matcherVM =>
            {
                MatcherVMs.Remove(matcherVM);
                MatcherVMsRemoved.Add(matcherVM);
                matcherVM.IsLevel = false;
            };

            AddMatcher = new DelegateCommand(_ =>
            {
                GeneralMatcherVM matcher = new GeneralMatcherVM(removeFromUI, cleanUpSetLevel);
                matcher.Name = "Matcher #" + (MatcherVMs.Count + 1);
                MatcherVMs.Add(matcher);
            });
        }

        public void cleanUpSetLevel(GeneralMatcherVM newLevel)
        {
            foreach (GeneralMatcherVM matcherVM in MatcherVMs)
            {
                if (matcherVM != newLevel && matcherVM.IsLevel)
                {
                    matcherVM.IsLevel = false;
                }
            }
        }

        public void apply()
        {
            matcherChain.LevelMatcherRef = null;
            foreach (GeneralMatcherVM matcher in MatcherVMs)
            {
                Console.WriteLine("Propagating Matcher");
                matcher.propagateToBackingMatchers(matcherChain);
            }
            foreach (GeneralMatcherVM matcher in MatcherVMsRemoved)
            {
                matcher.propagateToBackingMatchers(matcherChain);
            }
            MatcherVMsRemoved.Clear();
        }
    }
}
