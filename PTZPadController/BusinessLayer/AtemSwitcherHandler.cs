using BMDSwitcherAPI;
using GalaSoft.MvvmLight.Messaging;
using NLog.Fluent;
using PTZPadController.DataAccessLayer;
using PTZPadController.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace PTZPadController.BusinessLayer
{


    class AtemSwitcherHandler : ISwitcherHandler
    {
        private ISwitcherParser m_SwitcherParser;
        private bool m_Initialized;



        #region Constructor/Initialisation/Connection
        public AtemSwitcherHandler()
        {
            m_Initialized = false;
        }

        public void Initialize(ISwitcherParser parser)
        {
            m_SwitcherParser = parser;
            m_Initialized = true;
        }

        public void ConnectTo()
        {
            m_SwitcherParser.Connect();

            //var t = Task.Run(() =>
            //{
            //    //m_SwitcherParser.Connect();
            //});
            //return t;
        }

        public bool WaitForConnection()
        {
            while (!m_SwitcherParser.Connected)
            {
                PTZLogger.Log.Info("Atem Switcher Handler Sleeping for connexion");
                Thread.Sleep(800);
            }
            return m_SwitcherParser.Connected;
        }

        public void Disconnect()
        {
            m_SwitcherParser.Disconnect();
        }
        #endregion
        
        #region Switcher Commands
        public void SetPreviewSource(string cameraName)
        {
            m_SwitcherParser.SetPreviewSource(cameraName);
        }

        public void StartTransition(TransitionEnum transition)
        {
            m_SwitcherParser.StartTransition(transition);
        }

        public string GetCameraProgramName()
        {
            return m_SwitcherParser.GetCameraProgramName();
        }

        public string GetCameraPreviewName()
        {
            return m_SwitcherParser.GetCameraPreviewName();
        }
        #endregion
    }
}