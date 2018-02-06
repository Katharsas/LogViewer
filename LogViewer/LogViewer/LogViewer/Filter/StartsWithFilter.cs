namespace LogViewer.LogViewer.Filter
{
    /// <summary>
    /// A filter that checks if log lines start with the configured text pattern. Also see <see cref="LineFilter"/>.
    /// </summary>
    class StartsWithFilter : LineFilter
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
                    OnPropertyChanged();
                }
            }
        }

        public override bool filter(string line)
        {
            if (Pattern == "")
            {
                return true;
            }
            else
            {
                return !line.StartsWith(Pattern);
            }
        }
    }
}
