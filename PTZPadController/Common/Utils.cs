using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace PTZPadController.Common
{
    class Utils
    {
    }

    #region Timer for delayed task

    public static class At
    {

        class localTimer
        {
            public Timer Me { get; set; }
            public Action MyAction { get; set; }
        }
        #region Members

        /// <summary>
        /// Specifies the method that will be fired to execute the delayed anonymous method.
        /// </summary>
        private readonly static TimerCallback timerCallBack = ExecuteDelayedAction;

        #endregion

        #region Methods

        /// <summary>
        /// Method that executes an anonymous method after a delay period.
        /// </summary>
        /// <param name="action">The anonymous method that needs to be executed.</param>
        /// <param name="delay">The period of delay to wait before executing.</param>
        /// <param name="interval">The period (in milliseconds) to delay before executing the anonymous method again (Timeout.Infinite to disable).</param>
        public static Timer Do(Action action, TimeSpan delay, int interval = Timeout.Infinite)
        {
            // create a new thread timer to execute the method after the delay
            int iDelay = Convert.ToInt32(delay.TotalMilliseconds);
            var t = new localTimer { MyAction = action };
            t.Me = new Timer(timerCallBack, t, iDelay, interval);
            return t.Me;
        }

        /// <summary>
        /// Method that executes an anonymous method after a delay period.
        /// </summary>
        /// <param name="action">The anonymous method that needs to be executed.</param>
        /// <param name="delay">The period of delay (in milliseconds) to wait before executing.</param>
        /// <param name="interval">The period (in milliseconds) to delay before executing the anonymous method again (Timeout.Infinite to disable).</param>
        public static Timer Do(Action action, int delay, int interval = Timeout.Infinite)
        {
            return Do(action, TimeSpan.FromMilliseconds(delay), interval);


        }

        /// <summary>
        /// Method that executes an anonymous method after a delay period.
        /// </summary>
        /// <param name="action">The anonymous method that needs to be executed.</param>
        /// <param name="dueTime">The due time when this method needs to be executed.</param>
        /// <param name="interval">The period (in milliseconds) to delay before executing the anonymous method again (Timeout.Infinite to disable).</param>
        public static Timer Do(Action action, DateTime dueTime, int interval = Timeout.Infinite)
        {
            if (dueTime < DateTime.Now)
            {
                throw new ArgumentOutOfRangeException("dueTime", "The specified due time has already elapsed.");
            }

            return Do(action, dueTime - DateTime.Now, interval);


        }

        /// <summary>
        /// Method that executes a delayed action after a specific interval.
        /// </summary>
        /// <param name="o">The Action delegate that is to be executed.</param>
        /// <remarks>This method is invoked on its own thread.</remarks>
        private static void ExecuteDelayedAction(object o)
        {
            // invoke the anonymous method
            var t = o as localTimer;
            if (t == null)
                return;
            t.Me.Dispose();
            t.MyAction.Invoke();
        }

        #endregion
    }
    #endregion

}
