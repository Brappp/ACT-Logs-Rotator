$directoryPath = "$env:APPDATA\Advanced Combat Tracker\FFXIVLogs"

if (-Not (Test-Path $directoryPath)) {
    New-Item -Path $directoryPath -ItemType Directory
}

for ($i = 0; $i -lt 10000; $i++) {

    $fileName = "log_" + (Get-Date).ToString("yyyyMMdd") + "_$i.log"
    $filePath = Join-Path -Path $directoryPath -ChildPath $fileName

    New-Item -Path $filePath -ItemType File

    Add-Content -Path $filePath -Value "This is a fake log entry."

    # Modify the creation time to simulate older logs
    $creationTime = (Get-Date).AddDays(-$i)
    (Get-Item $filePath).CreationTime = $creationTime
}
