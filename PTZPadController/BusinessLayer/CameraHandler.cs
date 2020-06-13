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

        public string CameraName { get; set; }

        public bool PanTileWorking { get; private set; }

        public bool ZoomWorking { get; private set; }

        public CameraHandler()
        {
        }

        public void Initialize(ICameraParser camParser, string name)
        {
            m_CamParser = camParser;
            CameraName = name;
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
        public void ZoomStop()
        {
            m_CamParser.ZoomStop();
            ZoomWorking = false;
        }


        public void PanTiltHome()
        {
            //TODO
        }
        public void CameraMemoryReset()
        {
            //TODO
        }

        public void CameraMemorySet()
        {
            //TODO
        }

        public void CameraMemoryRecall()
        {
            //TODO
        }
        public void CompletionMessage()
        {
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
