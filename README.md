# ACT Logs Rotator Plugin

The ACT Logs Rotator Plugin is a tool for managing log files in Advanced Combat Tracker (ACT). This plugin helps you delete old logs based on your specified criteria, ensuring your log folder stays clean and organized.

## Features

- **Days to Keep Logs**: Specify the number of days to keep logs. Logs older than this duration are automatically deleted.
- **Log File Path**: Set the path where the log files are stored.
- **Auto Run on Launch**: Enable or disable automatic log rotation when ACT is launched.
- **Protected Logs**: Protect specific logs from being deleted.
- **Manual Log Rotation**: Manually trigger log rotation at any time.

## User Interface

The plugin's user interface is organized into several sections:

1. **Settings**: Configure the number of days to keep logs, the log file path, and the auto run on launch option.
2. **Actions**: Buttons to save settings and manually run the log rotation.
3. **Logs**: Displays lists of all logs and protected logs, allowing users to manage log protection interactively.
4. **Status**: Shows information about the last run and the number of logs deleted.

## How It Works

1. **Initialization**: Loads settings from an XML file and sets up the UI components.
2. **Settings Management**: Users can configure log management settings, including the number of days to keep logs and the log file path.
3. **Log Rotation**: Deletes logs older than the specified number of days, except those marked as protected.
4. **Interactive Log Management**: Allows users to protect or unprotect logs by moving them between the "All Logs" and "Protected Logs" lists.

## Installation and Usage

1. **Install the Plugin**: Add the ACT Logs Rotator Plugin to ACT.
2. **Configure Settings**:
   - Open the plugin tab in ACT.
   - Enter the number of days to keep logs.
   - Enter the path to your log files.
   - Enable auto run on launch if desired.
   - Save your settings.
3. **Run Log Rotation**:
   - Click the "Run" button to manually trigger log rotation.
   - The progress bar will display the status.
4. **Manage Logs**:
   - Double-click logs in the "All Logs" list to move them to the "Protected Logs" list.
   - Double-click logs in the "Protected Logs" list to move them back to the "All Logs" list.
5. **View Status**:
   - Check the status section for information on the last run and the number of logs deleted.

## Code Structure

### `PluginSettings` Class

- Stores user-defined settings.
- Fields:
  - `int DaysToKeep`
  - `string LogfilePath`
  - `bool AutoRunOnLaunch`
  - `HashSet<string> LockedLogs`
- Methods: Constructor initializes default settings.

### `LogRotatorPlugin` Class

- Manages the UI and log rotation logic.
- Fields: Various UI controls and settings.
- Methods:
  - `InitPlugin(TabPage, Label)`: Initializes the plugin.
  - `InitializeUIComponents()`: Sets up the UI controls.
  - `UpdateLogLists()`: Updates the log lists in the UI.
  - `BtnSave_Click(Object, EventArgs)`: Saves settings.
  - `BtnRun_Click(Object, EventArgs)`: Runs log rotation.
  - `RotateLogs()`: Deletes old logs based on the specified criteria.
  - `LoadSettings()`: Loads settings from an XML file.
  - `SaveSettings()`: Saves settings to an XML file.
  - `DeInitPlugin()`: Cleans up when the plugin is unloaded.

## License

This project is licensed under the MIT License.
