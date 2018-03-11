using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DnAce.Engine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class DnAce : Game
    {
        readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _splashScreenImage;
        private Texture2D _mouseCursor;
        SpriteFont _font1;
        Vector2 _fontPos;
        //private BasicEffect basicEffect;

        public DnAce()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //_graphics.IsFullScreen = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _font1 = Content.Load<SpriteFont>("Courier New");

            Song bgm = Content.Load<Song>("bgm/March");
            _splashScreenImage = Content.Load<Texture2D>("splash/cover");
            _mouseCursor = Content.Load<Texture2D>("sprites/mouse");

            MediaPlayer.Play(bgm);
            // TODO: use this.Content to load your game content here
            _fontPos = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2, _graphics.GraphicsDevice.Viewport.Height / 2);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Immediate);

            
            // Draw Hello World
            string output = "Hello World";

            // Find the center of the string
            Vector2 fontOrigin = _font1.MeasureString(output) / 2;
            // Draw the string
            _spriteBatch.DrawString(_font1, output, _fontPos, Color.LightGreen, 0, fontOrigin, 1.0f, SpriteEffects.None, 0.5f);


             _spriteBatch.Draw(_splashScreenImage, new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), Color.White);

            // mouse cursor
            var x = Mouse.GetState().X;
            var y = Mouse.GetState().Y;

            _spriteBatch.Draw(_mouseCursor, new Vector2(x, y), new Rectangle(0, 0, 32, 32), Color.AliceBlue);


            _spriteBatch.End();
            base.Draw(gameTime);
        }


    }
}
