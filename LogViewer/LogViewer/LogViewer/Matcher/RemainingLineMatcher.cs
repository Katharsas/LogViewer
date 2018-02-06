namespace LogViewer.LogViewer.Matcher
{
    /// <summary>
    /// Simply matches everything that remains.
    /// </summary>
    class RemainingLineMatcher : AbstractMatcher<string>
    {
        public override IMatcherResult<string> match(string statement, int startIndex)
        {
            string remaining = statement.Substring(startIndex);
            remaining = remaining.TrimEnd();
            return new MatcherResult<string>(statement.Length, remaining);
        }
    }
}
