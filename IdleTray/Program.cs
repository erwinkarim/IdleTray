using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IdleTray
{
    static class Program
    {

        public static IdleTrayAppContext idleTrayAppContext;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            idleTrayAppContext = new IdleTrayAppContext();
            //Application.Run(new ConfigForm());
            Application.Run(idleTrayAppContext);
        }
    }
}
