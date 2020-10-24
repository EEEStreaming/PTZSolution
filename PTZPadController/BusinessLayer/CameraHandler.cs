using GalaSoft.MvvmLight.Messaging;
using PTZPadController.DataAccessLayer;
using PTZPadController.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PTZPadController.BusinessLayer
{
    public class CameraHandler : ICameraHandler
    {
        private ICameraParser m_CamParser;

        public string CameraName { get { return m_CamParser.CameraName; } }

        public bool PanTileWorking { get; private set; }

        public bool ZoomWorking { get; private set; }

        public ICameraParserModel Parser { get { return m_CamParser; } }

        #region Constructor/Initialisation/Connection
        public CameraHandler()
        {
        }

        public void Initialize(ICameraParser camParser)
        {
            m_CamParser = camParser;
        }

        public Task ConnectTo()
        {
            Task t = Task.Run(() => { m_CamParser.Connect(); });
            return t;
        }
        public bool WaitForConnection()
        {
            while (!m_CamParser.Connected)
            {
                PTZLogger.Log.Info("Camera Handler {0} Sleeping for connexion", m_CamParser.CameraName);
                Thread.Sleep(800);
            }
            return m_CamParser.Connected;
        }
        #endregion

        #region Commands
        public void PanTiltUp(short moveSpeed)
        {
            if (m_CamParser != null)
            {
                PanTileWorking = true;
                m_CamParser.PanTiltUp(moveSpeed);
            }
        }

        public void PanTiltDown(short moveSpeed)
        {
            if (m_CamParser != null)
            {
                PanTileWorking = true;
                m_CamParser.PanTiltDown(moveSpeed);
            }
        }

        public void PanTiltLeft(short moveSpeed)
        {
            if (m_CamParser != null)
            {
                PanTileWorking = true;
                m_CamParser.PanTiltLeft(moveSpeed);
            }
        }

        public void PanTiltRight(short moveSpeed)
        {
            if (m_CamParser != null)
            {
                PanTileWorking = true;
                m_CamParser.PanTiltRight(moveSpeed);
            }
        }

        public void PanTiltUpLeft(short moveSpeed)
        {
            if (m_CamParser != null)
            {
                PanTileWorking = true;
                m_CamParser.PanTiltUpLeft(moveSpeed);
            }
        }

        public void PanTiltUpRight(short moveSpeed)
        {
            if (m_CamParser != null)
            {
                PanTileWorking = true;
                m_CamParser.PanTiltUpRight(moveSpeed);
            }
        }

        public void PanTiltDownLeft(short moveSpeed)
        {
            if (m_CamParser != null)
            {
                PanTileWorking = true;
                m_CamParser.PanTiltDownLeft(moveSpeed);
            }
        }

        public void PanTiltDownRight(short moveSpeed)
        {
            if (m_CamParser != null)
            {
                PanTileWorking = true;
                m_CamParser.PanTiltDownRight(moveSpeed);
            }
        }

        public void PanTiltStop()
        {
            if (m_CamParser != null)
            {
                m_CamParser.PanTiltStop();
            }
            PanTileWorking = false;
        }
        public void PanTiltHome()
        {
            //TODO, gerer PanTiltWorking (comment s'arrête ?)
            //throw new NotImplementedException();
            //m_CamParser.PanTiltHome();
            //PanTileWorking = true;
        }

        public void ZoomStop()
        {
            if (m_CamParser != null)
            {
                m_CamParser.ZoomStop();
            }
            ZoomWorking = false;
        }

        public void ZoomTele()
        {
            if (m_CamParser != null)
            {
                ZoomWorking = true;
                m_CamParser.ZoomTele();
            }
        }

        public void ZoomWide()
        {
            if (m_CamParser != null)
            {
                ZoomWorking = true;
                m_CamParser.ZoomWide();
            }
        }

        public void ZoomTele(short zoomSpeed)
        {
            if (m_CamParser != null)
            {
                ZoomWorking = true;
                m_CamParser.ZoomTele(zoomSpeed);
            }
        }

        public void ZoomWide(short zoomSpeed)
        {
            if (m_CamParser != null)
            {
                ZoomWorking = true;
                m_CamParser.ZoomWide(zoomSpeed);
            }
        }

        public void CameraMemoryReset(short memory)
        {
            //TODO, gerer recall et Set
            throw new NotImplementedException();
            //m_CamParser.CameraMemoryReset(memory);
        }

        public void CameraMemorySet(short memory)
        {
            //TODO, gerer reset et recall
            if (m_CamParser != null)
                m_CamParser.CameraMemorySet(memory);
        }

        public void CameraMemoryRecall(short memory)
        {
            //TODO, gerer PanTiltWorking (comment s'arrête ?)
            if (m_CamParser != null)
                m_CamParser.CameraMemoryRecall(memory);
            //PanTileWorking = true;
        }

        public void Tally(bool ledRed, bool ledGreen)
        {
            if (m_CamParser != null)
                m_CamParser.Tally(ledRed, ledGreen);
        }

        public void FocusModeAuto()
        {
            if (m_CamParser != null)
            {
                m_CamParser.FocusModeAuto();
                 
            }
        }

        public void FocusModeManual()
        {
            if (m_CamParser != null)
            { 
                m_CamParser.FocusModeManual();
                
            }
        }

        public void FocusModeOnePush()
        {
            if (m_CamParser != null)
            {
                m_CamParser.FocusModeOnePush();
            }
        }

        public void FocusOnePushTrigger()
        {
            if (m_CamParser != null)
            {
                m_CamParser.FocusOnePushTrigger();
            }
        }


        #endregion

        #region Methods
        public void CompletionMessage()
        {
            //TODO ????
            PTZLogger.Log.Debug("message OK");
        }

        #endregion

    }
}
