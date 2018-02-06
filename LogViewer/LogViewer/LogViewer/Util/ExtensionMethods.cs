using System;

namespace LogViewer.LogViewer.util
{
    static class ExtensionMethods
    {
        // see https://stackoverflow.com/questions/2683442/where-can-i-find-the-clamp-function-in-net
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
        public static T ClampMin<T>(this T val, T min) where T : IComparable<T>
        {
            return val.CompareTo(min) < 0 ? min : val;
        }
        public static T ClampMax<T>(this T val, T max) where T : IComparable<T>
        {
            return val.CompareTo(max) > 0 ? max : val;
        }
    }
}
