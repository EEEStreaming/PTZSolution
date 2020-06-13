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

        public void PanTiltUp()
        {
            PanTileWorking = true;
        }

        public void PanTiltDown()
        {
            PanTileWorking = true;

        }

        public void PanTiltLeft()
        {
            PanTileWorking = true;

        }

        public void PanTiltRight()
        {
            PanTileWorking = true;

        }

        public void PanTiltUpLeft()
        {
            PanTileWorking = true;

        }

        public void PanTiltUpRight()
        {
            PanTileWorking = true;

        }

        public void PanTiltDownLeft()
        {
            PanTileWorking = true;

        }

        public void PanTiltDownRight()
        {
            PanTileWorking = true;

        }

        public void PanTiltStop()
        {
            PanTileWorking = false;

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

        public void StopPanTile()
        {
            throw new NotImplementedException();
        }
    }
}
