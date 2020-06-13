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
        }

        public void PanTiltDown(short panSpeed, short tiltSpeed)
        {
            m_CamParser.PanTiltDown(panSpeed, tiltSpeed);
        }

        public void PanTiltLeft(short panSpeed, short tiltSpeed)
        {
            m_CamParser.PanTiltLeft(panSpeed, tiltSpeed);
        }

        public void PanTiltRight(short panSpeed, short tiltSpeed)
        {
            m_CamParser.PanTiltRight(panSpeed, tiltSpeed);
        }

        public void PanTiltUpLeft(short panSpeed, short tiltSpeed)
        {
            m_CamParser.PanTiltUpLeft(panSpeed, tiltSpeed);
        }

        public void PanTiltUpRight(short panSpeed, short tiltSpeed)
        {
            m_CamParser.PanTiltUpRight(panSpeed, tiltSpeed);
        }

        public void PanTiltDownLeft(short panSpeed, short tiltSpeed)
        {
            m_CamParser.PanTiltDownLeft(panSpeed, tiltSpeed);
        }

        public void PanTiltDownRight(short panSpeed, short tiltSpeed)
        {
            m_CamParser.PanTiltDownRight(panSpeed, tiltSpeed);
        }

        public void PanTiltStop()
        {
            m_CamParser.PanTiltStop();
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
    }
}
