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
using HIDDevices;
using HIDDevices.Controllers;
using HIDDevices.Usages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;

namespace PTZPadController.DataAccessLayer
{
    public class HIDParser : IHIDParser
    {
        private IGamePadHandler _PadHandler;
        private List<HIDGamePadModel> _ConfigGamePad;
        private TaskCompletionSource<bool> _DisconnectTcs;

        /// <inheritdoc />
        public string Description =>
            "Demonstrates configuration of controllers in a dependency injection scenario.";

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
            //using var subscription1 = controllers
            //    .ControlUsagesAll(GenericDesktopPage.X, ButtonPage.Button10)
            //    //.Connect()
            //    .Subscribe(changeSet =>
            //    {
            //        var logBuilder = new StringBuilder();
            //        logBuilder.AppendLine("Devices updated:");
            //        var first = true;
            //        foreach (var change in changeSet)
            //        {
            //            if (first)
            //            {
            //                first = false;
            //            }
            //            else
            //            {
            //                logBuilder.AppendLine(null);
            //            }

            //            var device = change.Current;
            //            logBuilder.Append("  The ")
            //                .Append(device)
            //                .Append(" Device  was ");
            //            switch (change.Reason)
            //            {
            //                case ChangeReason.Add:
            //                    logBuilder.AppendLine("added.");

            //                    break;
            //                case ChangeReason.Update:
            //                    logBuilder.AppendLine("updated.");
            //                    break;
            //                case ChangeReason.Remove:
            //                    // Warning, the controller will be in the process of being disposed, so you should not access it's methods
            //                    // (ToString() is safe though, and is all that is being accessed above).
            //                    logBuilder.AppendLine("removed.");
            //                    break;
            //            }

            //            logBuilder.Append("    DevicePath: ")
            //                .AppendLine(device.DevicePath)
            //                .Append("    Usages: ")
            //                .AppendLine(string.Join(", ", device.Usages))
            //                .Append("    Controls: ")
            //                .AppendLine(string.Join(", ", device.Keys));
            //        }

            //        logger.Info(logBuilder.ToString());
            //    });

            // Subscribe to all button control changes
            var batch = 0;
            using var subscription2 = controllers
                // Watch for control changes only
                .ControlChanges()
                .Subscribe(changes =>
                {
                    // Log the changes and look for a press of Button 1 on any controller.
                   
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

                                switch (change.Control.Name)
                                {
                                    case "Button 1" :
                                        if (change.Value == 1)
                                        {
                                            _PadHandler.CameraSetPreview(1);
                                        }
                                        break;
                                    case "Button 2":
                                        if (change.Value == 1)
                                        {
                                            _PadHandler.CameraSetPreview(2);
                                        }
                                        break;
                                    case "Button 3":
                                        if (change.Value == 1)
                                        {
                                            _PadHandler.CameraSetPreview(3);
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
            //using var subscription3 = controllers
            //    // Watch for button one changes only
            //    .ControlChanges(c => c.ButtonNumber == 1)
            //    //&& !c.Device.Usages.Contains(65538u))
            //    .Subscribe(changes =>
            //    {
            //        if (changes.Any(c => c.Value > 0.5))
            //        {
            //            DisconnectTcs.TrySetResult(true);
            //        }
            //    });

            // Subscribe to a specific controller type
            var gamepadBatch = 0;
            using var gamepadSubscription = controllers
                .Controllers<Gamepad>()
                .Do(gamepad =>
                {
                    var logBuilder = new StringBuilder();
                    logBuilder.Append(gamepad.Name)
                        .AppendLine(" found!  Following controls were mapped:");
                    foreach (var (control, infos) in gamepad.Mapping)
                    {
                        logBuilder.Append("  ")
                            .Append(control.Name)
                            .Append(" => ")
                            .AppendLine(string.Join(", ", infos.Select(info => info.PropertyName)));
                    }

                    logger.Info(logBuilder.ToString());

                    // Connect the gamepad, so we can start listening for changes.
                    gamepad.Connect();
                })
                .SelectMany(gamepad => gamepad.Changes)
                .Subscribe(changes =>
                {
                    var logBuilder = new StringBuilder();
                    logBuilder.Append("Gamepad Batch ").Append(++gamepadBatch).AppendLine(":");
                    foreach (var change in changes)
                    {
                        var valueStr = change.Value switch
                        {
                            bool b => b ? "Pressed" : "Not Pressed",
                            double d => d.ToString("F3"),
                            null => "<null>",
                            _ => change.Value.ToString()
                        };

                        logBuilder.Append("  ")
                            .Append(change.PropertyName)
                            .Append(": ")
                            .Append(valueStr)
                            .Append(" (")
                            .AppendFormat("{0:F3}", change.Elapsed.TotalMilliseconds)
                            .AppendLine("ms)");
                    }

                    logger.Info(logBuilder.ToString());
                });

            logger.Info("HI Parser wait for disconnection");

            // Wait on signal that Button 1 has been pressed
            await _DisconnectTcs.Task.ConfigureAwait(false);

            logger.Info("Finished");
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
