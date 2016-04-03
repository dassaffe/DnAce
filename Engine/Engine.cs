using System;
using Engine.Components;
using Microsoft.Xna.Framework;

namespace Engine
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class Engine : Game
    {
        private HeadUpDisplayComponent Hud { get; }
        internal InputComponent Input { get; }
        internal ScreenComponent Screen { get; }
        internal SimulationComponent Simulation { get; }
        private SceneComponent Scene { get; }
        internal MusicComponent Music { get; }
        internal SoundComponent Sound { get; }
        private MouseComponent Mouse { get; }
        internal LocalComponent Local { get; set; }

        int _frameRate;
        int _frameCounter;
        TimeSpan _elapsedTime = TimeSpan.Zero;

        public Engine()
        {
            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            //graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = false;

            Input = new InputComponent(this) { UpdateOrder = 0 };
            Components.Add(Input);

            Screen = new ScreenComponent(this) { UpdateOrder = 1, DrawOrder = 2 };
            Components.Add(Screen);

            Simulation = new SimulationComponent(this) { UpdateOrder = 3 };
            Components.Add(Simulation);

            Scene = new SceneComponent(this) { UpdateOrder = 4, DrawOrder = 0 };
            Components.Add(Scene);

            Hud = new HeadUpDisplayComponent(this) { UpdateOrder = 5, DrawOrder = 1 };
            Components.Add(Hud);

            Music = new MusicComponent(this) { UpdateOrder = 6 };
            Components.Add(Music);

            Sound = new SoundComponent(this) { UpdateOrder = 7 };
            Components.Add(Sound);

            Local = new LocalComponent(this) { UpdateOrder = 2 };
            Components.Add(Local);




            Mouse = new MouseComponent(this) { UpdateOrder = 999, DrawOrder = 999 };
            Components.Add(Mouse);
        }

        ///// <summary>
        /////     Allows the game to perform any initialization it needs to before starting to run.
        /////     This is where it can query for any required services and load any non-graphic
        /////     related content.  Calling base.Initialize will enumerate through any components
        /////     and initialize them as well.
        ///// </summary>
        //protected override void Initialize()
        //{
        //    base.Initialize();
        //}
        ///// <summary>
        /////     LoadContent will be called once per game and is the place to load
        /////     all of your content.
        ///// </summary>
        //protected override void LoadContent()
        //{
        //}
        ///// <summary>
        /////     UnloadContent will be called once per game and is the place to unload
        /////     game-specific content.
        ///// </summary>
        //protected override void UnloadContent()
        //{
        //}

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCounter;
                _frameCounter = 0;
            }

            if (Window != null)
                Window.Title = string.Format("{0} / FPS: {1}", "DnAce", _frameRate);

            base.Update(gameTime);
        }

        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _frameCounter++;

            base.Draw(gameTime);
        }
    }
}