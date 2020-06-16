using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace PTZPadController.Converter
{
    public sealed class ParametrizedBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;

            if (value is bool)
                flag = (bool)value;
            else
            {
                if (value is bool?)
                {
                    bool? flag2 = (bool?)value;
                    flag = (flag2.HasValue && flag2.Value);
                }
            }

            //If false is passed as a converter parameter then reverse the value of input value
            if (parameter != null)
            {
                bool par = true;
                if ((bool.TryParse(parameter.ToString(), out par)) && (!par)) flag = !flag;
            }

            return flag ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
                return (Visibility)value == Visibility.Visible;

            return false;
        }

        public ParametrizedBooleanToVisibilityConverter()
        {
        }
    }
}
