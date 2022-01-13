using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AutoRestart
{
    public class Settings
    {
        public List<RestartItem> RestartItems { get; set; } = new List<RestartItem> { };
        public bool Save(string fileName)
        {
            try
            {
                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Settings));
                    xmlSerializer.Serialize(stream, this);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static Settings Load(string fileName)
        {
            try
            {
                using (var stream = new FileStream(fileName, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(Settings));
                    var item = (Settings)serializer.Deserialize(stream);
                    return item;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
