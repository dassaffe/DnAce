using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.GameElements
{
    public class MapLayer : Layer
    {
        private readonly List<Tile> _overLayTiles;
        private readonly List<Tile> _underLayTiles;
        private State _state;
        [XmlElement("AutoTileSheets")]
        public List<Tilesheet> AutoTileSheets;
        public string OverlayTiles;
        public string SolidTiles;
        public Tilesheet TileSheet;

        public MapLayer()
        {
            TileSheet = new Tilesheet();
            AutoTileSheets = new List<Tilesheet>();
            _underLayTiles = new List<Tile>();
            _overLayTiles = new List<Tile>();
            SolidTiles = String.Empty;
            OverlayTiles = String.Empty;
            LayerId = Guid.NewGuid().ToString();
            LayerName = "New Maplayer";
        }

        public void ChangeTile(Vector2 position, Vector2 newTile, string tileType)
        {
            var split = Tile.Row[(int)position.Y].Split(']');
            var newRow = String.Empty;
            for (var xIndex = 0; xIndex < split.Length; xIndex++)
            {
                if (!String.IsNullOrEmpty(split[xIndex]))
                {
                    if (xIndex == (int)position.X)
                    {
                        var type = String.Empty;
                        switch (tileType)
                        {
                            case "Map":
                                type = "M";
                                break;

                            case "Autotile":
                                type = "A";
                                break;

                            case "Event":
                                type = "E";
                                break;
                        }
                        newRow += String.Format("[{0}:{1}:{2}]", type, (int)newTile.X, (int)newTile.Y);
                    }
                    else
                    {
                        newRow += split[xIndex] + "]";
                    }
                }
            }

            Tile.Row[(int)position.Y] = newRow;
        }

        public static string GetTileAt(TileMap mapLayer, int posX, int posY)
        {
            if (posY < 0 || posY > mapLayer.Row.Count - 1)
                return "[M:x:x]";

            var row = mapLayer.Row[posY];
            var field = row.Split(']');

            if (posX < 0 || posX > field.Length - 2)
                return "[M:x:x]";

            return field[posX] + "]";
        }

        public void LoadContent(Vector2 tileDimensions)
        {
            TileSheet.LoadContent(TileType.Map);

            foreach (var autoTileSheet in AutoTileSheets)
            {
                autoTileSheet.LoadContent(TileType.Autotile);
            }

            var position = -tileDimensions;

            for (var posY = 0; posY < Tile.Row.Count; posY++)
            {
                var split = Tile.Row[posY].Split(']');

                position.X = -tileDimensions.X;
                position.Y += tileDimensions.Y;
                for (var posX = 0; posX < split.Length; posX++)
                {
                    if (!String.IsNullOrEmpty(split[posX]))
                    {
                        position.X += tileDimensions.X;
                        if (!split[posX].Contains("x"))
                        {
                            _state = State.Passive;
                            var tile = new Tile();

                            var str = split[posX].Replace("[", String.Empty);
                            var splitStr = str.Split(':');
                            var value1 = Int32.Parse(splitStr[1]);
                            var value2 = Int32.Parse(splitStr[2]);

                            if (SolidTiles.Contains(split[posX] + "]"))
                                _state = State.Solid;

                            var tileType = TileType.Event;
                            switch (splitStr[0])
                            {
                                case "A":
                                    tileType = TileType.Autotile;
                                    break;
                                case "M":
                                    tileType = TileType.Map;
                                    break;
                            }

                            if (tileType == TileType.Map)
                            {
                                var srcRect = new Rectangle(value1 * (int)tileDimensions.X, value2 * (int)tileDimensions.Y,
                                    (int)tileDimensions.X, (int)tileDimensions.Y);

                                tile.LoadContent(new Dictionary<Vector2, Rectangle> { { position, srcRect } }, _state,
                                    tileType, value1);

                                if (OverlayTiles.Contains(split[posX] + "]"))
                                    _overLayTiles.Add(tile);
                                else
                                    _underLayTiles.Add(tile);
                            }
                            else if (tileType == TileType.Autotile)
                            {
                                var actTile = split[posX] + "]";
                                var tileLeft = GetTileAt(Tile, posX - 1, posY);
                                var tileRight = GetTileAt(Tile, posX + 1, posY);
                                var tileTop = GetTileAt(Tile, posX, posY - 1);
                                var tileBottom = GetTileAt(Tile, posX, posY + 1);

                                var tileTopLeft = GetTileAt(Tile, posX - 1, posY - 1);
                                var tileTopRight = GetTileAt(Tile, posX + 1, posY - 1);
                                var tileBottomLeft = GetTileAt(Tile, posX - 1, posY + 1);
                                var tileBottomRight = GetTileAt(Tile, posX + 1, posY + 1);
                                var srcRect = GetSrcRect(tileDimensions, actTile.Equals(tileLeft),
                                    actTile.Equals(tileRight), actTile.Equals(tileTop), actTile.Equals(tileBottom),
                                    actTile.Equals(tileTopLeft), actTile.Equals(tileTopRight),
                                    actTile.Equals(tileBottomLeft), actTile.Equals(tileBottomRight));

                                if (srcRect.Count == 2)
                                    Debug.Write("fsdfsd");
                                else if (srcRect.Count == 4)
                                    Debug.Write("fsdfsd");
                                var dict = new Dictionary<Vector2, Rectangle>();
                                for (int index = 0; index < srcRect.Count; index++)
                                {
                                    var newPos = new Vector2(position.X, position.Y);

                                    if (srcRect.Count == 2)
                                    {
                                        if (srcRect[index].Width == (int)tileDimensions.X && index % 2 == 1) // Oben + Unten
                                            newPos = new Vector2(position.X, position.Y + tileDimensions.Y / 2);
                                        else if (srcRect[index].Height == (int)tileDimensions.Y && index % 2 == 1) // Links + Rechts
                                            newPos = new Vector2(position.X + tileDimensions.X / 2, position.Y);
                                    }
                                    else if (srcRect.Count == 4)
                                    {
                                        if (index == 1) { newPos = new Vector2(position.X + tileDimensions.X / 2, position.Y); }
                                        else if (index == 2) { newPos = new Vector2(position.X, position.Y + tileDimensions.Y / 2); }
                                        else if (index == 3) { newPos = new Vector2(position.X + tileDimensions.X / 2, position.Y + tileDimensions.Y / 2); }
                                    }
                                    dict = new Dictionary<Vector2, Rectangle> { { newPos, srcRect[index] } };
                                }

                                tile.LoadContent(dict, _state, tileType, value1);

                                if (OverlayTiles.Contains(split[posX] + "]"))
                                    _overLayTiles.Add(tile);
                                else
                                    _underLayTiles.Add(tile);
                            }
                        }
                    }
                }
            }
        }

        private List<Rectangle> GetSrcRect(Vector2 tileDimensions, bool isLeft, bool isRight, bool isTop, bool isBottom,
            bool isTopLeft, bool isTopRight, bool isBottomLeft, bool isBottomRight)
        {
            var srcRect = new List<Rectangle>();

            if (!isLeft && !isRight && !isTop && !isBottom)
            {
                srcRect.Add(new Rectangle(0, 0, (int)tileDimensions.X, (int)tileDimensions.Y));
            }
            else if (isLeft && !isRight && !isTop && !isBottom)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X, (int)tileDimensions.Y, (int)tileDimensions.X,
                    (int)tileDimensions.Y / 2));

                srcRect.Add(new Rectangle((int)tileDimensions.X, (int)tileDimensions.Y / 2 * 5, (int)tileDimensions.X,
                    (int)tileDimensions.Y / 2));
            }
            else if (!isLeft && isRight && !isTop && !isBottom)
            {
                srcRect.Add(new Rectangle(0, (int)tileDimensions.Y, (int)tileDimensions.X, (int)tileDimensions.Y / 2));

                srcRect.Add(new Rectangle(0, (int)tileDimensions.Y / 2 * 5, (int)tileDimensions.X,
                    (int)tileDimensions.Y / 2));
            }
            else if (!isLeft && !isRight && isTop && !isBottom)
            {
                srcRect.Add(new Rectangle(0, (int)tileDimensions.Y * 2, (int)tileDimensions.X / 2, (int)tileDimensions.Y));

                srcRect.Add(new Rectangle((int)tileDimensions.X / 2 * 3, (int)tileDimensions.Y * 2, (int)tileDimensions.X / 2,
                    (int)tileDimensions.Y));
            }
            else if (!isLeft && !isRight && !isTop && isBottom)
            {
                srcRect.Add(new Rectangle(0, (int)tileDimensions.Y, (int)tileDimensions.X / 2, (int)tileDimensions.Y));

                srcRect.Add(new Rectangle((int)tileDimensions.X / 2 * 3, (int)tileDimensions.Y, (int)tileDimensions.X / 2,
                    (int)tileDimensions.Y));
            }
            else if (isLeft && isRight && !isTop && !isBottom)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X / 2, (int)tileDimensions.Y, (int)tileDimensions.X,
                    (int)tileDimensions.Y / 2));

                srcRect.Add(new Rectangle((int)tileDimensions.X / 2, (int)tileDimensions.Y / 2 * 5, (int)tileDimensions.X,
                    (int)tileDimensions.Y / 2));
            }
            else if (isLeft && !isRight && isTop && !isBottom)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X, (int)tileDimensions.Y * 2, (int)tileDimensions.X,
                    (int)tileDimensions.Y));
            }
            else if (!isLeft && isRight && isTop && !isBottom)
            {
                srcRect.Add(new Rectangle(0, (int)tileDimensions.Y * 2, (int)tileDimensions.X, (int)tileDimensions.Y));
            }
            else if (isLeft && !isRight && !isTop && isBottom)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X, (int)tileDimensions.Y, (int)tileDimensions.X,
                    (int)tileDimensions.Y));
            }
            else if (!isLeft && isRight && !isTop && isBottom)
            {
                srcRect.Add(new Rectangle(0, (int)tileDimensions.Y, (int)tileDimensions.X, (int)tileDimensions.Y));
            }
            else if (!isLeft && !isRight && isTop && isBottom)
            {
                srcRect.Add(new Rectangle(0, (int)tileDimensions.Y / 2 * 3, (int)tileDimensions.X / 2,
                    (int)tileDimensions.Y));

                srcRect.Add(new Rectangle((int)tileDimensions.X / 2 * 3, (int)tileDimensions.Y / 2 * 3,
                    (int)tileDimensions.X / 2, (int)tileDimensions.Y));
            }
            else if (isLeft && isRight && isTop && !isBottom)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X / 2, (int)tileDimensions.Y * 2, (int)tileDimensions.X,
                    (int)tileDimensions.Y));
            }
            else if (isLeft && isRight && !isTop && isBottom)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X / 2, (int)tileDimensions.Y, (int)tileDimensions.X,
                    (int)tileDimensions.Y));
            }
            else if (isLeft && !isRight && isTop && isBottom)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X, (int)tileDimensions.Y / 2 * 3, (int)tileDimensions.X,
                    (int)tileDimensions.Y));
            }
            else if (!isLeft && isRight && isTop && isBottom)
            {
                srcRect.Add(new Rectangle(0, (int)tileDimensions.Y / 2 * 3, (int)tileDimensions.X, (int)tileDimensions.Y));
            }
            else if (isLeft && isRight && isTop && isBottom)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X, 0, (int)tileDimensions.X, (int)tileDimensions.Y));
            }

            if (isLeft && isTop && isTopLeft)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X / 2, (int)tileDimensions.Y / 2 * 3, (int)tileDimensions.X / 2,
                    (int)tileDimensions.Y / 2));
            }
            if (isRight && isTop && isTopRight)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X, (int)tileDimensions.Y / 2 * 3, (int)tileDimensions.X / 2,
                    (int)tileDimensions.Y / 2));
            }
            if (isLeft && isBottom && isBottomLeft)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X / 2, (int)tileDimensions.Y * 2, (int)tileDimensions.X / 2,
                    (int)tileDimensions.Y / 2));
            }
            if (isRight && isBottom && isBottomRight)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X, (int)tileDimensions.Y * 2, (int)tileDimensions.X / 2,
                    (int)tileDimensions.Y / 2));
            }

            if (isLeft && isTop && !isTopLeft)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X, 0, (int)tileDimensions.X / 2, (int)tileDimensions.Y / 2));
            }
            if (isRight && isTop && !isTopRight)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X / 2 * 3, 0, (int)tileDimensions.X / 2,
                    (int)tileDimensions.Y / 2));
            }
            if (isLeft && isBottom && !isBottomLeft)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X, (int)tileDimensions.Y / 2, (int)tileDimensions.X / 2,
                    (int)tileDimensions.Y / 2));
            }
            if (isRight && isBottom && !isBottomRight)
            {
                srcRect.Add(new Rectangle((int)tileDimensions.X / 2 * 3, (int)tileDimensions.Y / 2, (int)tileDimensions.X / 2,
                    (int)tileDimensions.Y / 2));
            }

            if (srcRect.Count == 3)
                srcRect.RemoveAt(0);
            if (srcRect.Count == 5)
                srcRect.RemoveAt(0);

            return srcRect;
        }

        public void UnloadContent()
        {
            TileSheet.UnloadContent();

            foreach (var autoTileSheet in AutoTileSheets)
            {
                autoTileSheet.UnloadContent();
            }
        }

        public void Update(ref Player player, Vector2 tileDimensions)
        {
            var mapSize = new Vector2(Tile.Row[0].Split(']').Length - 1, Tile.Row.Count);

            foreach (var tile in _underLayTiles)
                tile.Update(ref player, mapSize, tileDimensions);

            foreach (var tile in _overLayTiles)
                tile.Update(ref player, mapSize, tileDimensions);
        }

        public void Draw(SpriteBatch spriteBatch, DrawType drawType)
        {
            var tiles = (drawType == DrawType.UnderLay ? _underLayTiles : _overLayTiles).Where(x => x.TileType == TileType.Map);
            var autoTiles = (drawType == DrawType.UnderLay ? _underLayTiles : _overLayTiles).Where(x => x.TileType == TileType.Autotile);

            foreach (var tile in tiles.Where(x => x.TileType == TileType.Map))
            {
                foreach (var posSrcRect in tile.PosSourceRect)
                {
                    TileSheet.Position = posSrcRect.Key;
                    TileSheet.SourceRect = posSrcRect.Value;
                    TileSheet.Draw(spriteBatch);
                }
            }

            foreach (var tile in autoTiles)
            {
                foreach (var posSrcRect in tile.PosSourceRect)
                {
                    AutoTileSheets[tile.AutoTile].Position = posSrcRect.Key;
                    AutoTileSheets[tile.AutoTile].SourceRect = posSrcRect.Value;
                    AutoTileSheets[tile.AutoTile].Draw(spriteBatch);
                }
            }
        }
    }
}