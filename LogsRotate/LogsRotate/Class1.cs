using System;
using System.IO;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using System.Drawing;
using System.Xml.Serialization;

namespace LogsRotate
{
    public class PluginSettings
    {
        public int DaysToKeep { get; set; }
        public string LogfilePath { get; set; }
        public bool AutoRunOnLaunch { get; set; }
    }

    public class LogRotatorPlugin : IActPluginV1
    {
        // UI Elements
        private Label lblStatus;
        private Label lblLogFilePath;
        private TextBox txtDaysToKeep;
        private TextBox txtLogFilePath;
        private Button btnSave;
        private Button btnRun;
        private LinkLabel lnkGitHub;
        private CheckBox chkAutoRunOnLaunch;
        private Label lblLastRun;
        private Label lblLogsDeleted;

        private PluginSettings settings;
        private string settingsFilePath;

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            lblStatus = pluginStatusText;

            // Initialize UI elements
            txtDaysToKeep = new TextBox();
            txtLogFilePath = new TextBox();
            chkAutoRunOnLaunch = new CheckBox();

            // Resolve the %AppData% path and set the settings file path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string actPluginPath = Path.Combine(appDataPath, "Advanced Combat Tracker", "Plugins");
            settingsFilePath = Path.Combine(actPluginPath, "LogsRotateSettings.xml");

            // Load settings
            LoadSettings();

            // Update UI elements with loaded settings
            txtDaysToKeep.Text = settings.DaysToKeep.ToString();
            txtLogFilePath.Text = settings.LogfilePath;
            chkAutoRunOnLaunch.Checked = settings.AutoRunOnLaunch;

            // Create UI elements for Days to Keep
            Label lblDaysToKeep = new Label
            {
                Text = "Days to Keep:",
                Location = new Point(10, 10),
                AutoSize = true
            };

            txtDaysToKeep.Location = new Point(100, 10);

            // Create UI elements for Log File Path
            lblLogFilePath = new Label
            {
                Text = "Log File Path:",
                Location = new Point(10, 40),
                AutoSize = true
            };

            txtLogFilePath.Location = new Point(100, 40);

            // Create CheckBox for Auto Run on Launch
            chkAutoRunOnLaunch.Text = "Auto Run on Launch";
            chkAutoRunOnLaunch.Location = new Point(10, 100);
            chkAutoRunOnLaunch.AutoSize = true;

            // Create Save button
            btnSave = new Button
            {
                Text = "Save",
                Location = new Point(100, 70),
                Width = 70
            };
            btnSave.Click += BtnSave_Click;

            // Create Run button
            btnRun = new Button
            {
                Text = "Run",
                Location = new Point(180, 70),
                Width = 70
            };
            btnRun.Click += BtnRun_Click;

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
            pluginScreenSpace.Controls.Add(btnSave);
            pluginScreenSpace.Controls.Add(btnRun);
            pluginScreenSpace.Controls.Add(lnkGitHub);
            pluginScreenSpace.Controls.Add(chkAutoRunOnLaunch);
            pluginScreenSpace.Controls.Add(lblLastRun);
            pluginScreenSpace.Controls.Add(lblLogsDeleted);

            lblStatus.Text = "Plugin Initialized.";

            // Automatically run log rotation if the plugin is enabled
            if (settings.AutoRunOnLaunch)
            {
                BtnRun_Click(null, null);
            }
        }

        private void LoadSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PluginSettings));
                using (StreamReader reader = new StreamReader(settingsFilePath))
                {
                    settings = (PluginSettings)serializer.Deserialize(reader);
                }
            }
            else
            {
                settings = new PluginSettings
                {
                    DaysToKeep = 365,
                    LogfilePath = Path.GetDirectoryName(ActGlobals.oFormActMain.LogFilePath),
                    AutoRunOnLaunch = false
                };
                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PluginSettings));
            using (StreamWriter writer = new StreamWriter(settingsFilePath))
            {
                serializer.Serialize(writer, settings);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Save settings
            settings.DaysToKeep = int.Parse(txtDaysToKeep.Text);
            settings.LogfilePath = txtLogFilePath.Text;
            settings.AutoRunOnLaunch = chkAutoRunOnLaunch.Checked;

            SaveSettings();
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            // Apply settings and run
            int deletedCount = RotateLogs(int.Parse(txtDaysToKeep.Text), txtLogFilePath.Text);

            lblLastRun.Text = $"Last Run: {DateTime.Now}";
            lblLogsDeleted.Text = $"Logs Deleted: {deletedCount}";
        }

        private void LnkGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Brappp/ACT-Logs-Rotator");
        }

        public void DeInitPlugin()
        {
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
