using System.Collections.Generic;
using System.Xml.Serialization;

namespace RomTidy
{
    [XmlRoot("gameList")]
    public class GameList
    {
        [XmlElement("game")]
        public List<Game> Games { get; set; } = new List<Game>();
    }
}
