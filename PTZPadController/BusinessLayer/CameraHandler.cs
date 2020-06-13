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

        public void PanTiltUp()
        {
        }

        public void PanTiltDown()
        {

        }

        public void PanTiltLeft()
        {

        }

        public void PanTiltRight()
        {

        }

        public void PanTiltUpLeft()
        {

        }

        public void PanTiltUpRight()
        {

        }

        public void PanTiltDownLeft()
        {

        }

        public void PanTiltDownRight()
        {

        }

        public void PanTiltStop()
        {

        }

        public void PanTiltHome()
        {

        }
        public void CameraMemoryReset()
        {

        }

        public void CameraMemorySet()
        {

        }

        public void CameraMemoryRecall()
        {

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
