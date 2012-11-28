using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace IdleTray
{
    static class Program
    {

        public static IdleTrayAppContext idleTrayAppContext;
        static String _mutexID = "8KeSLUeOEEIamgDf-DEvMzpXvPRVyfQoi";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Boolean _isNotRunning;
            using (Mutex _mutex = new Mutex(true, _mutexID, out _isNotRunning))
            {
                if (_isNotRunning)
                {
                    //single instance of the app is running
                    idleTrayAppContext = new IdleTrayAppContext();
                    Application.Run(idleTrayAppContext);
                }
                else
                {
                    MessageBox.Show("An instance of Idle Client is already running");
                    return;
                }
            }
            
        }
    }
}
