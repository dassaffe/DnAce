using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Engine.Components
{
    class MouseComponent : DrawableGameComponent
    {
        private Engine _gameEngine;
        private SpriteBatch _spriteBatch;
        private Texture2D _mouseCursor;

        public MouseComponent(Engine gameEngine) : base(gameEngine)
        {
            _gameEngine = gameEngine;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _mouseCursor = Game.Content.Load<Texture2D>("UI/ui");
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            MouseState ms = Mouse.GetState();

            _spriteBatch.Draw(_mouseCursor, new Vector2(ms.Position.X, ms.Position.Y), new Rectangle(144, 482, 27, 25), Color.White);

            _spriteBatch.End();
        }
    }
}