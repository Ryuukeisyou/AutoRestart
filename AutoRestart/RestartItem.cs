using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AutoRestart
{
    public class RestartItem
    {
        public string Location { get; set; }
        public string AppName { get; set; }
        public List<DateTime> Times { get; set; }
        public bool BringToFront { get; set; }
    }
}
