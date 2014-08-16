using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Databox.Integration
{
    public class KPI
    {
        private string fValue { get; set; }
        private string fKey { get; set; }
        private DateTime fTimeStamp { get; set; }
        private Dictionary<string, string> fAttributes { get; set; }

        public KPI(string key, string value, DateTime date = DateTime.UtcNow, Dictionary<string, string> attributes = null)
        {
            this.fKey = key;
            this.fValue = value;
            this.fTimeStamp = date;
            if (attributes) {
                this.fAttributes = attributes;
            }
        }

        public string Key { get { return fKey;  } }
        public string Value { get { return fValue;  } }
        public DateTime Timestamp { get { return fTimeStamp; } }
        public Dictionary<string, string> Attributes { get { return fAttributes; } }
    }
}
