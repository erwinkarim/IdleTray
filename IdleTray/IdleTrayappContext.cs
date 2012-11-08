using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;

namespace IdleTray
{
    class IdleTrayAppContext : ApplicationContext
    {
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        NotifyIcon notifyIcon = new NotifyIcon();
        ConfigForm cform = new ConfigForm();

        public IdleTrayAppContext()
        {
            //build the menu
            MenuItem configMenuItem = new MenuItem("Configure server...", new EventHandler(ShowConfig));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

            //new timer
            Timer idleTimer = new Timer();
            idleTimer.Tick += new EventHandler(idleTimer_Tick);
            idleTimer.Interval = 1000 * 10; //every 10 sec
            idleTimer.Start();

            //new nicon
            notifyIcon.Icon = IdleTray.Properties.Resources.Icon1;
            notifyIcon.DoubleClick += new EventHandler(ShowConfig);
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { configMenuItem, exitMenuItem });
            notifyIcon.Visible = true;
        }

        void ShowConfig(object sender, EventArgs s)
        {
            if (cform.Visible)
                cform.Focus();
            else
                cform.ShowDialog();
        }

        void Exit(Object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }

        void idleTimer_Tick(object sender, EventArgs e)
        {
            //get system uptime
            int systemUptime = Environment.TickCount;
            //get user input time
            int LastInputTicks = 0;
            //get idle time
            int idleTime = 0;

            
            LASTINPUTINFO LastInputInfo = new LASTINPUTINFO();
            LastInputInfo.cbSize = (uint)Marshal.SizeOf(LastInputInfo);
            LastInputInfo.dwTime = 0;

            if (GetLastInputInfo(ref LastInputInfo))
            {
                LastInputTicks = (int)LastInputInfo.dwTime;
                idleTime = systemUptime - LastInputTicks;
            }

            //send the message to the server
            string sURL = "http://" + IdleTray.Properties.Settings.Default.FireworkServer + "/idle_user/report?user=" +
                Environment.UserName + "&host=" + Environment.MachineName + "&idle=" + idleTime.ToString();
            WebRequest webRequest = WebRequest.Create(sURL);
            Stream objStream = webRequest.GetResponse().GetResponseStream();
            objStream.Close();

            //MessageBox.Show(Environment.UserName + "@" + Environment.MachineName + ":" + idleTime.ToString());
        }
    }
}
