using System.Collections.Generic;
using Engine.Components;
using Engine.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Screens
{
    /// <summary>
    /// Abstrakte Basisklasse für alle Screens.
    /// </summary>
    internal abstract class Screen
    {
        /// <summary>
        /// Auflistung aller enthaltener Controls.
        /// </summary>
        public List<Control> Controls { get; }

        /// <summary>
        /// Position und Größe des Screens.
        /// </summary>
        public Rectangle Position { get; set; }

        /// <summary>
        /// Referenz auf den Screen Manager.
        /// </summary>
        protected ScreenComponent Manager { get; }

        public Screen(ScreenComponent manager)
        {
            Manager = manager;
            Controls = new List<Control>();
        }

        public Screen(ScreenComponent manager, Point size) : this(manager)
        {
            Point pos = new Point(
                (manager.GraphicsDevice.Viewport.Width - size.X) / 2,
                (manager.GraphicsDevice.Viewport.Height - size.Y) / 2);

            Position = new Rectangle(pos, size);
        }

        /// <summary>
        /// Wird aufgerufen, sobald der Screen in die Renderauflistung kommt
        /// </summary>
        public virtual void OnShow() { }

        /// <summary>
        /// Wird aufgerufen, sobald der Screen aus der Renderauflistung entfernt wird
        /// </summary>
        public virtual void OnHide() { }

        public abstract void Update(GameTime gameTime);

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Manager.Panel.Draw(spriteBatch, Position);
            // spriteBatch.Draw(Manager.Pixel, Position, Color.DarkBlue);
            foreach (Control control in Controls)
                control.Draw(spriteBatch, Position.Location);
        }
    }
}

