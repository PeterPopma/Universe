using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Universe.Forms
{
    public partial class FormMain : Form
    {
        private static System.Windows.Forms.Timer updateScreenTimer;

        public FormMain()
        {
            InitializeComponent();
            SetupTimers();
        }

        private void SetupTimers()
        {
            // Create a timer with a 10 msec interval.
            updateScreenTimer = new System.Windows.Forms.Timer();
            updateScreenTimer.Interval = 10;
            updateScreenTimer.Tick += new EventHandler(OnTimedEventUpdateScreen);
            updateScreenTimer.Start();
        }

        private void OnTimedEventUpdateScreen(object sender, EventArgs eArgs)
        {
            displayMonogame.UpdateFrame();
            displayMonogame.UpdateScreen();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the timer
            updateScreenTimer.Enabled = false;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            //this.FormBorderStyle = FormBorderStyle.None;
            //this.WindowState = FormWindowState.Maximized;
            //this.Height = Screen.PrimaryScreen.WorkingArea.Height;
            //this.Width = this.Height;
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
        }

        private void displayMonogame_MouseDown(object sender, MouseEventArgs e)
        {
            displayMonogame.OnMouseDown(e.X, e.Y);
        }

        private void displayMonogame_MouseUp(object sender, MouseEventArgs e)
        {
            displayMonogame.IsMouseDown = false;
        }
    }
}
