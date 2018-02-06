using System.Text.RegularExpressions;

namespace LogViewer.LogViewer.Filter
{
    /// <summary>
    /// A filter that interprets the configured text pattern as regex pattern and tries to match that
    /// pattern onto a log line. Also see <see cref="LineFilter"/>.
    /// </summary>
    class MatchesRegexFilter : LineFilter
    {
        private string _pattern = "";

        public override string Pattern
        {
            get { return _pattern; }
            set
            {
                if (_pattern != value)
                {
                    _pattern = value;
                    compiledRegex = value != "" ? new Regex(value) : null;
                    OnPropertyChanged();
                }
            }
        }

        private Regex compiledRegex;

        public override bool filter(string line)
        {
            if (compiledRegex == null)
            {
                return true;
            }
            else
            {
                Match match = compiledRegex.Match(line);
                return !match.Success;
            }
        }
    }
}
