using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PTZPadController.PresentationLayer
{
    class DisplayMessage : IDisplayMessage
    {
        public MessageBoxResult Show(string message)
        {
           return MessageBox.Show(message, "PTZPad Controller");
        }

        public MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return MessageBox.Show(message,caption,button,icon);
        }

    }
}
