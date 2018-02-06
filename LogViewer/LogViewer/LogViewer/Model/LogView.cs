using System;
using System.Collections.Generic;
using System.Linq;
using LogViewer.LogViewer.Filter;
using WpfUtils;

namespace LogViewer.LogViewer.Model
{
    /// <summary>
    /// Holds all state belonging to a full LogViewer view, which consists of the user's parsing settings (Filters, matchers),
    /// and the parsed lines / LogAtoms. Contains state to determine what and how much needs to be reparsed when settings change,
    /// and provides methods to trigger the (re)parsing.
    /// </summary>
    public class LogView
    {
        private IList<string> lines;
        public MyBindingList<LogAtom> Statements { get; }

        public MyBindingList<LineFilter> Filters { get; }
        public MatcherChain Matchers { get; }

        private MyBindingList<LogAtom> dirty;
        private bool areFiltersDirty;
        private bool areMatchersDirty;

        public LogView()
        {
            lines = new List<string>();
            Statements = new MyBindingList<LogAtom>();

            Filters = new MyBindingList<LineFilter>();
            Matchers = new MatcherChain();

            dirty = new MyBindingList<LogAtom>();

            areFiltersDirty = false;
            Filters.ListChanged += (sender, args) =>
            {
                areFiltersDirty = true;
            };

            areMatchersDirty = false;
            Matchers.PropertyChanged += (sender, args) =>
            {
                areMatchersDirty = true;
            };
        }

        public void clear()
        {
            Statements.Clear();
        }

        public void parseAddLines(string newLogLines)
        {
            string[] newLines = newLogLines.Split(
                new[] {"\r\n", "\n"},
                StringSplitOptions.None
            );

            int lineIndex = newLines.Length;
            for (int i = 0; i < newLines.Length; i++)
            {
                if (newLines[i] == "")
                {
                    continue;
                }

                int currentLineIndex = lineIndex + i;
                string currentLine = newLines[i] + Environment.NewLine;
                lines.Add(currentLine);

                parseLine(currentLine, currentLineIndex);
            }
            parseCleanup();
        }

        public void reparseDirtyFiltersAndMatchers()
        {
            Console.WriteLine("matchers: " + Matchers.Matchers.Count);
            if (areFiltersDirty || areMatchersDirty)
            {
                Statements.Clear();
                reparseExistingLines();
                areFiltersDirty = false;
                areMatchersDirty = false;
            }
        }

        private void reparseExistingLines()
        {
            for (int i = 0; i < lines.Count; i++)
            {
                int currentLineIndex = i;
                string currentLine = lines[i];
                parseLine(currentLine, currentLineIndex);
            }
            parseCleanup();
        }

        private void parseLine(string line, int lineIndex)
        {
            bool removed = !LineFilter.filter(line, Filters);
            if (!removed)
            {
                matchAddLine(line, lineIndex);
            }
        }

        private void parseCleanup()
        {
            foreach (LogAtom atom in dirty)
            {
                Matchers.reparseAtom(atom);
            }
            dirty.Clear();
        }

        /// <summary>
        /// Try to apply Matchers. Add as new atom if success, otherwise append to last atom.
        /// </summary>
        private void matchAddLine(string line, int lineIndex)
        {
            MatcherParseResult parseResult = Matchers.parseLine(lineIndex, line);
            if (!parseResult.isLineNewAtom)
            {
                if (Statements.Count > 0)
                {
                    LogAtom last = Statements.Last();
                    if (last.LineNumber >= lineIndex)
                    {
                        throw new InvalidOperationException("Invalid line sequence in statements list.");
                    }
                    last.RawAdditionalLines.Add(line);
                    markDirty(last);
                }
                else
                {
                    // create emergency atom even if this is not an atom (for displaying error)
                    LogAtom failed = parseResult.newAtom;
                    failed.RawAdditionalLines.Add("Parsing failed! Line: " + failed.RawMetaLine);
                    Statements.Add(failed);
                }
            }
            else
            {
                if (parseResult.isNewAtomParseSuccess)
                {
                    Statements.Add(parseResult.newAtom);
                }
                else
                {
                    LogAtom failed = parseResult.newAtom;
                    failed.RawAdditionalLines.Add("Parsing failed! Line: " + failed.RawMetaLine);
                    Statements.Add(failed);
                }
                
            }
        }

        /// <summary>
        /// Mark for re-matching.
        /// </summary>
        private void markDirty(LogAtom atom)
        {
            dirty.Add(atom);
        }
    }
}
