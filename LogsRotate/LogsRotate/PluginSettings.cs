using System.Collections.Generic;

namespace LogsRotate
{
    public class PluginSettings
    {
        public int DaysToKeep { get; set; } = 30;
        public string LogfilePath { get; set; }
        public bool AutoRunOnLaunch { get; set; } = false;
        public HashSet<string> LockedLogs { get; set; } = new HashSet<string>();

        public PluginSettings()
        {
            DaysToKeep = 30;
            LogfilePath = string.Empty;
            AutoRunOnLaunch = false;
            LockedLogs = new HashSet<string>();
        }
    }
}
