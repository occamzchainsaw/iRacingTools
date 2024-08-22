using iMonitor;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace iLauncher
{
    public partial class LauncherForm : Form
    {
        private IRacingWatcher iRacingWatcher;
        public LauncherForm()
        {
            InitializeComponent();
            double interval = Properties.Settings.Default.iRacingCheckInterval;
            iRacingWatcher = new IRacingWatcher(interval);
            iRacingWatcher.IRacingStarted += iRacingStarted;
            iRacingWatcher.IRacingStopped += iRacingStopped;
            iRacingWatcher.StartMonitoring();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            lblStatus.Text = iRacingWatcher.IsStarted ? "running" : "not running";
            lblStatus.ForeColor = iRacingWatcher.IsStarted ? Color.ForestGreen : Color.DarkRed;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            iRacingWatcher.StopMonitoring();
        }

        private void iRacingStarted(object sender, EventArgs e)
        {
            // todo
            // check a list of programs to launch
            // launch them motherfuckers if they need to be launched with iRacing and they aren't already running
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    iRacingStarted(sender, e);
                }));
            }
            else
            {
                lblStatus.Text = "running";
                lblStatus.ForeColor = Color.ForestGreen;
            }
        }

        private void iRacingStopped(object sender, EventArgs e) 
        {
            // todo
            // check list of programs with their options
            // if mofo needs to shut down with iRacing, shut it down
            // if mofo needs to start when iRacing disconnects, launch it
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    iRacingStopped(sender, e);
                }));
            }
            else
            {
                lblStatus.Text = "not running";
                lblStatus.ForeColor = Color.DarkRed;
            }
        }
    }
}
