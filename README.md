# LogRotatorPlugin for ACT

## Overview
The LogRotatorPlugin is a plugin for Advanced Combat Tracker (ACT) that manages log files by deleting old logs based on user-defined criteria. It allows users to specify how many days of logs to keep, automatically run log rotation on launch, and protect specific logs from deletion. Additionally, users can interactively manage their logs through a graphical user interface (GUI).

## Features
- **Days to Keep Logs**: Specify the number of days to keep logs. Logs older than this duration are deleted.
- **Log File Path**: Set the path where the log files are stored.
- **Auto Run on Launch**: Enable or disable automatic log rotation when ACT is launched.
- **Protected Logs**: Protect specific logs from being deleted.
- **Run Now**: Manually trigger log rotation.
- **GitHub Link**: Access the plugin’s GitHub repository for updates and documentation.

## User Interface
The plugin's user interface is organized into several sections:

1. **Settings**: Contains fields for configuring the number of days to keep logs, the log file path, and a checkbox for enabling auto run on launch.
2. **Actions**: Contains buttons to save settings and manually run the log rotation.
3. **Logs**: Displays two lists - "All Logs" and "Protected Logs". Users can move logs between these lists by double-clicking.
4. **Status**: Displays information about the last run and the number of logs deleted.
5. **GitHub Link**: Provides a link to the plugin's GitHub repository.

## How It Works
1. **Initialization**: When the plugin is initialized, it loads settings from an XML file and sets up the UI components.
2. **Settings Management**: Users can enter settings for log management. The settings include the number of days to keep logs, the path to the log files, and whether to run log rotation on launch.
3. **Log Rotation**: When triggered, either automatically or manually, the plugin deletes logs older than the specified number of days, except those marked as protected.
4. **Interactive Log Management**: Users can double-click logs in the "All Logs" list to protect them, moving them to the "Protected Logs" list, and vice versa.

## How to Use
1. **Install the Plugin**: Add the LogRotatorPlugin to ACT.
2. **Configure Settings**:
    - Open the plugin tab in ACT.
    - Enter the number of days to keep logs in the "Days to Keep" field.
    - Enter the path to your log files in the "Log File Path" field.
    - Check the "Auto Run on Launch" checkbox if you want the plugin to run automatically when ACT starts.
    - Click "Save" to save your settings.
3. **Run Log Rotation**:
    - Click the "Run" button in the Actions section to manually trigger log rotation.
    - The progress bar will show the status, and the logs will be processed according to the specified settings.
4. **Manage Logs**:
    - Double-click a log in the "All Logs" list to move it to the "Protected Logs" list, preventing it from being deleted.
    - Double-click a log in the "Protected Logs" list to move it back to the "All Logs" list, making it eligible for deletion.
5. **View Status**:
    - Check the "Last Run" label to see when the log rotation was last run.
    - Check the "Logs Deleted" label to see how many logs were deleted in the last run.

## Code Structure
### `PluginSettings` Class
- **Purpose**: Stores user-defined settings.
- **Fields**:
  - `int DaysToKeep`: Number of days to keep logs.
  - `string LogfilePath`: Path to log files.
  - `bool AutoRunOnLaunch`: Indicates if log rotation should run on ACT launch.
  - `HashSet<string> LockedLogs`: Stores logs protected from deletion.
- **Methods**: Constructor initializes default settings.

### `LogRotatorPlugin` Class
- **Purpose**: Main plugin class managing the UI and log rotation logic.
- **Fields**:
  - Various UI controls (`Label`, `TextBox`, `Button`, `ProgressBar`, `CheckBox`, `LinkLabel`, `ListBox`).
  - `PluginSettings settings`: Stores plugin settings.
  - `string settingsFilePath`: Path to the settings file.
- **Methods**:
  - `InitPlugin(TabPage, Label)`: Initializes the plugin, loads settings, sets up UI components, and updates the UI.
  - `InitializeUIComponents()`: Sets up the UI controls.
  - `LstAllLogs_DoubleClick(Object, EventArgs)`: Moves a log from "All Logs" to "Protected Logs".
  - `LstProtectedLogs_DoubleClick(Object, EventArgs)`: Moves a log from "Protected Logs" to "All Logs".
  - `UpdateLogLists()`: Updates the log lists displayed in the UI.
  - `LnkGitHub_LinkClicked(Object, LinkLabelLinkClickedEventArgs)`: Opens the GitHub repository link.
  - `UpdateUIFromSettings()`: Updates the UI based on the current settings.
  - `BtnSave_Click(Object, EventArgs)`: Saves settings.
  - `ValidateInputs()`: Validates user inputs for settings.
  - `BtnRun_Click(Object, EventArgs)`: Runs log rotation.
  - `RotateLogs()`: Deletes old logs based on the specified criteria.
  - `LoadSettings()`: Loads settings from an XML file.
  - `SaveSettings()`: Saves settings to an XML file.
  - `DeInitPlugin()`: Cleans up when the plugin is unloaded.
