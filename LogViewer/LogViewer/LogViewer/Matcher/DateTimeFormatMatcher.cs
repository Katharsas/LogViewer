using System;
using System.Globalization;

namespace LogViewer.LogViewer.Matcher
{
    public class ParsedDateTime : IComparable<ParsedDateTime>, IComparable
    {
        public DateTime Value { get; }
        public string Original { get; }

        public ParsedDateTime(DateTime value, string original)
        {
            Value = value;
            Original = original;
        }

        public int CompareTo(ParsedDateTime other)
        {
            return Value.CompareTo(other.Value);
        }

        public int CompareTo(object obj)
        {
            if (obj != null && !(obj is ParsedDateTime))
                throw new ArgumentException("Object must be of type ParsedDateTime.");

            return CompareTo((ParsedDateTime) obj);
        }

        public override string ToString()
        {
            return Original;
        }
    }

    /// <summary>
    /// This matcher tries to parse a DateTime with given format from input.
    /// It counts whitespace sections up in format string, counts them down in input and then advances to
    /// the position where the given separator string can be found. This is necessary to avoid finding
    /// the separator string in the middle of the date and therefore stopping too early.
    /// <para/>
    /// This method allows the user to specify a separator string (which is "any whitespace" by default)
    /// even if the date contains that string / contains whitespace somewhere in the middle of the date.
    /// <para/>
    /// When the separator is found, the part of the string that is supposed to be a date will be parsed
    /// against given format string. DateTime format syntax:
    /// <para/>
    /// <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings">See Microsoft Documentation</a>
    /// </summary>
    class DateTimeFormatMatcher : InlineSeparatorMatcher<ParsedDateTime>
    {
        private string _format;

        public string Format
        {
            get { return _format; }
            set
            {
                if (_format != value)
                {
                    _format = value;
                    OnPropertyChanged();
                }
            }
        }

        public override IMatcherResult<ParsedDateTime> match(string statement, int startIndex)
        {
            int valueStart = parsePrefix(statement, startIndex);
            if (valueStart == -1) return null;

            int whitespaceCount = findWhiteSpaceOccurences(Format);
            int afterLastWhitespaceIndex = skipWhitespaceOccurences(statement, startIndex, whitespaceCount);
            if (afterLastWhitespaceIndex == -1) return null;

            int valueEnd = parseUntilSeparatorFound(statement, afterLastWhitespaceIndex);
            if (valueEnd == -1) return null;

            int suffixStart = parseSuffix(statement, valueStart, valueEnd, false);
            if (suffixStart == -1) return null;

            int next = valueEnd + Separator.Length;
            string original = statement.Substring(valueStart, suffixStart - valueStart);

            DateTime value;
            bool parsable = DateTime.TryParseExact(original, Format, null, DateTimeStyles.AllowWhiteSpaces, out value);
            if (!parsable) return null;

            var result = new ParsedDateTime(value, original);
            return new MatcherResult<ParsedDateTime>(next, result);
        }

        /// <summary>
        /// Counts how often whitespace can be found in given string, including at start / end.
        /// Multiple whitespace chars next to each other are counted as 1 occurence.
        /// </summary>
        protected static int findWhiteSpaceOccurences(string str)
        {
            bool wasLastWhitespace = false;
            int whitespaceFound = 0;
            for (int i = 0; i < str.Length; i++)
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
                    }
                }
            }
            return whitespaceFound;
        }

    }
}
