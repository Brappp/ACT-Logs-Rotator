# ACT Logs Rotator Plugin

The ACT Logs Rotator Plugin is an Advanced Combat Tracker (ACT) plugin designed to manage and rotate logs based on user-defined settings. It helps in maintaining a clean log directory by automatically deleting older logs and provides features for protecting specific logs, auditing actions, and automatically updating the plugin.

## Features

- **Log Management**: Automatically delete logs older than a specified number of days.
- **Log Protection**: Allow users to protect specific logs from being deleted.
- **Audit Trail**: Maintain a record of actions performed by the plugin.
- **Automatic Updates**: Check for and notify the user of new updates available for the plugin.

## Components

### LogRotatorPlugin

The main plugin class that implements `IActPluginV1`. It manages plugin initialization, UI components, and integration with the ACT framework.

### AuditManager

Manages the audit trail for the plugin actions. It records each action with a timestamp, description, and whether the action was performed automatically.

#### Properties
- **AuditRecords**: A list of all audit records.

#### Methods
- **LogAction**: Logs an action to the audit trail.
- **ClearAuditTrail**: Clears the audit trail.

### PluginSettings

Holds the configuration settings for the plugin.

#### Properties
- **DaysToKeep**: The number of days to keep logs before they are deleted.
- **LogfilePath**: The path where logs are stored.
- **AutoRunOnLaunch**: Whether the plugin should run automatically on ACT launch.
- **LockedLogs**: A set of logs that are protected from deletion.

### UpdateChecker

Checks for updates to the plugin by contacting a GitHub repository.

#### Methods
- **CheckForUpdates**: Checks GitHub for the latest release of the plugin and compares it with the current version.

## Installation

1. Download the latest release from the GitHub repository.
2. Extract the ZIP file into your ACT plugins directory.
3. In ACT, go to `Plugins` > `Plugin Listing` and add `LogRotatorPlugin.dll`.
4. Restart ACT to load the plugin.

## Usage

After installation, the plugin tab will appear in the ACT interface. Configure the plugin settings according to your preferences:

- Set the number of days to keep logs.
- Specify the log file path.
- Choose whether to run the plugin automatically on ACT launch.
- Protect specific logs via the plugin UI.

## File Locations

- **Settings File**: Located at `%AppData%/Advanced Combat Tracker/Config/LogRotatorSettings.xml`
- **Audit Trail File**: Located at `%AppData%/Advanced Combat Tracker/Logs/LogRotatorAuditTrail.xml`

## Checking for Updates

The plugin can automatically check for new updates on GitHub. If a new version is available, it will prompt you to download it. Here's how you can update your plugin:

1. Click "Yes" when prompted to download the latest version.
2. The browser will open the download page where you can download the new version's `.zip` file.
3. Once downloaded, extract the `.zip` file.
4. Replace the existing `LogRotatorPlugin.dll` in your ACT plugins directory with the new one from the extracted folder.
5. Restart ACT for the changes to take effect.
