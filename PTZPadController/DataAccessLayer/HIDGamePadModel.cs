using System;
using System.Collections.Generic;

namespace PTZPadController.DataAccessLayer
{
    public class HIDGamePadModel
    {
        public string HidDeviceName { get; set; }

        public List<Tuple<HIDGamePadCommands,string[]>> MappedCommands { get; set; }
    }
}