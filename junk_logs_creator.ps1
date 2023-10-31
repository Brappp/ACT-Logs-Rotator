# Define the directory where you want to generate the fake log files
$directoryPath = "$env:APPDATA\Advanced Combat Tracker\FFXIVLogs"

# Create the directory if it doesn't exist
if (-Not (Test-Path $directoryPath)) {
    New-Item -Path $directoryPath -ItemType Directory
}

# Generate 10 fake log files
for ($i = 0; $i -lt 10000; $i++) {
    # Create a file name with the current date and index
    $fileName = "log_" + (Get-Date).ToString("yyyyMMdd") + "_$i.log"
    $filePath = Join-Path -Path $directoryPath -ChildPath $fileName

    # Create the log file
    New-Item -Path $filePath -ItemType File

    # Write some fake log content
    Add-Content -Path $filePath -Value "This is a fake log entry."

    # Modify the creation time to simulate older logs
    $creationTime = (Get-Date).AddDays(-$i)
    (Get-Item $filePath).CreationTime = $creationTime
}
