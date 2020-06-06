using GalaSoft.MvvmLight.Threading;
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
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static App()
        {
            DispatcherHelper.Initialize();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            logger.Info("PTZPad Controller application start");

        }        
        
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            NLog.LogManager.Shutdown(); // Flush and close down internal threads and timers
        }


    }

}
