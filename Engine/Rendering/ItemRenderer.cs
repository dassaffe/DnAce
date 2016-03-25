﻿using Engine.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering
{
    /// <summary>
    /// Render-Container für einzelne Spiele-Items.
    /// </summary>
    internal abstract class ItemRenderer
    {
        /// <summary>
        /// Referenz auf das Item
        /// </summary>
        protected Item Item { get; private set; }

        /// <summary>
        /// Referenz auf die Kamera
        /// </summary>
        protected Camera Camera { get; private set; }

        /// <summary>
        /// Referenz auf die zu verwendende Textur
        /// </summary>
        protected Texture2D Texture { get; private set; }

        /// <summary>
        /// Größenangabe eines Frames in Pixel
        /// </summary>
        protected Point FrameSize { get; private set; }

        /// <summary>
        /// Anzahl Millisekunden pro Frame
        /// </summary>
        protected int FrameTime { get; private set; }

        /// <summary>
        /// Item-Mittelpunkt in Pixel
        /// </summary>
        protected Point ItemOffset { get; private set; }

        /// <summary>
        /// Skalierungsfaktor beim rendern
        /// </summary>
        protected float FrameScale { get; private set; }

        /// <summary>
        /// Vergangene Animationszeit in Millisekunden
        /// </summary>
        protected int AnimationTime { get; set; }

        /// <summary>
        /// Initialisierung des Item Renderers
        /// </summary>
        /// <param name="item">Item Referenz</param>
        /// <param name="camera">Kamera Referenz</param>
        /// <param name="texture">Textur Referenz</param>
        /// <param name="frameSize">Größe eines Frames in Pixel</param>
        /// <param name="frameTime">Anzahl Millisekunden pro Frame</param>
        /// <param name="itemOffset">Mittelpunkt des Items innerhalb des Frames</param>
        /// <param name="frameScale">Skalierung</param>
        public ItemRenderer(Item item, Camera camera, Texture2D texture, Point frameSize, int frameTime, Point itemOffset, float frameScale)
        {
            Item = item;
            Camera = camera;
            Texture = texture;
            FrameSize = frameSize;
            FrameTime = frameTime;
            ItemOffset = itemOffset;
            FrameScale = frameScale;
        }

        /// <summary>
        /// Render-Methode für dieses Item.
        /// </summary>
        /// <param name="spriteBatch">ItemBatch Referenz</param>
        /// <param name="offset">Der Offset der View</param>
        /// <param name="gameTime">Aktuelle Game Time</param>
        public abstract void Draw(SpriteBatch spriteBatch, Point offset, GameTime gameTime);
    }
}