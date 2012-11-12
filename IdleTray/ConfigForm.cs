using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IdleTray
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = IdleTray.Properties.Settings.Default.FireworkServer;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IdleTray.Properties.Settings.Default.FireworkServer = textBox1.Text;
            IdleTray.Properties.Settings.Default.Save();


            Close();
        }


        void textBox1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                button1_Click(sender, e);
            }         
        }

    }
}
