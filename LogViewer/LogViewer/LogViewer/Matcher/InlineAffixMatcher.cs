using System;
using LogViewer.LogViewer.util;

namespace LogViewer.LogViewer.Matcher
{
    /// <summary>
    /// Provides matchers with abstract prefix/suffix functionality to reduce code duplication.
    /// </summary>
    abstract class InlineAffixMatcher<T> : AbstractMatcher<T> where T : IComparable
    {
        private string _ignoredPrefix = "";
        public string ignoredPrefix
        {
            get { return _ignoredPrefix; }
            set
            {
                if (_ignoredPrefix != value)
                {
                    _ignoredPrefix = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ignoredSuffix = "";
        public string ignoredSuffix
        {
            get { return _ignoredSuffix; }
            set
            {
                if (_ignoredSuffix != value)
                {
                    _ignoredSuffix = value;
                    OnPropertyChanged();
                }
            }
        }

        public abstract override IMatcherResult<T> match(string statement, int startIndex);

        /// <summary>
        /// Tries to parse prefix. Whitespace at start of prefix is ignored.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="startIndex"></param>
        /// <returns>-1 if statement is not parsable, otherwise the index of the char after the prefix</returns>
        protected int parsePrefix(string statement, int startIndex)
        {
            string trimmedPrefix = ignoredPrefix.TrimStart();
            if (trimmedPrefix.Length == 0)
            {
                return startIndex;
            }
            int valueStart = startIndex + trimmedPrefix.Length;
            if (valueStart > statement.Length)
            {
                return -1; // prefix doesn't fit
            }
            int index = statement.IndexOf(trimmedPrefix, startIndex, trimmedPrefix.Length, StringComparison.Ordinal);
            if (index != startIndex)
            {
                return -1;
            }
            return valueStart;
        }

        /// <summary>
        /// Tried to parse suffix. Suffix chars must start/end/overlap at valueEnd.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="valueStart"></param>
        /// <param name="valueEnd"></param>
        /// <param name="searchForwards">True if suffix is allowed to end behind valueEnd, False if it must end exactly at valueEnd.</param>
        /// <returns>-1 if statement is not parsable, otherwise the index of the first suffix char</returns>
        protected int parseSuffix(string statement, int valueStart, int valueEnd, bool searchForwards)
        {
            if (ignoredSuffix.Length == 0)
            {
                return valueEnd;
            }
            
            if (!searchForwards)
            {
                int suffixStart = valueEnd - ignoredSuffix.Length;
                if (suffixStart < valueStart)
                {
                    return -1; // suffix doesn't fit
                }
                return statement.IndexOf(ignoredSuffix, suffixStart, ignoredSuffix.Length, StringComparison.Ordinal);
            }
            else
            {
                // how many chars before value end do we want to start searching for the start of suffix?
                int valueLength = valueEnd - valueStart;
                int searchLengthBackwards = ignoredSuffix.Length.ClampMax(valueLength);

                // how many chars after value end do we include into search for suffix?
                int searchLengthForwards = ignoredSuffix.Length.ClampMax(statement.Length - valueEnd);

                int indexSearchStart = valueEnd - searchLengthBackwards;
                int indexSearchCount = searchLengthBackwards + searchLengthForwards;
                if (indexSearchCount < ignoredSuffix.Length)
                {
                    return -1; // suffix doesn't fit
                }
                return statement.IndexOf(ignoredSuffix, indexSearchStart, indexSearchCount, StringComparison.Ordinal);
            }
        }
    }
}
