using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.GameElements
{
    public class EventLayer : Layer
    {
        [XmlElement("Events")]
        public List<Event> EventList;

        public EventLayer()
        {
            LayerId = Guid.NewGuid().ToString();
            LayerName = "New Eventlayer";
            EventList = new List<Event>();

            //var ev = new Event();
            //var teleport = new Event.TeleportEvent("3BEE7BF6-4998-4D0F-BEAF-3E97F31E40FF", new Vector2(5, 0));
            //ev.EventColl.Add(teleport.ToString());
            //teleport = new Event.TeleportEvent("3BEE7BF6-4998-4D0F-BEAF-3E97F31E40FF", new Vector2(6, 0));
            //ev.EventColl.Add(teleport.ToString());
            //EventList.Add(ev);

            //var ev2 = new Event();
            //var teleport2 = new Event.TeleportEvent("3BEE7BF6-4998-4D0F-BEAF-3E97F31E40FF",new Vector2(5, 0));
            //ev2.EventColl.Add(teleport2.ToString());
            //EventList.Add(ev2);
        }

        public void LoadContent(Vector2 tileDimensions)
        {
            Vector2 position = -tileDimensions;

            foreach (string row in Tile.Row)
            {
                string[] split = row.Split(']');

                position.X = -tileDimensions.X;
                position.Y += tileDimensions.Y;
                foreach (string s in split)
                {
                    if (!String.IsNullOrEmpty(s))
                    {
                        position.X += tileDimensions.X;
                        if (!s.Contains("x"))
                        {
                            var tile = new Tile();

                            string str = s.Replace("[", String.Empty);
                            string value1 = str.Substring(0, str.IndexOf(':'));
                            int value2 = int.Parse(str.Substring(str.IndexOf(':') + 1));

                            //tile.LoadContent(position, new Rectangle(value1 * (int)tileDimensions.X, value2 * (int)tileDimensions.Y, (int)tileDimensions.X, (int)tileDimensions.Y));

                            //eventTiles.Add(tile);
                        }
                    }
                }
            }
        }

        public void UnloadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            //foreach (var tile in eventTiles)
            //    tile.Update(gameTime, ref player);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}