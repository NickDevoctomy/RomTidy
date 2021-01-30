using System;
using System.Xml.Serialization;

namespace RomTidy
{
    [Serializable, XmlRoot("game")]
    public class Game
    {
        public string path { get; set; }
        public string name { get; set; }
        public string desc { get; set; }
        public string Image { get; set; }
        public string video { get; set; }
        public string Marquee { get; set; }
        public string thumbnail { get; set; }
        public string Fanart { get; set; }
        public string manual { get; set; }
        public string rating { get; set; }
        public string releasedate { get; set; }
        public string developer { get; set; }
        public string publisher { get; set; }
        public string genre { get; set; }
        public string md5 { get; set; }
        public string lang { get; set; }
    }
}
