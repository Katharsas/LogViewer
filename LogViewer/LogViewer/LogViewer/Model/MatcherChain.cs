using System;
using System.Collections.Generic;
using System.ComponentModel;
using LogViewer.LogViewer.Matcher;
using LogViewer.LogViewer.util;
using WpfUtils;

namespace LogViewer.LogViewer.Model
{
    public class MatcherParseResult
    {
        // weather this line is a new LogAtom or belongs to the previous one
        public readonly bool isLineNewAtom;

        // if it is, weather this LogAtom was parsed successfully or some parsers failed
        public readonly bool isNewAtomParseSuccess;

        public readonly LogAtom newAtom;

        public MatcherParseResult(LogAtom atom)
        {
            isLineNewAtom = false;
            isNewAtomParseSuccess = false;
            newAtom = atom;
        }

        public MatcherParseResult(LogAtom atom, bool success)
        {
            isLineNewAtom = true;
            isNewAtomParseSuccess = success;
            newAtom = atom;
        }
    }


    public class MatcherChain : NotifyPropertyChanged
    {
        private int _matchesNeededForAtomDetected;

        public int MatchesNeededForAtomDetected
        {
            get { return _matchesNeededForAtomDetected; }
            private set
            {
                if (_matchesNeededForAtomDetected != value)
                {
                    _matchesNeededForAtomDetected = value;
                    OnPropertyChanged();
                }
            }
        }

        public MyBindingList<IMatcher<IComparable>> Matchers { get; }

        public IMatcher<string> RemainingLineMatcher { get; }

        private IMatcher<IComparable> _levelMatcherRef;
        public IMatcher<IComparable> LevelMatcherRef
        {
            get { return _levelMatcherRef; }
            set
            {
                if (_levelMatcherRef != value)
                {
                    _levelMatcherRef = value;
                    OnPropertyChanged();
                }
            }
        }

        public MatcherChain()
        {
            Matchers = new MyBindingList<IMatcher<IComparable>>();
            RemainingLineMatcher remainingLineMatcher = new RemainingLineMatcher();
            remainingLineMatcher.Name = "Message";
            RemainingLineMatcher = remainingLineMatcher;

            Matchers.ListChanged += onMatcherListChanged;
            onMatcherListChanged(null, null);
        }

        private void onMatcherListChanged(object sender, ListChangedEventArgs e)
        {
            MatchesNeededForAtomDetected = 2.ClampMax(Matchers.Count);
            OnPropertyChanged("Matchers");
        }

        public MatcherParseResult parseLine(int lineNumber, string line)
        {
            int currentIndex = 0;
            int currentMatcher = 0;
            List<IComparable> resultValues = new List<IComparable>();

            foreach (IMatcher<IComparable> matcher in Matchers)
            {
                // skip whitespace
                for (; currentIndex < line.Length; currentIndex++)
                {
                    if (!char.IsWhiteSpace(line[currentIndex]))
                    {
                        break;
                    }
                }

                IMatcherResult<IComparable> matcherResult = matcher.match(line, currentIndex);
                if (matcherResult == null)
                {
                    LogAtom failed = new LogAtom(line, lineNumber);
                    failed.MetaValues = resultValues;

                    // not very sophisticated empiric method to decide if a line is a new atom or belongs to previous
                    if (currentMatcher < MatchesNeededForAtomDetected)
                    {
                        // failed, not even an atom
                        failed.MetaValues.Clear();
                        return new MatcherParseResult(failed);
                    }
                    else
                    {
                        // failed, new atom
                        return new MatcherParseResult(failed, false);
                    }
                }
                else
                {
                    resultValues.Add(matcherResult.Value);
                    currentIndex = matcherResult.NextIndex;
                }
                currentMatcher++;
            }

            // new (successful) atom
            resultValues.Add(RemainingLineMatcher.match(line, currentIndex).Value);
            LogAtom success = new LogAtom(line, lineNumber);
            success.MetaValues = resultValues;
            return new MatcherParseResult(success, true);
        }

        public void reparseAtom(LogAtom atom)
        {
            
        }
    }
}
