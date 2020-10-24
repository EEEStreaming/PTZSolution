
using PTZPadController.BusinessLayer;
using System;
using System.Collections.Generic;

namespace PTZPadController.DataAccessLayer
{
    public class HIDGamePadModel
    {
        public string HidDeviceName { get; set; }
        public List<MappCommnadModel> MappedCommands { get; set; }
    }
}