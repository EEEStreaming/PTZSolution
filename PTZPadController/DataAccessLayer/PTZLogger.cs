using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTZPadController.DataAccessLayer
{
    public static class PTZLogger
    {
        public static NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        internal static void Shutdown()
        {
            NLog.LogManager.Shutdown(); // Flush and close down internal threads and timers
        }
    }
}
