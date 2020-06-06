using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PTZPadController.Common
{
    public static class Icons
    {

        #region Variables
        private static IconSize m_IconConnectionLost;
        private static IconSize m_IconConnectionReady;
        private static IconSize m_IconAppManager;
        #endregion

        #region Const
        const string MyPath = "/Images/Icons/";
        //internal const string S16 = "16";
        internal const string S32 = "32";

        #endregion
        #region Methods
        internal static BitmapImage CreateBitmap(string myFile, string size)
        {
            var p = string.Format("pack://application:,,,/{0};Component{1}{2}/{3}", "PTZPadController", MyPath, size, myFile);
            //var a = new BitmapImage(new Uri())
            return new BitmapImage(new Uri(p));

        }

        #endregion
        #region Button Icons
        public static IconSize ConnectionLost
        {
            get { return m_IconConnectionLost ?? (m_IconConnectionLost = new IconSize("Link - 02.png")); }
        }
        public static IconSize ConnectionReady
        {
            get { return m_IconConnectionReady ?? (m_IconConnectionReady = new IconSize("Link - 01.png")); }
        }
        #endregion

        #region ApplicationIcon
        public static IconSize AppManager
        {
            get { return m_IconAppManager ?? (m_IconAppManager = new IconSize("Remote Control-WF.png")); }
        }
        #endregion
    }

    public class IconSize
    {
        #region Variables
        private readonly string m_IconName;
        private BitmapImage m_S16Bmp;
        private BitmapImage m_S32Bmp;

        #endregion

        public IconSize(string iconName)
        {
            m_IconName = iconName;
        }

        public string IconName { get { return m_IconName; } }


        public BitmapImage S32
        {
            get { return m_S32Bmp ?? (m_S32Bmp = Icons.CreateBitmap(m_IconName, Icons.S32)); }
        }


    }

}
