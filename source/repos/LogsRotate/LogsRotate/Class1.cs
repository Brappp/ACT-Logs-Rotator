using System;
using System.IO;
using System.Windows.Forms;
using Advanced_Combat_Tracker;

namespace LogRotatorPlugin
{
    public class LogRotatorPlugin : IActPluginV1
    {
        // UI Elements
        private Label lblStatus;
        private TextBox txtDaysToKeep;
        private TextBox txtLogFilePath;
        private TextBox txtInterval;  // TextBox for interval
        private Button btnApply;
        private Timer autoRunTimer;  // Timer for auto-run

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            lblStatus = pluginStatusText;

            // Initialize and configure the timer for auto-run
            autoRunTimer = new Timer();
            autoRunTimer.Interval = 60000;  // 60,000 milliseconds = 1 minute
            autoRunTimer.Tick += AutoRunTimer_Tick;
            autoRunTimer.Start();

            // Create UI elements for Days to Keep
            Label lblDaysToKeep = new Label
            {
                Text = "Days to Keep:",
                Location = new System.Drawing.Point(10, 10),
                AutoSize = true
            };

            txtDaysToKeep = new TextBox
            {
                Location = new System.Drawing.Point(100, 10),
                Text = "5"
            };

            // Create UI elements for Log File Path
            Label lblLogFilePath = new Label
            {
                Text = "Log File Path:",
                Location = new System.Drawing.Point(10, 40),
                AutoSize = true
            };

            string folderPath = Path.GetDirectoryName(ActGlobals.oFormActMain.LogFilePath);

            txtLogFilePath = new TextBox
            {
                Location = new System.Drawing.Point(100, 40),
                Text = folderPath  // Populate with ACT's log folder path
            };

            // Create UI elements for Interval
            Label lblInterval = new Label
            {
                Text = "Interval (min):",
                Location = new System.Drawing.Point(10, 70),
                AutoSize = true
            };

            txtInterval = new TextBox
            {
                Location = new System.Drawing.Point(100, 70),
                Text = "1"
            };

            // Create Apply button
            btnApply = new Button
            {
                Text = "Apply",
                Location = new System.Drawing.Point(100, 100)
            };
            btnApply.Click += BtnApply_Click;

            // Add UI elements to plugin tab
            pluginScreenSpace.Controls.Add(lblDaysToKeep);
            pluginScreenSpace.Controls.Add(txtDaysToKeep);
            pluginScreenSpace.Controls.Add(lblLogFilePath);
            pluginScreenSpace.Controls.Add(txtLogFilePath);
            pluginScreenSpace.Controls.Add(lblInterval);
            pluginScreenSpace.Controls.Add(txtInterval);
            pluginScreenSpace.Controls.Add(btnApply);

            lblStatus.Text = "Plugin Initialized.";
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            // Update timer interval based on user input
            int interval = int.Parse(txtInterval.Text) * 60000;  // Convert minutes to milliseconds
            autoRunTimer.Interval = interval;

            // Run log rotation
            RotateLogs(int.Parse(txtDaysToKeep.Text), txtLogFilePath.Text);
        }

        private void AutoRunTimer_Tick(object sender, EventArgs e)
        {
            // Automatically run log rotation
            RotateLogs(int.Parse(txtDaysToKeep.Text), txtLogFilePath.Text);
        }

        public void DeInitPlugin()
        {
            lblStatus.Text = "Plugin Unloaded.";
        }

        private void RotateLogs(int daysToKeep, string folderPath)
        {
            try
            {
                DateTime currentDate = DateTime.Now;
                DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
                FileInfo[] logFiles = dirInfo.GetFiles("*.log");

                int deletedCount = 0;

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
        }
    }
}
