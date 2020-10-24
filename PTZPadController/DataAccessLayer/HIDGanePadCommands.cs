using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PTZPadController.DataAccessLayer
{
    public enum HIDGamePadCommands
    {
        [Description("X")]
        GenericDesktopX,
        [Description("Y")]
        GenericDesktopY,
        [Description("Z")]
        GenericDesktopZ,
        [Description("Rx")]
        GenericDesktopRx,
        [Description("Ry")]
        GenericDesktopRy,
        [Description("Rz")]
        GenericDesktopRz,
        [Description("Slider")]
        GenericDesktopSlider,
        [Description("Hat switch")]
        GenericDesktopHatSwitch,
        [Description("Button 1")]
        Button1,
        [Description("Button 2")]
        Button2,
        [Description("Button 3")]
        Button3,
        [Description("Button 4")]
        Button4,
        [Description("Button 5")]
        Button5,
        [Description("Button 6")]
        Button6,
        [Description("Button 7")]
        Button7,
        [Description("Button 8")]
        Button8,
        [Description("Button 9")]
        Button9,
        [Description("Button 10")]
        Button10,
        [Description("Button 11")]
        Button11,
        [Description("Button 12")]
        Button12,
        [Description("Button 13")]
        Button13,
        [Description("Button 14")]
        Button14,
        [Description("Button 15")]
        Button15,
        [Description("Button 16")]
        Button16,

    }
}
