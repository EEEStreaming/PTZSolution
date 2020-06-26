using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PTZPadController.PresentationLayer
{
    class DisplayMessage : IDisplayMessage
    {
        public void Show(string message)
        {
            MessageBox.Show(message, "PTZPad Controller");
        }
    }
}
