using System;

namespace LogViewer.LogViewer.Matcher
{
    /// <summary>
    /// This matcher tries to find the first occurence of the configured Separator string and
    /// returns the text up until that Separator as value. If Separator is an empty string, it is
    /// interpreted as "any whitespace".
    /// </summary>
    class TextConfigurableMatcher : InlineSeparatorMatcher<string>
    {
        public override IMatcherResult<string> match(string statement, int startIndex)
        {
            int valueStart = parsePrefix(statement, startIndex);
            if (valueStart == -1) return null;

            int valueEnd = parseUntilSeparatorFound(statement, valueStart);
            if (valueEnd == -1) return null;

            int suffixStart = parseSuffix(statement, valueStart, valueEnd, false);
            if (suffixStart == -1) return null;

            int next = valueEnd + Separator.Length;
            string result = statement.Substring(valueStart, suffixStart - valueStart);

            return new MatcherResult<string>(next, result);
        }
    }
}
