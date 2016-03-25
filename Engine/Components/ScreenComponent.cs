using System.Collections.Generic;
using System.Diagnostics;
using Engine.Controls;
using Engine.Model;
using Engine.Rendering;
using Engine.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Components
{
    /// <summary>
    /// Komponente zur Verwaltung von Screen-Overlays.
    /// </summary>
    internal class ScreenComponent : DrawableGameComponent
    {
        private readonly Stack<Screen> _screens;

        private SpriteBatch _spriteBatch;

        #region Shared Resources

        /// <summary>
        /// Ein einzelner Pixel
        /// </summary>
        public Texture2D Pixel { get; private set; }

        /// <summary>
        /// Standard Auswahlpfeil
        /// </summary>
        /// <value>The arrow.</value>
        public Texture2D Arrow { get; private set; }

        /// <summary>
        /// Standard-Schriftart für Dialoge
        /// </summary>
        public SpriteFont Font { get; private set; }

        /// <summary>
        /// Standard Hintergrund für Panels
        /// </summary>
        public NineTileRenderer Panel { get; private set; }

        /// <summary>
        /// Standard Hintergrund für Buttons
        /// </summary>
        public NineTileRenderer Button { get; private set; }

        /// <summary>
        /// Standard Hintergrund für selektierte Buttons
        /// </summary>
        public NineTileRenderer ButtonHovered { get; private set; }

        /// <summary>
        /// Standard Hintergrund für einen Rahmen
        /// </summary>
        public NineTileRenderer Border { get; private set; }

        /// <summary>
        /// Dictionary von Item Icons
        /// </summary>
        public Dictionary<string, Texture2D> Icons { get; private set; }

        #endregion

        /// <summary>
        /// Liefert den aktuellen Screen oder null zurück.
        /// </summary>
        public Screen ActiveScreen
        {
            get { return _screens.Count > 0 ? _screens.Peek() : null; }
        }

        /// <summary>
        /// Referenz auf das Game (Überschrieben mit spezialisiertem Type)
        /// </summary>
        public new Engine GameEngine { get; private set; }

        public ScreenComponent(Engine gameEngine)
            : base(gameEngine)
        {
            GameEngine = gameEngine;
            _screens = new Stack<Screen>();
            Icons = new Dictionary<string, Texture2D>();
        }

        /// <summary>
        /// Zeigt den übergebenen Screen an.
        /// </summary>
        public void ShowScreen(Screen screen)
        {
            _screens.Push(screen);
            screen.OnShow();
        }

        /// <summary>
        /// Entfernt den obersten Screen.
        /// </summary>
        public void CloseScreen()
        {
            if (_screens.Count > 0)
            {
                var screen = _screens.Pop();
                screen.OnHide();
            }
        }

        protected override void LoadContent()
        {
            // Sprite Batch erstellen
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Standard Pixel erstellen
            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });

            // Schriftart laden
            //Font = GameEngine.Content.Load<SpriteFont>("Fonts/VCROSDMono");
            Font = GameEngine.Content.Load<SpriteFont>("Fonts/AdvoCut");
            //var glyphs = Font.MeasureString("WASD08/15");
            //Debug.Write(glyphs);

            // Hintergründe laden
            Texture2D texture = Game.Content.Load<Texture2D>("UI/ui");
            Panel = new NineTileRenderer(texture, new Rectangle(190, 100, 100, 100), new Point(30, 30));
            Border = new NineTileRenderer(texture, new Rectangle(283, 200, 93, 94), new Point(30, 30));
            Button = new NineTileRenderer(texture, new Rectangle(0, 282, 190, 49), new Point(10, 10));
            ButtonHovered = new NineTileRenderer(texture, new Rectangle(0, 143, 190, 45), new Point(10, 10));

            // Arrow
            Rectangle source = new Rectangle(325, 486, 22, 21);
            Color[] buffer = new Color[source.Width * source.Height];
            texture.GetData(0, source, buffer, 0, buffer.Length);
            Arrow = new Texture2D(GraphicsDevice, source.Width, source.Height);
            Arrow.SetData(buffer);

            // Icon-Texturen sammeln
            List<string> requiredIconTextures = new List<string>();
            foreach (Area area in GameEngine.Simulation.World.Areas)
            {
                foreach (Item item in area.Items)
                {
                    if (!string.IsNullOrEmpty(item.Icon) && !requiredIconTextures.Contains(item.Icon))
                    {
                        requiredIconTextures.Add(item.Icon);
                    }
                }
            }
            foreach (Area area in GameEngine.Simulation.World.Areas)
            {
                foreach (Sprite sprite in area.Sprites)
                {
                    if (!string.IsNullOrEmpty(sprite.Icon) && !requiredIconTextures.Contains(sprite.Icon))
                    {
                        requiredIconTextures.Add(sprite.Icon);
                    }
                }
            }

            // Erforderliche Icon-Texturen laden
            foreach (string textureName in requiredIconTextures)
            {
                Icons.Add(textureName, GameEngine.Content.Load<Texture2D>("Icons/" + textureName));
            }
        }

        public override void Update(GameTime gameTime)
        {
            Screen activeScreen = ActiveScreen;
            if (activeScreen != null)
            {
                foreach (Control control in activeScreen.Controls)
                    control.Update(gameTime);
                activeScreen.Update(gameTime);
                GameEngine.Input.Handled = true;
            }

            // Spezialtasten prüfen
            if (!GameEngine.Input.Handled)
            {
                if (GameEngine.Input.Close)
                {
                    ShowScreen(new MainMenuScreen(this));
                    GameEngine.Input.Handled = true;
                }
            }
            if (!GameEngine.Input.Handled)
            {
                if (GameEngine.Input.Inventory)
                {
                    ShowScreen(new InventoryScreen(this));
                    GameEngine.Input.Handled = true;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin(samplerState: SamplerState.LinearWrap);
            foreach (Screen screen in _screens)
            {
                screen.Draw(gameTime, _spriteBatch);
            }
            _spriteBatch.End();
        }
    }
}