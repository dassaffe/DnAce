using Engine.Components;
using Engine.Controls;
using Engine.Model;
using Microsoft.Xna.Framework;

namespace Engine.Screens
{
    internal class ShoutScreen : Screen
    {
        public ShoutScreen(ScreenComponent manager, Character speaker, string message)
            : base(manager)
        {
            Position = new Rectangle(10, manager.GraphicsDevice.Viewport.Height - 54, manager.GraphicsDevice.Viewport.Width - 20, 44);
            Controls.Add(new Icon(manager) { Position = new Rectangle(10, 10, 24, 24), Texture = speaker.Icon });
            Controls.Add(new Label(manager) { Text = message, Position = new Rectangle(45, 12, Position.Width - 20, Position.Height - 20) });
        }

        public override void Update(GameTime gameTime)
        {
            if (!Manager.GameEngine.Input.Handled)
            {
                if (Manager.GameEngine.Input.Close | Manager.GameEngine.Input.Interact)
                {
                    Manager.CloseScreen();
                    Manager.GameEngine.Input.Handled = true;
                }
            }
        }
    }
}