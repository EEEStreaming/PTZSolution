using PTZPadController.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public CameraHandler()
        {
        }

        public void Initialize(ICameraParser camParser)
        {
            m_CamParser = camParser;
        }

        public void PanTiltUp(short panSpeed, short tiltSpeed)
        {
            m_CamParser.PanTiltUp(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltDown(short panSpeed, short tiltSpeed)
        {
            m_CamParser.PanTiltDown(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltLeft(short panSpeed, short tiltSpeed)
        {
            m_CamParser.PanTiltLeft(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltRight(short panSpeed, short tiltSpeed)
        {
            m_CamParser.PanTiltRight(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltUpLeft(short panSpeed, short tiltSpeed)
        {
            m_CamParser.PanTiltUpLeft(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltUpRight(short panSpeed, short tiltSpeed)
        {
            m_CamParser.PanTiltUpRight(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltDownLeft(short panSpeed, short tiltSpeed)
        {
            m_CamParser.PanTiltDownLeft(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltDownRight(short panSpeed, short tiltSpeed)
        {
            m_CamParser.PanTiltDownRight(panSpeed, tiltSpeed);
            PanTileWorking = true;
        }

        public void PanTiltStop()
        {
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
            m_CamParser.ZoomStop();
            ZoomWorking = false;
        }

        public void ZoomTele()
        {
            m_CamParser.ZoomTele();
            ZoomWorking = true;
        }

        public void ZoomWide()
        {
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
        public void CompletionMessage()
        {
            //TODO ????
            PTZLogger.Log.Debug("message OK");
        }

        public void Tally(bool ledRed, bool ledGreen)
        {
            m_CamParser.Tally(ledRed, ledGreen);
        }

        public void Connect()
        {
            m_CamParser.Connect();
        }

        public void StopPanTile()
        {
            throw new NotImplementedException();
        }
    }
}
