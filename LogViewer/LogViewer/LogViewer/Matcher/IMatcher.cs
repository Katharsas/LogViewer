using System;
using System.ComponentModel;
using WpfUtils;

namespace LogViewer.LogViewer.Matcher
{
    /// <summary>
    /// Return type for <see cref="IMatcher{T}.match"/> function.
    /// If a matcher returns this (not null), the matching was successfull and the matched/parsed
    /// <see cref="Value"/> can be retrieved. The matcher has parsed the given line(s) up to 
    /// positon <see cref="NextIndex"/>, which can be used as startIndex for the next matcher.
    /// </summary>
    public interface IMatcherResult<out T> where T : IComparable
    {
        int NextIndex { get; }
        T Value { get; }
    }

    /// <summary>
    /// Abstract base class for all <see cref="IMatcherResult{T}"/> which implements the needed properties.
    /// </summary>
    public class MatcherResult<T> : IMatcherResult<T> where T : IComparable
    {
        public int NextIndex { get; }
        public T Value { get; }

        public MatcherResult(int nextIndex, T value)
        {
            NextIndex = nextIndex;
            Value = value;
        }
    }

    /// <summary>
    /// A matcher in general tries to parse a string from a given start position until a value has been found.
    /// It then returns the value as well as where the matcher has stopped parsing the string, see <see cref="IMatcherResult{T}"/>,
    /// so the next Matcher can continue to extract values from the string.
    /// </summary>
    /// <typeparam name="T">The type of the value that was computed by a matcher from the parsed string.</typeparam>
    public interface IMatcher<out T> : INotifyPropertyChanged where T : IComparable
    {
        string Name { get; set; }
        IMatcherResult<T> match(string statement, int startIndex);
        Type getResultValueType();
    }

    public abstract class AbstractMatcher<T> : NotifyPropertyChanged, IMatcher<T> where T : IComparable
    {
        private string _name = "";
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public abstract IMatcherResult<T> match(string statement, int startIndex);

        public Type getResultValueType()
        {
            return typeof(T);
        }
    }
}
