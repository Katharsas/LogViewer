using System.Collections.Generic;
using WpfUtils;

namespace LogViewer.LogViewer.Filter
{
    /// <summary>
    /// Filter used for prefiltering log lines before atempting to parse lines.
    /// </summary>
    public abstract class LineFilter : NotifyPropertyChanged
    {
        /// <summary>
        /// Text pattern used to configure the filter.
        /// </summary>
        public abstract string Pattern { get; set; }

        /// <summary>
        /// Check if filter matches given line, based on filter implementation.
        /// The empty string Pattern is a special case and will cause filters to not match.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>False if the line should be ignored, True otherwise.</returns>
        public abstract bool filter(string line);


        public static bool filter(string line, IList<LineFilter> filters)
        {
            foreach (LineFilter filter in filters)
            {
                if (!filter.filter(line))
                {
                    return false;
                }
            }
            return true;
        }
    }
}