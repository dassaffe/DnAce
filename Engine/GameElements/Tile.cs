using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.GameElements
{
    public class Tile
    {
        private State _state;
        public TileType TileType { get; private set; }
        public Dictionary<Vector2, Rectangle> PosSourceRect { get; private set; }
        public int AutoTile { get; private set; }

        public void LoadContent(Dictionary<Vector2, Rectangle> posSourceRect, State state, TileType tileType,
            int autoTile)
        {
            PosSourceRect = posSourceRect;
            _state = state;
            TileType = tileType;
            AutoTile = autoTile;
        }

        public void UnloadContent()
        {
        }

        public void Update(ref Player player, Vector2 mapSize, Vector2 tileDimensions)
        {
            if (_state == State.Solid) // Collision Detection
            {
                foreach (var posSrcRect in PosSourceRect)
                {
                    var tileRect = new Rectangle((int)posSrcRect.Key.X, (int)posSrcRect.Key.Y, (int)tileDimensions.X, (int)tileDimensions.Y);
                    var playerRect = new Rectangle((int)player.Image.Position.X, (int)player.Image.Position.Y, player.Image.SourceRect.Width, player.Image.SourceRect.Height);

                    if (playerRect.Intersects(tileRect))
                    {
                        if (player.Velocity.X < 0) // move left
                        {
                            player.Image.Position.X = tileRect.Right;
                        }
                        else if (player.Velocity.X > 0) // move right
                        {
                            player.Image.Position.X = tileRect.Left - player.Image.SourceRect.Width;
                        }
                        else if (player.Velocity.Y < 0) // move up
                        {
                            player.Image.Position.Y = tileRect.Bottom;
                        }
                        else if (player.Velocity.Y > 0) // move down
                        {
                            player.Image.Position.Y = tileRect.Top - player.Image.SourceRect.Height;
                        }

                        player.Velocity = Vector2.Zero;
                    }
                }
            }

            if (player.Image.Position.X < 0)
            {
                player.Image.Position.X = 0;
            }
            else if (player.Image.Position.X >
                     mapSize.X * tileDimensions.X - player.Image.Size.X / player.Image.SpriteSheetEffect.AmountOfFrames.X)
            {
                player.Image.Position.X = mapSize.X * tileDimensions.X -
                                          player.Image.Size.X / player.Image.SpriteSheetEffect.AmountOfFrames.X;
            }

            if (player.Image.Position.Y < 0)
            {
                player.Image.Position.Y = 0;
            }
            else if (player.Image.Position.Y >
                     mapSize.Y * tileDimensions.Y - player.Image.Size.Y / player.Image.SpriteSheetEffect.AmountOfFrames.Y)
            {
                player.Image.Position.Y = mapSize.Y * tileDimensions.Y -
                                          player.Image.Size.Y / player.Image.SpriteSheetEffect.AmountOfFrames.Y;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}