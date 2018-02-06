using System;
using System.Collections.Generic;

namespace LogViewer.LogViewer.Model
{
    /// <summary>
    /// Represents a single logging statement. Contains both source strings (split into first line for meta values and
    /// additional lines for message/stacktrace) and parsed meta values / message / stacktrace.
    /// 
    /// If the atom has less meta values than matchers were used for parsing (including hardcoded RemainingLineMatcher),
    /// the parsing failed.
    /// 
    /// Note: Multi-line meta information not implemented, but it should be possible to extend this to make that work too.
    /// </summary>
    public class LogAtom
    {
        public string RawMetaLine { get; }
        public int LineNumber { get; }

        public List<string> RawAdditionalLines { get; } // not immutable!
        public List<IComparable> MetaValues { get; set; } // not immutable!

        public LogAtom(string rawMetaLine, int lineNumber)
        {
            RawMetaLine = rawMetaLine;
            LineNumber = lineNumber;
            RawAdditionalLines = new List<string>();
        }

        public override string ToString()
        {
            if (MetaValues == null)
            {
                return LineNumber + ": null";
            }
            else
            {
                return LineNumber + ": [" + String.Join(", ", MetaValues) + "]";
            }
        }
    }
}