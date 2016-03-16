using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Engine.GameElements
{
    public class Layer
    {
        public class TileMap
        {
            [XmlElement("Row")]
            public List<string> Row;

            public TileMap()
            {
                Row = new List<string>();
            }
        }

        public string LayerId;
        public string LayerName;
        [XmlElement("TileMap")]
        public TileMap Tile;

        public Layer()
        {
            LayerId = Guid.NewGuid().ToString();
            LayerName = "New MapLayer";
        }
    }
}
