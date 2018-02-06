using System.Text.RegularExpressions;

namespace LogViewer.LogViewer.Matcher
{
    /// <summary>
    /// Applies a given regex pattern onto the input (starting at given startIndex) which must match
    /// sucessfully. Returns as value the the first capture group if one was specified in the pattern,
    /// or the entire remaining string if not.
    /// </summary>
    class InlineRegexMatcher : AbstractMatcher<string>
    {
        public string _pattern;

        public string Pattern {
            get { return _pattern; }
            set
            {
                this._pattern = value;
                OnPropertyChanged();
            }
        }

        public override IMatcherResult<string> match(string statement, int startIndex)
        {
            string regexPrefix = @"^.{" + startIndex + "}";
            Regex regex = new Regex(regexPrefix + Pattern);
            Match match = regex.Match(statement);
            if (!match.Success)
            {
                return null;
            }
            GroupCollection groups = match.Groups;
            Group entireMatch = groups[0]; // this group always exists
            int matchEnd = entireMatch.Length;

            if (groups.Count == 1)
            {
                // group at index 0 is entire match, use this if no capturing groups were specified
               
                int valueLength = matchEnd - startIndex;
                string value = statement.Substring(startIndex, valueLength);
                return new MatcherResult<string>(matchEnd, value);
            }
            else
            {
                // if groups: return first groups value
                // TODO Idea: if multiple groups exist, return their values concatinated as result

                Group firstCaptureGroup = groups[1];
                string value = firstCaptureGroup.Value;
                return new MatcherResult<string>(matchEnd, value);
            }
        }
    }
}
