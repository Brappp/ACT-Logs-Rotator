using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks; // Added for async/await
using Advanced_Combat_Tracker;

namespace LogsRotate
{
    public class LogRotatorPlugin : IActPluginV1
    {
        private TabPage pluginTab;
        private Label lblStatus;
        private TextBox txtDaysToKeep, txtLogFilePath;
        private Button btnSave, btnRun;
        private ProgressBar progressBar;
        private CheckBox chkAutoRunOnLaunch;
        private LinkLabel lnkGitHub;
        private Label lblLastRun, lblLogsDeleted;
        private ListBox lstAllLogs, lstProtectedLogs;
        private PluginSettings settings;
        private string settingsFilePath;

        public async void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            this.pluginTab = pluginScreenSpace;
            this.lblStatus = pluginStatusText;
            LoadSettings();
            InitializeUIComponents();
            UpdateUIFromSettings();

            // Auto run log rotation if enabled
            if (settings.AutoRunOnLaunch)
            {
                RunLogRotation();
            }

            // Check for updates
            await UpdateChecker.CheckForUpdates();
        }

        private void InitializeUIComponents()
        {
            pluginTab.Controls.Clear();

            // Main layout panel
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                AutoSize = true
            };

            // GroupBox for settings
            GroupBox settingsGroup = new GroupBox
            {
                Text = "Settings",
                Dock = DockStyle.Top,
                Padding = new Padding(10),
                AutoSize = true
            };

