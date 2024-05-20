﻿using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Advanced_Combat_Tracker;

namespace LogsRotate
{
    public class LogRotatorPlugin : IActPluginV1
    {
        private TabPage pluginTab;
        private Label lblStatus;
        private TextBox txtDaysToKeep, txtLogFilePath;
        private Button btnSave, btnRun, btnClearAudit;
        private ProgressBar progressBar;
        private CheckBox chkAutoRunOnLaunch;
        private LinkLabel lnkGitHub;
        private Label lblLastRun, lblLogsDeleted;
        private ListBox lstAllLogs, lstProtectedLogs, lstAuditTrail;
        private PluginSettings settings;
        private string settingsFilePath;
        private AuditManager auditManager;

        public async void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            this.pluginTab = pluginScreenSpace;
            this.lblStatus = pluginStatusText;
            LoadSettings();
            InitializeUIComponents();
            UpdateUIFromSettings();

            string auditPath = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "LogRotatorAuditTrail.xml");
            auditManager = new AuditManager(auditPath);
            PopulateAuditTrailUI();

            if (settings.AutoRunOnLaunch)
            {
                RunLogRotation();
            }

            await UpdateChecker.CheckForUpdates();

            pluginStatusText.Text = "Started";
        }

        private void InitializeUIComponents()
        {
            pluginTab.Controls.Clear();

            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                AutoSize = true
            };

            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Settings
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Actions
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F)); // Logs
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F)); // Audit Trail
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Last Run & Logs Deleted
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // GitHub link

            GroupBox settingsGroup = CreateSettingsGroup();
            mainLayout.Controls.Add(settingsGroup, 0, 0);
            mainLayout.SetColumnSpan(settingsGroup, 2);

            GroupBox actionsGroup = CreateActionsGroup();
            mainLayout.Controls.Add(actionsGroup, 0, 1);
            mainLayout.SetColumnSpan(actionsGroup, 2);

            GroupBox logsGroup = CreateLogsGroup();
            mainLayout.Controls.Add(logsGroup, 0, 2);
            mainLayout.SetColumnSpan(logsGroup, 2);

            GroupBox auditGroup = CreateAuditGroup();
            mainLayout.Controls.Add(auditGroup, 0, 3);
            mainLayout.SetColumnSpan(auditGroup, 2);

            SetUpStatusAndLinks(mainLayout);

            pluginTab.Controls.Add(mainLayout);
        }

        private void UpdateUIFromSettings()
        {
            txtDaysToKeep.Text = settings.DaysToKeep.ToString();
            txtLogFilePath.Text = settings.LogfilePath;
            chkAutoRunOnLaunch.Checked = settings.AutoRunOnLaunch;

            lblStatus.Text = "Settings Loaded.";
        }

        private GroupBox CreateSettingsGroup()
        {
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
                RowCount = 3,
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

            settingsGroup.Controls.Add(settingsLayout);
            return settingsGroup;
        }

        private GroupBox CreateActionsGroup()
        {
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
                RowCount = 1,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            btnRun = new Button { Text = "Run Now", Width = 100, Anchor = AnchorStyles.Left };
            btnRun.Click += BtnRun_Click;
            actionsLayout.Controls.Add(btnRun, 0, 0);

            progressBar = new ProgressBar { Dock = DockStyle.Fill };
            actionsLayout.Controls.Add(progressBar, 1, 0);

            TableLayoutPanel actionButtonsLayout = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 2,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            btnSave = new Button { Text = "Save Settings", Width = 150, Anchor = AnchorStyles.Left };
            btnSave.Click += BtnSave_Click;
            actionButtonsLayout.Controls.Add(btnSave, 0, 0);

            btnClearAudit = new Button { Text = "Clear Audit Log", Width = 150, Anchor = AnchorStyles.Left };
            btnClearAudit.Click += BtnClearAudit_Click;
            actionButtonsLayout.Controls.Add(btnClearAudit, 0, 1);

            actionsLayout.Controls.Add(actionButtonsLayout, 2, 0);

            actionsGroup.Controls.Add(actionsLayout);
            return actionsGroup;
        }

        private GroupBox CreateLogsGroup()
        {
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

            logsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            logsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            logsLayout.Controls.Add(new Label { Text = "All Logs:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
            lstAllLogs = new ListBox { Dock = DockStyle.Fill, Height = 100 }; // Set fixed height for All Logs
            lstAllLogs.DoubleClick += LstAllLogs_DoubleClick;
            logsLayout.Controls.Add(lstAllLogs, 0, 1);

            logsLayout.Controls.Add(new Label { Text = "Protected Logs:", Anchor = AnchorStyles.Left, AutoSize = true }, 1, 0);
            lstProtectedLogs = new ListBox { Dock = DockStyle.Fill, Height = 100 }; // Set fixed height for Protected Logs
            lstProtectedLogs.DoubleClick += LstProtectedLogs_DoubleClick;
            logsLayout.Controls.Add(lstProtectedLogs, 1, 1);

            logsGroup.Controls.Add(logsLayout);
            return logsGroup;
        }

        private GroupBox CreateAuditGroup()
        {
            GroupBox auditGroup = new GroupBox
            {
                Text = "Audit Log",
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                AutoSize = true
            };

            TableLayoutPanel auditLayout = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 1,
                AutoSize = true,
                Dock = DockStyle.Fill
            };

            lstAuditTrail = new ListBox
            {
                Dock = DockStyle.Fill,
                Height = 200 // Set a fixed height for the list box
            };
            auditLayout.Controls.Add(lstAuditTrail, 0, 0);

            auditGroup.Controls.Add(auditLayout);

            return auditGroup;
        }

        private void SetUpStatusAndLinks(TableLayoutPanel layout)
        {
            lblLastRun = new Label { Text = "Last Run: Never", Anchor = AnchorStyles.Left, AutoSize = true };
            lblLogsDeleted = new Label { Text = "Logs Deleted: 0", Anchor = AnchorStyles.Left, AutoSize = true };
            layout.Controls.Add(lblLastRun, 0, 4);
            layout.Controls.Add(lblLogsDeleted, 1, 4);

            lnkGitHub = new LinkLabel { Text = "https://github.com/Brappp/ACT-Logs-Rotator", AutoSize = true };
            lnkGitHub.LinkClicked += LnkGitHub_LinkClicked;
            layout.Controls.Add(lnkGitHub, 0, 5);
            layout.SetColumnSpan(lnkGitHub, 2);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                string oldSettings = $"Days to Keep: {settings.DaysToKeep}, Log File Path: {settings.LogfilePath}, Auto Run on Launch: {settings.AutoRunOnLaunch}";

                settings.DaysToKeep = int.Parse(txtDaysToKeep.Text);
                settings.LogfilePath = txtLogFilePath.Text;
                settings.AutoRunOnLaunch = chkAutoRunOnLaunch.Checked;

                string newSettings = $"Days to Keep: {settings.DaysToKeep}, Log File Path: {settings.LogfilePath}, Auto Run on Launch: {settings.AutoRunOnLaunch}";

                SaveSettings();
                LogAction($"Settings Changed: {oldSettings} -> {newSettings}", false);
                UpdateLogLists();
            }
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            RunLogRotation();
        }

        private void BtnClearAudit_Click(object sender, EventArgs e)
        {
            auditManager.ClearAuditTrail();
            PopulateAuditTrailUI();
            LogAction("Audit log cleared", false);
        }

        private void RunLogRotation()
        {
            progressBar.Value = 0;
            int logsDeleted = RotateLogs();
            progressBar.Value = 100;

            lblLastRun.Text = $"Last Run: {DateTime.Now}";
            lblLogsDeleted.Text = $"Logs Deleted: {logsDeleted}";
            LogAction($"Rotation Run: {logsDeleted} logs deleted", false);

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

        private void LstAllLogs_DoubleClick(object sender, EventArgs e)
        {
            if (lstAllLogs.SelectedItem != null)
            {
                string selectedLog = lstAllLogs.SelectedItem.ToString();
                settings.LockedLogs.Add(selectedLog);
                UpdateLogLists();
                SaveSettings();
                LogAction($"Log Protected: {selectedLog}", false);
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
                LogAction($"Log Unprotected: {selectedLog}", false);
            }
        }

        private void LnkGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Brappp/ACT-Logs-Rotator");
        }

        private void PopulateAuditTrailUI()
        {
            lstAuditTrail.Items.Clear();
            foreach (var record in auditManager.AuditRecords)
            {
                lstAuditTrail.Items.Add($"{record.Timestamp}: {record.Description} (Auto: {record.IsAutomatic})");
            }
        }

        private void LogAction(string description, bool isAutomatic)
        {
            auditManager.LogAction(description, isAutomatic);
            lstAuditTrail.Items.Insert(0, $"{DateTime.Now}: {description} (Auto: {isAutomatic})");
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

        public void DeInitPlugin()
        {
            lblStatus.Text = "Plugin Unloaded.";
        }
    }
}
