using PTZPadController.DataAccessLayer;
using PTZPadController.ViewModel;
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

        static public Window Win;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            PTZLogger.Log.Info("PTZPad Controller application start");

            //Load configuration
            var locator = this.FindResource("Locator") as ViewModelLocator;

            locator.Initialize();
        
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            PTZLogger.Shutdown();
        }

    }
}
