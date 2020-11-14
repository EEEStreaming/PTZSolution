// Licensed under the Apache License, Version 2.0 (the "License").
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using GalaSoft.MvvmLight.Messaging;
using HIDDevices;
using HIDDevices.Controllers;
using HIDDevices.Usages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTZPadController.BusinessLayer;
using PTZPadController.Common;
using PTZPadController.DataAccessLayer;
using PTZPadController.Messages;

namespace PTZPadController.DataAccessLayer
{
    public class HIDParser : IHIDParser
    {
        private const int MAX_SIMULTANEOUS_COMMANDS = 7;
        private IGamePadHandler _PadHandler;
        private List<HIDGamePadModel> _ConfigGamePad;
        private TaskCompletionSource<bool> _DisconnectTcs;
        private bool is_connected = false;
        /// <inheritdoc />
        public string Description =>
            "Configuration of controllers in a dependency injection scenario.";

        public bool Connected { get { return is_connected; } }

        /// <inheritdoc />
        public async Task ExecuteAsync(CancellationToken token = default)
        {
            // Create an example service provider (this may be done by your framework code already)
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // NOTE: ServiceProvider should be disposed asynchronously by most frameworks, however many examples don't show this,
            // Devices also supports asynchronous disposal, and so should be disposed automatically by the service provider
            // when it is disposed.  If you create a Devices object yourself, also use await using to ensure correct disposal.
            await using var serviceProvider = serviceCollection.BuildServiceProvider();

            // Get the logger
            var logger = PTZLogger.Log;

            // Grab the controllers service
            var controllers = serviceProvider.GetService<Devices>();

            // Subscribe to changes in controllers
            using var subscription1 = controllers
                .ControlUsagesAll(GenericDesktopPage.X, ButtonPage.Button10)
                .Subscribe(changes =>
                {
                    var logBuilder = new StringBuilder();
                    var first = true;
                    foreach (var change in changes)
                    {
                        if (first)
                            first = false;
                        else
                            logBuilder.AppendLine();

                        var device = change.Current;
                        var padCfg = _ConfigGamePad.FirstOrDefault(g => g.HidDeviceName == device.Name);
                        if (padCfg != null)
                        {
                            logBuilder.Append("  Device :'").Append(device).Append("' was ");
                            switch (change.Reason)
                            {
                                case ChangeReason.Add:
                                    logBuilder.AppendLine("added.");
                                    is_connected = true;
                                    Messenger.Default.Send(new NotificationMessage<IHIDParser>(this, NotificationSource.GamePadConnected));
                                    break;
                                case ChangeReason.Update:
                                    logBuilder.AppendLine("updated.");
                                    break;
                                case ChangeReason.Remove:
                                    is_connected = false;
                                    Messenger.Default.Send(new NotificationMessage<IHIDParser>(this, NotificationSource.GamePadConnected));
                                    // Warning, the controller will be in the process of being disposed, so you should not access it's methods
                                    // (ToString() is safe though, and is all that is being accessed above).
                                    logBuilder.AppendLine("removed.");
                                    break;
                            }


                            logBuilder.Append("    DevicePath: ")
                                .AppendLine(device.DevicePath)
                                .Append("    Usages: ")
                                .AppendLine(string.Join(", ", device.Usages))
                                .Append("    Controls: ")
                                .AppendLine(string.Join(", ", device.Keys));
                        }
                        else if (change.Reason == ChangeReason.Add)
                        {
                            logBuilder.Append("  Device :'").Append(device).AppendLine("' connected but not configurated for the application.");
                            logBuilder.Append("    DevicePath: ")
                                .AppendLine(device.DevicePath)
                                .Append("    Usages: ")
                                .AppendLine(string.Join(", ", device.Usages))
                                .Append("    Controls: ")
                                .AppendLine(string.Join(", ", device.Keys));
                        }
                    }
                    logger.Info(logBuilder.ToString());
                });

            // Subscribe to all button control changes
            var batch = 0;
            using var subscription2 = controllers
                // Watch for control changes only
                .ControlChanges()
                .Subscribe(changes =>
                {
                    // Log the changes and look for a press of Button 1 on any controller.
                   if (changes.Count > MAX_SIMULTANEOUS_COMMANDS)
                    {
                        return;
                    }
                    foreach (var group in changes.GroupBy(c => c.Control.Device))
                    {
                        var padCfg = _ConfigGamePad.FirstOrDefault(g => g.HidDeviceName == group.Key.Name);
                        if (padCfg!= null)
                        {
                            var logBuilder = new StringBuilder();
                            logBuilder.Append("Batch ").Append(++batch).AppendLine(":");

                            logBuilder.Append("  ").Append(group.Key).AppendLine(":");

                            foreach (var change in group)
                            {
                                logBuilder
                                    .Append("    ")
                                    .Append(change.Control.Name)
                                    .Append(": Index :").Append(change.Control.Index).Append(" :")
                                    .Append(change.PreviousValue.ToString("F3"))
                                    .Append(" -> ")
                                    .Append(change.Value.ToString("F3"))
                                    .Append(" (")
                                    .Append(change.Elapsed.TotalMilliseconds.ToString("0.###"))
                                    .AppendLine("ms)");

                                //check if control is in the configuration gamepad
                                var mappedCommand = padCfg.MappedCommands.FirstOrDefault(map => map.GamePadCommand.Any(cmd => cmd.GetDescription() == change.Control.Name));
                                if (mappedCommand != null)
                                {
                                    bool CameraPanTiltAxesExecuted = false;
                                    ButtonCommand btnSatus = ButtonCommand.NA;
                                    switch (mappedCommand.PTZCommand)
                                    {
                                        case GamePadHandlerCommands.CameraPanTiltAxes:
                                            //set on reset status
                                            double _x = 0.5;
                                            double _y = 0.5;
                                            if (!CameraPanTiltAxesExecuted)
                                            {
                                                //we are in CameraPanTiltAxes command. First Axe is for camera X, and second config is for camera y
                                                if (mappedCommand.GamePadCommand.Count() == 2)
                                                {
                                                    string axeName = mappedCommand.GamePadCommand[0].GetDescription();
                                                    var ctrl = group.Key.Values.FirstOrDefault(c => c.Control.Name == axeName);
                                                    if (ctrl != null)
                                                    {
                                                        _x = ctrl.Value;
                                                    }
                                                    axeName = mappedCommand.GamePadCommand[1].GetDescription();
                                                    ctrl = group.Key.Values.FirstOrDefault(c => c.Control.Name == axeName);
                                                    if (ctrl != null)
                                                    {
                                                        _y = ctrl.Value;
                                                    }

                                                    _PadHandler.CameraPanTiltAxes(_x, _y);
                                                }
                                                
                                                CameraPanTiltAxesExecuted = true;
                                            }
                                            break;
                                        case GamePadHandlerCommands.CameraZoomAxe:
                                            if (change.Control.Name == HIDGamePadCommands.GenericDesktopHatSwitch.GetDescription())
                                            {
                                                //It's a switch. converte to axe
                                                if (change.Value == 0.0)
                                                    _PadHandler.CameraZoomAxe(0.0);
                                                else if (Math.Round(change.Value, 1) == 0.6)
                                                    _PadHandler.CameraZoomAxe(1.0);
                                                else if (double.IsNaN(change.Value))
                                                    _PadHandler.CameraZoomAxe(0.5);
                                            }
                                            else
                                                _PadHandler.CameraZoomAxe(change.Value);
                                            break;
                                        case GamePadHandlerCommands.CameraPreset1:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.CameraPreset1(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.CameraPreset2:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.CameraPreset2(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.CameraPreset3:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.CameraPreset3(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.CameraPreset4:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.CameraPreset4(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.CameraPreset5:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.CameraPreset5(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.CameraPreset6:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.CameraPreset6(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.CameraPreset7:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.CameraPreset7(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.CameraPreset8:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.CameraPreset8(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.CameraFocusAutoMode:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.CameraFocusAutoMode(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.CameraFocusOnePushMode:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.CameraFocusOnePushMode(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.CameraFocusOnePushTriger:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.CameraFocusOnePushTriger(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.CameraFocusAutoOnePushSwitchMode:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.CameraFocusAutoOnePushSwitchMode(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.Camera1SetPreview:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.Camera1SetPreview(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.Camera2SetPreview:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.Camera2SetPreview(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.Camera3SetPreview:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.Camera3SetPreview(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.Camera4SetPreview:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.Camera4SetPreview(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.CameraNextPreview:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.CameraNextPreview(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.CameraProgramSetPreview:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.CameraProgramSetPreview(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.SwitcherCut:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.SwitcherCut(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.SwitcherMix:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.SwitcherMix(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.CameraSetSensitivity:
                                            _PadHandler.CameraSetSensitivity(change.Value);
                                            break;
                                        case GamePadHandlerCommands.CameraNextSensitivity:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.CameraNextSensitivity(btnSatus);
                                            break;
                                        case GamePadHandlerCommands.InverseYAxis:
                                            btnSatus = GetButtonStatus(change.Value);
                                            _PadHandler.InverseYAxis(btnSatus);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                    switch (change.Control.Name)
                                {
                                    case "Button 1" :
                                        if (change.Value == 1)
                                        {
                                            _PadHandler.Camera1SetPreview(ButtonCommand.Up);
                                        }
                                        break;
                                    case "Button 2":
                                        if (change.Value == 1)
                                        {
                                            _PadHandler.Camera2SetPreview(ButtonCommand.Up);
                                        }
                                        break;
                                    case "Button 3":
                                        if (change.Value == 1)
                                        {
                                            _PadHandler.Camera3SetPreview(ButtonCommand.Up);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }

                            logger.Info(logBuilder.ToString());

                            

                        }
                    }

                });


            _DisconnectTcs = new TaskCompletionSource<bool>();
            logger.Info("HID Parser wait for disconnection");

            // Wait on signal from method StopAsync to stop HIDParser
            await _DisconnectTcs.Task.ConfigureAwait(false);

            logger.Info("HID Parser Finished");
        }

        private ButtonCommand GetButtonStatus(double value)
        {
            if (value == 1.0)
                return ButtonCommand.Down;
            if (value == 0.0)
                return ButtonCommand.Up;
            return ButtonCommand.NA;
        }

        public void Initialize(List<HIDGamePadModel> gamePads, IGamePadHandler padHandler)
        {
            _PadHandler = padHandler;
            _ConfigGamePad = gamePads;
        }

        private static void ConfigureServices(IServiceCollection services) =>
            // Configure logging and add the Devices service as a singleton.
            services
                //.AddLogging(configure => configure.AddConsole())
                .AddSingleton<Devices>();

        public void StopAsync()
        {
            if (_DisconnectTcs != null)
             _DisconnectTcs.TrySetResult(true);
            _DisconnectTcs = null;
        }
    }
}