            TableLayoutPanel settingsLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 4,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            settingsLayout.Controls.Add(new Label { Text = "Days to Keep:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
            txtDaysToKeep = new TextBox { Width = 100, Anchor = AnchorStyles.Left };
            settingsLayout.Controls.Add(txtDaysToKeep, 1, 0);

            settingsLayout.Controls.Add(new Label { Text = "Log File Path:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 1);
            txtLogFilePath = new TextBox { Width = 300, Anchor = AnchorStyles.Left };
            settingsLayout.Controls.Add(txtLogFilePath, 1, 1);

            settingsLayout.Controls.Add(new Label { Text = "Auto Run on Launch:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 2);
            chkAutoRunOnLaunch = new CheckBox { Anchor = AnchorStyles.Left };
            settingsLayout.Controls.Add(chkAutoRunOnLaunch, 1, 2);

            btnSave = new Button { Text = "Save", Anchor = AnchorStyles.Right };
            btnSave.Click += BtnSave_Click;
            settingsLayout.Controls.Add(btnSave, 1, 3);

            settingsGroup.Controls.Add(settingsLayout);
            mainLayout.Controls.Add(settingsGroup, 0, 0);
            mainLayout.SetColumnSpan(settingsGroup, 2);

            // GroupBox for actions
            GroupBox actionsGroup = new GroupBox
            {
                Text = "Actions",
                Dock = DockStyle.Top,
                Padding = new Padding(10),
                AutoSize = true
            };

            TableLayoutPanel actionsLayout = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 2,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            btnRun = new Button { Text = "Run", Anchor = AnchorStyles.Right };
            btnRun.Click += BtnRun_Click;
            actionsLayout.Controls.Add(btnRun, 0, 0);

            progressBar = new ProgressBar { Dock = DockStyle.Fill };
            actionsLayout.Controls.Add(progressBar, 1, 0);

            actionsGroup.Controls.Add(actionsLayout);
            mainLayout.Controls.Add(actionsGroup, 0, 1);
            mainLayout.SetColumnSpan(actionsGroup, 2);

            // GroupBox for logs
            GroupBox logsGroup = new GroupBox
            {
                Text = "Logs",
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                AutoSize = true
            };

            TableLayoutPanel logsLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 2,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            // Set the column width percentages to be equal
            logsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            logsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            logsLayout.Controls.Add(new Label { Text = "All Logs:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
            lstAllLogs = new ListBox { Dock = DockStyle.Fill, Height = 200 };
            lstAllLogs.DoubleClick += LstAllLogs_DoubleClick;
            logsLayout.Controls.Add(lstAllLogs, 0, 1);

            logsLayout.Controls.Add(new Label { Text = "Protected Logs:", Anchor = AnchorStyles.Left, AutoSize = true }, 1, 0);
            lstProtectedLogs = new ListBox { Dock = DockStyle.Fill, Height = 200 };
            lstProtectedLogs.DoubleClick += LstProtectedLogs_DoubleClick;
            logsLayout.Controls.Add(lstProtectedLogs, 1, 1);

            logsGroup.Controls.Add(logsLayout);
            mainLayout.Controls.Add(logsGroup, 0, 2);
            mainLayout.SetColumnSpan(logsGroup, 2);

            // Status labels
            lblLastRun = new Label { Text = "Last Run: Never", Anchor = AnchorStyles.Left, AutoSize = true };
            lblLogsDeleted = new Label { Text = "Logs Deleted: 0", Anchor = AnchorStyles.Left, AutoSize = true };
            mainLayout.Controls.Add(lblLastRun, 0, 3);
            mainLayout.Controls.Add(lblLogsDeleted, 1, 3);

            // GitHub link
            lnkGitHub = new LinkLabel { Text = "https://github.com/Brappp/ACT-Logs-Rotator", AutoSize = true };
            lnkGitHub.LinkClicked += LnkGitHub_LinkClicked;
            mainLayout.Controls.Add(lnkGitHub, 0, 4);
            mainLayout.SetColumnSpan(lnkGitHub, 2);

            // Status label
            lblStatus = new Label { AutoSize = true, Dock = DockStyle.Bottom, TextAlign = ContentAlignment.MiddleLeft };
            mainLayout.Controls.Add(lblStatus, 0, 5);
            mainLayout.SetColumnSpan(lblStatus, 2);

            pluginTab.Controls.Add(mainLayout);
        }

        private void LstAllLogs_DoubleClick(object sender, EventArgs e)
        {
            if (lstAllLogs.SelectedItem != null)
            {
                string selectedLog = lstAllLogs.SelectedItem.ToString();
                settings.LockedLogs.Add(selectedLog);
                UpdateLogLists();
                SaveSettings();
            }
        }

        private void LstProtectedLogs_DoubleClick(object sender, EventArgs e)
        {
            if (lstProtectedLogs.SelectedItem != null)
            {
                string selectedLog = lstProtectedLogs.SelectedItem.ToString();
                settings.LockedLogs.Remove(selectedLog);
                UpdateLogLists();
                SaveSettings();
            }
        }

        private void UpdateLogLists()
        {
            lstAllLogs.Items.Clear();
            lstProtectedLogs.Items.Clear();

            DirectoryInfo dirInfo = new DirectoryInfo(settings.LogfilePath);
            FileInfo[] logFiles = dirInfo.GetFiles("*.log");

            foreach (FileInfo file in logFiles)
            {
                if (settings.LockedLogs.Contains(file.FullName))
                {
                    lstProtectedLogs.Items.Add(file.FullName);
                }
                else
                {
                    lstAllLogs.Items.Add(file.FullName);
                }
            }
        }

        private void LnkGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Brappp/ACT-Logs-Rotator");
        }

        private void UpdateUIFromSettings()
        {
            txtDaysToKeep.Text = settings.DaysToKeep.ToString();
            txtLogFilePath.Text = settings.LogfilePath;
            chkAutoRunOnLaunch.Checked = settings.AutoRunOnLaunch;
            lblStatus.Text = "Plugin Initialized.";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                settings.DaysToKeep = int.Parse(txtDaysToKeep.Text);
                settings.LogfilePath = txtLogFilePath.Text;
                settings.AutoRunOnLaunch = chkAutoRunOnLaunch.Checked;

                SaveSettings();
                lblStatus.Text = "Settings Saved.";
                UpdateLogLists();
            }
        }

        private bool ValidateInputs()
        {
            if (!int.TryParse(txtDaysToKeep.Text, out int days) || days <= 0)
            {
                MessageBox.Show("Please enter a valid number of days to keep logs.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrEmpty(txtLogFilePath.Text) || !Directory.Exists(txtLogFilePath.Text))
            {
                MessageBox.Show("Please enter a valid log file path.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            RunLogRotation();
        }

        private void RunLogRotation()
        {
            progressBar.Value = 0;
            int logsDeleted = RotateLogs();
            progressBar.Value = 100;

            lblLastRun.Text = $"Last Run: {DateTime.Now}";
            lblLogsDeleted.Text = $"Logs Deleted: {logsDeleted}";

            // Refresh the log lists after running
            UpdateLogLists();
        }

        private int RotateLogs()
        {
            int deletedCount = 0;
            DirectoryInfo dirInfo = new DirectoryInfo(settings.LogfilePath);
            FileInfo[] logFiles = dirInfo.GetFiles("*.log");

            foreach (FileInfo file in logFiles)
            {
                if (!settings.LockedLogs.Contains(file.FullName) && (DateTime.Now - file.CreationTime).TotalDays > settings.DaysToKeep)
                {
                    file.Delete();
                    deletedCount++;
                }
            }

            return deletedCount;
        }

        private void LoadSettings()
        {
            settingsFilePath = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "LogRotatorSettings.xml");
            if (File.Exists(settingsFilePath))
            {
                using (StreamReader reader = new StreamReader(settingsFilePath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(PluginSettings));
                    settings = (PluginSettings)serializer.Deserialize(reader);
                }
            }
            else
            {
                settings = new PluginSettings
                {
                    DaysToKeep = 30,
                    LogfilePath = Path.GetDirectoryName(ActGlobals.oFormActMain.LogFilePath),
                    AutoRunOnLaunch = false
                };
                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            using (StreamWriter writer = new StreamWriter(settingsFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PluginSettings));
                serializer.Serialize(writer, settings);
            }
        }

        public void DeInitPlugin()
        {
            lblStatus.Text = "Plugin Unloaded.";
        }
    }
}
