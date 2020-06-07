﻿using PTZPadController.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PTZPadController
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            //DispatcherHelper.Initialize();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            PTZLogger.Log.Info("PTZPad Controller application start");

        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            PTZLogger.Shutdown();
        }

    }
}
