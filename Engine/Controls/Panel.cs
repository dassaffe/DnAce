using Engine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Controls
{
    internal class Panel : Control
    {
        public Panel(ScreenComponent manager) : base(manager)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Point offset)
        {
            Manager.Border.Draw(spriteBatch,
                new Rectangle(
                    Position.X + offset.X,
                    Position.Y + offset.Y,
                    Position.Width,
                    Position.Height));
        }
    }
}