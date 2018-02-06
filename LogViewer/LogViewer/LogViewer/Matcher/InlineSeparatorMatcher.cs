using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer.LogViewer.Matcher
{
    /// <summary>
    /// Provides functionality for matchers that use Separator as well as affixes.
    /// The structure of the input string is assumed to be like this:
    /// <para/>
    /// ... [Prefix] [Value] [Suffix] [Separator] ...
    /// </summary>
    abstract class InlineSeparatorMatcher<T> : InlineAffixMatcher<T> where T : IComparable
    {
        private string _separator = "";
        public string Separator
        {
            get { return _separator; }
            set
            {
                if (_separator != value)
                {
                    _separator = value;
                    OnPropertyChanged();
                }
            }
        }

        protected int parseUntilSeparatorFound(string str, int startIndex)
        {
            int current = startIndex;

            bool isWhitespaceSeparator = Separator == "";
            if (isWhitespaceSeparator)
            {
                for (; current < str.Length; current++)
                {
                    if (char.IsWhiteSpace(str[current]))
                    {
                        break;
                    }
                }
                return current;
            }
            else
            {
                return str.IndexOf(Separator, startIndex, StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Returns position in string at first char after specified amount of whitespace occurences
        /// where found. Multiple whitespace chars next to each other are counted as 1 occurence.
        /// </summary>
        protected static int skipWhitespaceOccurences(string str, int startIndex, int occurences)
        {
            if (occurences == 0)
            {
                return startIndex;
            }

            bool wasLastWhitespace = false;
            int whitespaceFound = 0;
            for (int i = startIndex; i < str.Length; i++)
            {
                if (char.IsWhiteSpace(str[i]))
                {
                    if (!wasLastWhitespace)
                    {
                        whitespaceFound++;
                        wasLastWhitespace = true;
                    }
                }
                else
                {
                    if (wasLastWhitespace)
                    {
                        wasLastWhitespace = false;
                        if (whitespaceFound == occurences)
                        {
                            int afterLastOccurence = i;
                            return afterLastOccurence;
                        }
                    }
                }
            }
            return -1;
        }
    }
}
