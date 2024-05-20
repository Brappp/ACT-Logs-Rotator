using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace LogsRotate
{
    public class AuditManager
    {
        public List<AuditRecord> AuditRecords { get; private set; }
        public string AuditTrailPath { get; private set; }

        public AuditManager(string path)
        {
            this.AuditTrailPath = path;
            LoadAuditTrail();
        }

        public class AuditRecord
        {
            public DateTime Timestamp { get; set; }
            public string Description { get; set; }
            public bool IsAutomatic { get; set; }
        }

        private void LoadAuditTrail()
        {
            if (File.Exists(AuditTrailPath))
            {
                using (StreamReader reader = new StreamReader(AuditTrailPath))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<AuditRecord>));
                    AuditRecords = (List<AuditRecord>)serializer.Deserialize(reader);
                }
            }
            else
            {
                AuditRecords = new List<AuditRecord>();
            }
        }

        public void LogAction(string description, bool isAutomatic)
        {
            AuditRecord record = new AuditRecord
            {
                Timestamp = DateTime.Now,
                Description = description,
                IsAutomatic = isAutomatic
            };
            AuditRecords.Add(record);
            SaveAuditTrail();
        }

        private void SaveAuditTrail()
        {
            using (StreamWriter writer = new StreamWriter(AuditTrailPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<AuditRecord>));
                serializer.Serialize(writer, AuditRecords);
            }
        }

        public void ClearAuditTrail()
        {
            AuditRecords.Clear();
            SaveAuditTrail();
        }
    }
}
