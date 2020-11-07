using PTZPadController.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace PTZPadController.Converter
{
    public class FocusModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ECameraFocusMode paramFocus = (ECameraFocusMode)parameter;
            ECameraFocusMode valueFocus = (ECameraFocusMode)value;

            return valueFocus == paramFocus ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
