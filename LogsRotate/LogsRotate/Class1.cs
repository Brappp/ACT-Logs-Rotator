using System;
using System.IO;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using System.Drawing;

namespace LogsRotate 
{
    public class LogRotatorPlugin : IActPluginV1
    {
        // UI Elements
        private Label lblStatus;
        private Label lblLogFilePath;
        private TextBox txtDaysToKeep;
        private TextBox txtLogFilePath;
        private Button btnApply;
        private Timer autoRunTimer;
        private LinkLabel lnkGitHub;
        private CheckBox chkRunOnce;
        private Label lblLastRun;
        private Label lblLogsDeleted;
        private CheckBox chkEnabled; // Checkbox to enable or disable the plugin

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            lblStatus = pluginStatusText;

            // Initialize UI elements before accessing their properties
            txtDaysToKeep = new TextBox();
            txtLogFilePath = new TextBox();
            chkRunOnce = new CheckBox();
            chkEnabled = new CheckBox(); // Initialize the enable checkbox

            // Initialize and configure the timer for auto-run
            autoRunTimer = new Timer();
            autoRunTimer.Interval = 60000;  // 60,000 milliseconds = 1 minute
            autoRunTimer.Tick += AutoRunTimer_Tick;

            // Load settings
            txtDaysToKeep.Text = LogsRotate.Settings.Default.DaysToKeep.ToString();
            txtLogFilePath.Text = Path.GetDirectoryName(ActGlobals.oFormActMain.LogFilePath);
            chkRunOnce.Checked = LogsRotate.Settings.Default.RunOnce;
            chkEnabled.Checked = LogsRotate.Settings.Default.PluginEnabled; // Load the state of the checkbox

            // Create UI elements for Days to Keep
            Label lblDaysToKeep = new Label
            {
                Text = "Days to Keep:",
                Location = new Point(10, 10),
                AutoSize = true
            };

            txtDaysToKeep.Location = new Point(100, 10);
            txtDaysToKeep.Text = "5";

            // Create UI elements for Log File Path
            lblLogFilePath = new Label
            {
                Text = "Log File Path:",
                Location = new Point(10, 40),
                AutoSize = true
            };

            txtLogFilePath.Location = new Point(100, 40);

            // Create CheckBox for Run Once
            chkRunOnce.Text = "Run Once at ACT Launch";
            chkRunOnce.Location = new Point(10, 100);
            chkRunOnce.AutoSize = true;
            chkRunOnce.CheckedChanged += ChkRunOnce_CheckedChanged;

            // Create UI elements for Enable/Disable
            chkEnabled.Text = "Enable Plugin";
            chkEnabled.Location = new Point(10, 130); 
            chkEnabled.AutoSize = true;
            chkEnabled.CheckedChanged += ChkEnabled_CheckedChanged;

            // Create Apply button
            btnApply = new Button
            {
                Text = "Save and Run",
                Location = new Point(100, 70),
                Width = 150
            };
            btnApply.Click += BtnApply_Click;

            // Create LinkLabel for GitHub
            lnkGitHub = new LinkLabel
            {
                Text = "GitHub Repository",
                Location = new Point(10, 160), 
                AutoSize = true
            };
            lnkGitHub.LinkClicked += LnkGitHub_LinkClicked;

            // Create Labels for Last Run and Logs Deleted
            lblLastRun = new Label
            {
                Text = "Last Run: Never",
                Location = new Point(10, 190),
                AutoSize = true
            };

            lblLogsDeleted = new Label
            {
                Text = "Logs Deleted: 0",
                Location = new Point(10, 210),
                AutoSize = true
            };

            // Add UI elements to plugin tab
            pluginScreenSpace.Controls.Add(lblDaysToKeep);
            pluginScreenSpace.Controls.Add(txtDaysToKeep);
            pluginScreenSpace.Controls.Add(lblLogFilePath);
            pluginScreenSpace.Controls.Add(txtLogFilePath);
            pluginScreenSpace.Controls.Add(btnApply);
            pluginScreenSpace.Controls.Add(lnkGitHub);
            pluginScreenSpace.Controls.Add(chkRunOnce);
            pluginScreenSpace.Controls.Add(chkEnabled); 
            pluginScreenSpace.Controls.Add(lblLastRun);
            pluginScreenSpace.Controls.Add(lblLogsDeleted);

            lblStatus.Text = "Plugin Initialized.";

            // Run once at ACT launch if the checkbox is checked
            if (chkRunOnce.Checked)
            {
                BtnApply_Click(null, null);
            }
        }

        private void ChkEnabled_CheckedChanged(object sender, EventArgs e)
        {
            // Start or stop the timer based on the checkbox state
            if (chkEnabled.Checked)
            {
                autoRunTimer.Start();
            }
            else
            {
                autoRunTimer.Stop();
            }

            // Save the state of the checkbox
            LogsRotate.Settings.Default.PluginEnabled = chkEnabled.Checked;
            LogsRotate.Settings.Default.Save();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            try
            {
                // Apply settings
                int deletedCount = RotateLogs(int.Parse(txtDaysToKeep.Text), txtLogFilePath.Text);

                lblLastRun.Text = $"Last Run: {DateTime.Now}";
                lblLogsDeleted.Text = $"Logs Deleted: {deletedCount}";

                if (chkRunOnce.Checked)
                {
                    autoRunTimer.Stop();
                }

                // Save settings
                LogsRotate.Settings.Default.DaysToKeep = int.Parse(txtDaysToKeep.Text);
                LogsRotate.Settings.Default.LogfilePath = txtLogFilePath.Text;
                LogsRotate.Settings.Default.RunOnce = chkRunOnce.Checked;
                LogsRotate.Settings.Default.Save();
            }
            catch (FormatException ex)
            {
                lblStatus.Text = "Error: Invalid input format. " + ex.Message;
            }
        }

        private void AutoRunTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                int deletedCount = RotateLogs(int.Parse(txtDaysToKeep.Text), txtLogFilePath.Text);

                lblLastRun.Text = $"Last Run: {DateTime.Now}";
                lblLogsDeleted.Text = $"Logs Deleted: {deletedCount}";

                if (chkRunOnce.Checked)
                {
                    autoRunTimer.Stop();
                }
            }
            catch (FormatException ex)
            {
                lblStatus.Text = "Error: Invalid input format. " + ex.Message;
            }
        }

        private void LnkGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Brappp/ACT-Logs-Rotator");
        }

        private void ChkRunOnce_CheckedChanged(object sender, EventArgs e)
        {
            // placeholder
        }

        public void DeInitPlugin()
        {
            // Save the state of the checkbox
            LogsRotate.Settings.Default.PluginEnabled = chkEnabled.Checked;
            LogsRotate.Settings.Default.Save();

            lblStatus.Text = "Plugin Unloaded.";
        }

        private int RotateLogs(int daysToKeep, string folderPath)
        {
            int deletedCount = 0;
            try
            {
                DateTime currentDate = DateTime.Now;
                DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
                FileInfo[] logFiles = dirInfo.GetFiles("*.log");

                foreach (FileInfo file in logFiles)
                {
                    TimeSpan age = currentDate - file.CreationTime;

                    if (age.TotalDays > daysToKeep)
                    {
                        file.Delete();
                        deletedCount++;
                    }
                }

                lblStatus.Text = $"{deletedCount} log files deleted.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
            }
            return deletedCount;
        }
    }
}
