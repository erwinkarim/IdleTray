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

        public NotifyIcon notifyIcon = new NotifyIcon();
        ConfigForm cform = new ConfigForm();
        HttpWebRequest webRequest;

        public IdleTrayAppContext()
        {
            
            //build the menu
            MenuItem configMenuItem = new MenuItem("Configure server...", new EventHandler(ShowConfig));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

            //new timer
            Timer idleTimer = new Timer();
            idleTimer.Tick += new EventHandler(idleTimer_Tick);
            idleTimer.Interval = 1000 * 120; //2 minutes;
            idleTimer.Start();

            //new icon
            notifyIcon.Icon = IdleTray.Properties.Resources.Apps_nepomuk;
            notifyIcon.DoubleClick += new EventHandler(ShowConfig);
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { configMenuItem, new MenuItem("-"), exitMenuItem });
            notifyIcon.Visible = true;
            notifyIcon.Text = IdleTray.Properties.Settings.Default.FireworkServer;
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
            //update icon tray text
            notifyIcon.Text = IdleTray.Properties.Settings.Default.FireworkServer;

            //setup
            int systemUptime = Environment.TickCount;
            int LastInputTicks = 0;
            int idleTime = 0;

            //get last user keyboard/mouse event
            LASTINPUTINFO LastInputInfo = new LASTINPUTINFO();
            LastInputInfo.cbSize = (uint)Marshal.SizeOf(LastInputInfo);
            LastInputInfo.dwTime = 0;

            //get idle time
            if (GetLastInputInfo(ref LastInputInfo))
            {
                LastInputTicks = (int)LastInputInfo.dwTime;
                idleTime = systemUptime - LastInputTicks;
            }

            //send the message to the server
            string sURL = "http://" + IdleTray.Properties.Settings.Default.FireworkServer + "/idle_user/report?user=" +
                Environment.UserName + "&host=" + Environment.MachineName + "&idle=" + idleTime.ToString();
            webRequest = (HttpWebRequest)WebRequest.Create(sURL);

            HttpWebResponse webResponse;

            try
            {
                webResponse = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (WebException we)
            {
                notifyIcon.Text = "Error: " + ((HttpWebResponse)we.Response).StatusCode;
            }
            //webResponse.Close();
            
            
        }

        public void setIconText(String text)
        {
            notifyIcon.Text = text;
        }

    }
}
