/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:PTZPadController"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using PTZPadController.BusinessLayer;
using PTZPadController.DataAccessLayer;
using PTZPadController.Design;
using System;

namespace PTZPadController.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            //ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IPTZManager, DesignPTZManager>();
            }
            else
            {
                SimpleIoc.Default.Register<IPTZManager, PTZManager>();

                //Load Configuration
            }

            SimpleIoc.Default.Register<PTZMainViewModel>();

        }


        public PTZMainViewModel Main
        {
            get
            {
                return SimpleIoc.Default.GetInstance<PTZMainViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        /// <summary>
        /// Initialize the whole system
        /// </summary>
        internal void Initialize()
        {
            //Load configuration
            var cfg = ConfigurationFileParser.LoadConfigurationFile("Configuration.json");

            var ptzManager = SimpleIoc.Default.GetInstance<IPTZManager>();

            ptzManager.InitSeetings(cfg);

            //Create and connect connection to ATEM
            var atemHandler = new AtemSwitcherHandler(cfg.AtemHost);
            ptzManager.SetAtemHandler(atemHandler);

            //Create How many Camera
            foreach (var camcfg in cfg.Cameras)
            {
                var cam = new CameraHandler();
                var camParser = new CameraPTC140Parser();
                var socket = new SocketAutoConnectParser();
                socket.Initialize(camcfg.CameraName, camcfg.CameraHost, camcfg.CameraPort, camParser);
                camParser.Initialize(socket);
                cam.Initialize(camParser);
                ptzManager.AddCameraHandler(cam);

            }

            //Create and connect to pad


            //Startup the whole system
            ptzManager.StartUp();

        }
    }
}