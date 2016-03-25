using Engine.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering
{
    /// <summary>
    /// Sprite Renderer für simple, fortlaufende Animationen.
    /// </summary>
    internal class SimpleSpriteRenderer : SpriteRenderer
    {
        /// <summary>
        /// Anzahl Frames in der Animation
        /// </summary>
        private readonly int _frameCount;

        public SimpleSpriteRenderer(Sprite sprite, Camera camera, Texture2D texture, SpriteFont font)
            : base(sprite, camera, texture, font, new Point(32, 32), 200, new Point(16, 26), 1f)
        {
            _frameCount = 8;
        }

        /// <summary>
        /// Render-Methode für dieses Sprite.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch Referenz</param>
        /// <param name="offset">Der Offset der View</param>
        /// <param name="gameTime">Aktuelle Game Time</param>
        public override void Draw(SpriteBatch spriteBatch, Point offset, GameTime gameTime, bool highlight)
        {
            // Animationszeit neu berechnen (vergangene Millisekunden zum letzten Frame addieren)
            AnimationTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Ermittlung des aktuellen Frames
            int frame = (AnimationTime / FrameTime) % _frameCount;

            // Bestimmung der Position des Spieler-Mittelpunktes in View-Koordinaten
            int posX = (int)((Sprite.Position.X) * Camera.Scale) - offset.X;
            int posY = (int)((Sprite.Position.Y) * Camera.Scale) - offset.Y;
            //int radius = (int)(Sprite.Radius * Camera.Scale);

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

