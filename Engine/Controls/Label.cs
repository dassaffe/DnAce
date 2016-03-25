using Engine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Controls
{
    /// <summary>
    /// Textlabel
    /// </summary>
    internal class Label : Control
    {
        /// <summary>
        /// Ausgabetext
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Ausgabeschriftart
        /// </summary>
        public SpriteFont Font { get; set; }

        /// <summary>
        /// Textfarbe
        /// </summary>
        public Color Color { get; set; }

        public Label(ScreenComponent manager)
            : base(manager)
        {
            Font = manager.Font;
            Color = Color.White;
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            // Leerer Text wird ignoriert
            if (string.IsNullOrEmpty(Text))
                return;

            // Ohne Schriftart keine Schrift
            if (Font == null)
                return;

            spriteBatch.DrawString(Font, Text, new Vector2(offset.X + Position.X, offset.Y + Position.Y), Color);
        }
    }
}

