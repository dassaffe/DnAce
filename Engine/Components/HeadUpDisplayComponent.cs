using System.Linq;
using Engine.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Components
{
    internal class HeadUpDisplayComponent : DrawableGameComponent
    {
        private readonly Engine _gameEngine;
        private SpriteBatch _spriteBatch;
        private SpriteFont _pixelFont;
        private Texture2D _hearts;
        private Texture2D _coin;

        public HeadUpDisplayComponent(Engine gameEngine) : base(gameEngine)
        {
            _gameEngine = gameEngine;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //pixelFont = Game.Content.Load<SpriteFont>("Fonts/KenPixel");
            _pixelFont = Game.Content.Load<SpriteFont>("Fonts/AdvoCut");
            //pixelFont = Game.Content.Load<SpriteFont>("Fonts/VCROSDMono");
            _hearts = Game.Content.Load<Texture2D>("UI/hearts");
            _coin = Game.Content.Load<Texture2D>("Icons/coinicon");
        }

        public override void Draw(GameTime gameTime)
        {
            // Nur wenn Komponente sichtbar ist.
            if (!Visible)
                return;

            // Nur arbeiten, wenn es eine Welt, einen Player und eine aktive Area gibt.
            Area area = _gameEngine.Local.GetCurrentArea();
            if (_gameEngine.Simulation.World == null || _gameEngine.Local.Player == null || area == null)
                return;

            _spriteBatch.Begin();

            Vector2 position = _gameEngine.Local.Player.Position;
            string debugText = string.Format("{0} ({1:0}/{2:0})", area.Name, position.X, position.Y);

            // Ausgabe der ersten Debug-Info
            _spriteBatch.DrawString(_pixelFont, debugText, new Vector2(10, 10), Color.White);

            // Herzen ausgeben
            int totalHearts = _gameEngine.Local.Player.MaxHitpoints;
            int filledHearts = _gameEngine.Local.Player.Hitpoints;
            int offset = GraphicsDevice.Viewport.Width - (totalHearts * 34) - 10;
            for (int i = 0; i < totalHearts; i++)
            {
                Rectangle source = new Rectangle(0, (filledHearts > i ? 0 : 67), 32, 32);
                Rectangle destination = new Rectangle(offset + (i * 34), 10, 32, 32);

                _spriteBatch.Draw(_hearts, destination, source, Color.White);
            }

            // Coins ausgeben
            string coins = _gameEngine.Local.Player.Inventory.Count(i => i.Name.Equals("Münze")).ToString();
            _spriteBatch.Draw(_coin, new Rectangle(GraphicsDevice.Viewport.Width - 34, 49, 24, 24), Color.White);
            int coinSize = (int)_pixelFont.MeasureString(coins).X;
            _spriteBatch.DrawString(_pixelFont, coins, new Vector2(GraphicsDevice.Viewport.Width - coinSize - 35, 50), Color.White);

            // Quest anzeigen
            Quest quest = _gameEngine.Simulation.World.Quests.FirstOrDefault(q => q.State != QuestState.Inactive);
            if (quest != null)
            {
                _spriteBatch.DrawString(_pixelFont, quest.Name, new Vector2(10, 40), Color.White);
                _spriteBatch.DrawString(_pixelFont, quest.CurrentProgress.Description, new Vector2(10, 60), Color.White);
            }

            _spriteBatch.End();
        }
    }
}
