using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Engine.GameElements
{
    public class Map
    {
        public string MapId;
        public string MapName;
        public Vector2 TileDimensions;
        [XmlElement("MapLayer")]
        public List<MapLayer> MapLayer;
        [XmlElement("EventLayer")]
        public List<EventLayer> EventLayer;

        public Map()
        {
            MapLayer = new List<MapLayer>();
            EventLayer = new List<EventLayer>();
            TileDimensions = Vector2.Zero;
            MapId = Guid.NewGuid().ToString();
            MapName = "New Map";
        }

        public void LoadContent()
        {
            foreach (var layer in MapLayer)
                layer.LoadContent(TileDimensions);
        }

        public void UnloadContent()
        {
            foreach (var layer in MapLayer)
                layer.UnloadContent();
        }

        public void Update(ref Player player)
        {
            foreach (var layer in MapLayer)
                layer.Update(ref player, TileDimensions);
        }

        public void Draw(SpriteBatch spriteBatch, DrawType drawType)
        {
            foreach (var layer in MapLayer)
                layer.Draw(spriteBatch, drawType);
        }
    }
}