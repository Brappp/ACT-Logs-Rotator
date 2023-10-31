# ACT Log Rotator Plugin

## Overview

This is a plugin for Advanced Combat Tracker (ACT) that automatically rotates log files based on user-defined settings. The plugin allows you to specify how many days to keep log files and provides an option to automatically run the log rotation when ACT is launched.

## Features

- Automatically delete old log files.
- Configure the number of days to keep log files.
- Option to automatically run the log rotation when ACT is launched.
- Easy-to-use graphical interface.

## Installation

1. Clone this repository or download the ZIP file.
2. Build the project using Visual Studio.
3. Copy the resulting `.dll` file to your ACT plugins directory, typically located in `%AppData%\\Advanced Combat Tracker\\Plugins`.

## Importing the Plugin into ACT

1. Open ACT.
2. Navigate to the `Plugins` tab.
3. Click on `Browse` and locate the `.dll` file you copied earlier.
4. Click on `Add/Enable Plugin`.

## Usage

1. After importing the plugin, a new tab will appear in the ACT interface.
2. Configure the following settings:

    - **Days to Keep**: The number of days to keep log files. Files older than this will be deleted.
    - **Log File Path**: The folder where your log files are stored. This is automatically populated based on ACT's log file path.
    - **Auto Run on Launch**: Check this if you want the plugin to automatically run the log rotation when ACT is launched.

3. Click on `Save` to apply the settings.
4. Click on `Run` to run the log rotation immediately.

![Flow](/UML.jpg)
