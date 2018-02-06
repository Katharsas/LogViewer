using System;
using System.Threading;

namespace LogViewer.LogViewer.Matcher
{
    public class Number : IComparable<Number>, IComparable
    {
        public double Value { get; }
        public string Original { get; }

        public Number(double value, string original)
        {
            Value = value;
            Original = original;
        }

        public int CompareTo(Number other)
        {
            return Value.CompareTo(other.Value);
        }

        public int CompareTo(object obj)
        {
            if (obj != null && !(obj is Number))
                throw new ArgumentException("Object must be of type Number.");

            return CompareTo((Number) obj);
        }

        public override string ToString()
        {
            return Original;
        }
    }

    /// <summary>
    /// Matches a number. May be a simple number like "42" or a number containing given decimal/group
    /// separators like "1.122,67". Group separator cannot appear after decimal separator, and anything
    /// else must be digits.
    /// </summary>
    class NumberMatcher : InlineAffixMatcher<Number>
    {
        public static readonly char defaultDecimalSeparator = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
        public static readonly char defaultGroupSeparator = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator);

        private char? _decimalSeparator = defaultDecimalSeparator;
        private char? _groupSeparator = defaultGroupSeparator;

        public char? decimalSeparator
        {
            get { return _decimalSeparator; }
            set
            {
                if (_decimalSeparator != value)
                {
                    _decimalSeparator = value;
                    OnPropertyChanged();
                }
            }
        }

        public char? groupSeparator
        {
            get { return _groupSeparator; }
            set
            {
                if (_groupSeparator != value)
                {
                    _groupSeparator = value;
                    OnPropertyChanged();
                }
            }
        }

        public override IMatcherResult<Number> match(string statement, int startIndex)
        {
            int valueStart = parsePrefix(statement, startIndex);
            bool parsable = valueStart != -1;

            if (parsable)
            {
                int current = valueStart;

                // decimal seperator can only appear once
                bool beforeSeparator = true;

                for (int i = current; i < statement.Length; i++)
                {
                    char c = statement[i];
                    if (Char.IsDigit(c))
                    {
                        continue;
                    }
                    else if (c == groupSeparator && beforeSeparator)
                    {
                        continue;
                    }
                    else if (c == decimalSeparator)
                    {
                        if (beforeSeparator)
                        {
                            beforeSeparator = false;
                            continue;
                        }
                    }
                    // if not continued by now, end of number is reached
                    current = i;
                    break;
                }

                // try to parse suffix
                // the already parsed number can contain parts of the suffix, calculate which part to check
                // Example:

                // Statement: "[7,500]"
                // Prefix: "["
                // Suffix: "00]"
                // Parsed Number: "7,500"
                // Resulting Number after matching suffix parts are removed: "7,5"

                int valueEnd = current;
                int suffixStart = parseSuffix(statement, valueStart, valueEnd, true);
                parsable = suffixStart != -1;
                parsable = parsable && (suffixStart - valueStart) > 0;

                if (parsable)
                {
                    current = suffixStart + ignoredSuffix.Length;
                    string stringValue = statement.Substring(valueStart, suffixStart - valueStart);
                    double parsedValue = Double.Parse(stringValue);
                    Number result = new Number(parsedValue, stringValue);
                    return new MatcherResult<Number>(current, result);
                }
            }
            return null;
        }
    }
}
