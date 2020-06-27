using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PTZPadController.PresentationLayer
{
    public interface IDisplayMessage
    {
        MessageBoxResult Show(string message);
        MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage icon);
    }
}
