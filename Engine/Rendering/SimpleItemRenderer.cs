﻿using Engine.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering
{
    /// <summary>
    /// Sprite Renderer für simple, fortlaufende Animationen.
    /// </summary>
    internal class SimpleItemRenderer : ItemRenderer
    {
        /// <summary>
        /// Anzahl Frames in der Animation
        /// </summary>
        private readonly int _frameCount;

        public SimpleItemRenderer(Item item, Camera camera, Texture2D texture)
            : base(item, camera, texture, new Point(32, 32), 200, new Point(16, 26), 1f)
        {
            _frameCount = 8;
        }

        /// <summary>
        /// Render-Methode für dieses Sprite.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch Referenz</param>
        /// <param name="offset">Der Offset der View</param>
        /// <param name="gameTime">Aktuelle Game Time</param>
        public override void Draw(SpriteBatch spriteBatch, Point offset, GameTime gameTime)
        {
            // Animationszeit neu berechnen (vergangene Millisekunden zum letzten Frame addieren)
            AnimationTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Ermittlung des aktuellen Frames
            int frame = (AnimationTime / FrameTime) % _frameCount;

            // Bestimmung der Position des Spieler-Mittelpunktes in View-Koordinaten
            int posX = (int)((Item.Position.X) * Camera.Scale) - offset.X;
            int posY = (int)((Item.Position.Y) * Camera.Scale) - offset.Y;

            Vector2 scale = new Vector2(Camera.Scale / FrameSize.X, Camera.Scale / FrameSize.Y) * FrameScale;

            Rectangle sourceRectangle = new Rectangle(
                frame * FrameSize.X,
                0,
                FrameSize.X,
                FrameSize.Y);

            Rectangle destinationRectangle = new Rectangle(
                posX - (int)(ItemOffset.X * scale.X),
                posY - (int)(ItemOffset.Y * scale.Y),
                (int)(FrameSize.X * scale.X),
                (int)(FrameSize.Y * scale.Y));

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
        }
    }
}

