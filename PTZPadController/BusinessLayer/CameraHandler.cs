using PTZPadController.DataAccessLayer;
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
        public void PanTiltUp(short panSpeed, short tiltSpeed)
        {
            if (m_CamParser != null)
                m_CamParser.PanTiltUp(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltDown(short panSpeed, short tiltSpeed)
        {
            if (m_CamParser != null)
                m_CamParser.PanTiltDown(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltLeft(short panSpeed, short tiltSpeed)
        {
            if (m_CamParser != null)
                m_CamParser.PanTiltLeft(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltRight(short panSpeed, short tiltSpeed)
        {
            if (m_CamParser != null)
                m_CamParser.PanTiltRight(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltUpLeft(short panSpeed, short tiltSpeed)
        {
            if (m_CamParser != null)
                m_CamParser.PanTiltUpLeft(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltUpRight(short panSpeed, short tiltSpeed)
        {
            if (m_CamParser != null)
                m_CamParser.PanTiltUpRight(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltDownLeft(short panSpeed, short tiltSpeed)
        {
            if (m_CamParser != null)
                m_CamParser.PanTiltDownLeft(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltDownRight(short panSpeed, short tiltSpeed)
        {
            if (m_CamParser != null)
                m_CamParser.PanTiltDownRight(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltStop()
        {
            if (m_CamParser != null)
                m_CamParser.PanTiltStop();
            PanTileWorking = false;
        }
        public void PanTiltHome()
        {
            //TODO, gerer PanTiltWorking (comment s'arrête ?)
            throw new NotImplementedException();
            //m_CamParser.PanTiltHome();
            //PanTileWorking = true;
        }

        public void ZoomStop()
        {
            if (m_CamParser != null)
                m_CamParser.ZoomStop();
            ZoomWorking = false;
        }

        public void ZoomTele()
        {
            if (m_CamParser != null)
                m_CamParser.ZoomTele();
            ZoomWorking = true;
        }

        public void ZoomWide()
        {
            if (m_CamParser != null)
                m_CamParser.ZoomWide();
            ZoomWorking = true;
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
            throw new NotImplementedException();
            //m_CamParser.CameraMemorySet(memory);
        }

        public void CameraMemoryRecall(short memory)
        {
            //TODO, gerer PanTiltWorking (comment s'arrête ?)
            throw new NotImplementedException();
            //m_CamParser.CameraMemoryRecall(memory);
            //PanTileWorking = true;
        }


        public void Tally(bool ledRed, bool ledGreen)
        {
            if (m_CamParser != null)
                m_CamParser.Tally(ledRed, ledGreen);
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
