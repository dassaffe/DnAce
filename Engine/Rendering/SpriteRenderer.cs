using Engine.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering
{
    /// <summary>
    /// Render-Container für einzelne Spiele-Sprites.
    /// </summary>
    internal abstract class SpriteRenderer
    {
        /// <summary>
        /// Referenz auf das Sprite
        /// </summary>
        protected Sprite Sprite { get; private set; }

        /// <summary>
        /// Referenz auf die Kamera
        /// </summary>
        protected Camera Camera { get; private set; }

        /// <summary>
        /// Referenz auf die zu verwendende Textur
        /// </summary>
        protected Texture2D Texture { get; private set; }

        /// <summary>
        /// Referenz auf eine Sprite Font.
        /// </summary>
        /// <value>The font.</value>
        protected SpriteFont Font { get; private set; }

        /// <summary>
        /// Größenangabe eines Frames in Pixel
        /// </summary>
        protected Point FrameSize { get; private set; }

        /// <summary>
        /// Anzahl Millisekunden pro Frame
        /// </summary>
        protected int FrameTime { get; private set; }

        /// <summary>
        /// Sprite-Mittelpunkt in Pixel
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
        /// Initialisierung des Sprite Renderers
        /// </summary>
        /// <param name="sprite">Sprite Referenz</param>
        /// <param name="camera">Kamera Referenz</param>
        /// <param name="texture">Textur Referenz</param>
        /// <param name="frameSize">Größe eines Frames in Pixel</param>
        /// <param name="frameTime">Anzahl Millisekunden pro Frame</param>
        /// <param name="itemOffset">Mittelpunkt des Sprites innerhalb des Frames</param>
        /// <param name="frameScale">Skalierung</param>
        public SpriteRenderer(Sprite sprite, Camera camera, Texture2D texture, SpriteFont spriteFont, Point frameSize, int frameTime, Point itemOffset, float frameScale)
        {
            Sprite = sprite;
            Camera = camera;
            Texture = texture;
            Font = spriteFont;
            FrameSize = frameSize;
            FrameTime = frameTime;
            ItemOffset = itemOffset;
            FrameScale = frameScale;
        }

        /// <summary>
        /// Render-Methode für dieses Sprite.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch Referenz</param>
        /// <param name="offset">Der Offset der View</param>
        /// <param name="gameTime">Aktuelle Game Time</param>
        public abstract void Draw(SpriteBatch spriteBatch, Point offset, GameTime gameTime, bool highlight);
    }
}