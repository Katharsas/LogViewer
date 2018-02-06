﻿using System;
using System.Windows.Data;

// ReSharper disable CheckNamespace
namespace WpfUtils
{
    /// <summary>
    /// Allows for example to bind radiobuttons to an enum, see
    /// <see><cref>https://stackoverflow.com/questions/397556/how-to-bind-radiobuttons-to-an-enum</cref></see>
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }
}
