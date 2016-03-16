using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Xml.Serialization;
using Engine.Screens;

namespace Engine.Manager
{
    public class ScreenManager
    {
        [XmlIgnore]
        public Vector2 Dimensions { get; private set; }
        [XmlIgnore]
        public ContentManager Content { get; private set; }

        private readonly XmlManager<GameScreen> _xmlGameScreenManager;

        private GameScreen _currentScreen, _newScreen;
        [XmlIgnore]
        public GraphicsDevice GraphicsDevice;
        [XmlIgnore]
        public SpriteBatch SpriteBatch;

        public Image Image;

        [XmlIgnore]
        public bool IsTransitioning { get; private set; }

        private static ScreenManager _instance;
        public static ScreenManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var xml = new XmlManager<ScreenManager>();
                    _instance = xml.Load(Constants.ContentFolder + "/ScreenManager");
                }

                return _instance;
            }
        }

        public void ChangeScreens(string screenName)
        {
            _newScreen = (GameScreen)Activator.CreateInstance(Type.GetType(Constants.DefaultNamespace + "." + screenName));
            Image.IsActive = true;
            Image.FadeEffect.Increase = true;
            Image.Alpha = 0.0f;
            IsTransitioning = true;
        }

        private void Transition(GameTime gameTime)
        {
            if (IsTransitioning)
            {
                Image.Update(gameTime);
                if (Image.Alpha == 1.0f)
                {
                    _currentScreen.UnloadContent();
                    _currentScreen = _newScreen;
                    _xmlGameScreenManager.Type = _currentScreen.Type;
                    if (File.Exists(_currentScreen.XmlPath))
                        _currentScreen = _xmlGameScreenManager.Load(_currentScreen.XmlPath);
                    _currentScreen.LoadContent();
                }
                else if (Image.Alpha == 0.0f)
                {
                    Image.IsActive = false;
                    IsTransitioning = false;
                }
            }
        }

        public ScreenManager()
        {
            Image = new Image();
            Dimensions = new Vector2(640, 480);
            _currentScreen = new GameplayScreen();
            //_currentScreen = new SplashScreen();
            _xmlGameScreenManager = new XmlManager<GameScreen> { Type = _currentScreen.Type };
            //_currentScreen = _xmlGameScreenManager.Load(Constants.ContentFolder + "/SplashScreen.xml");
        }

        public void LoadContent(ContentManager content)
        {
            Content = new ContentManager(content.ServiceProvider, "Content");
            _currentScreen.LoadContent();
            Image.LoadContent();
        }

        public void UnloadContent()
        {
            _currentScreen.UnloadContent();
            Image.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            _currentScreen.Update(gameTime);
            Transition(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _currentScreen.Draw(spriteBatch);

            if (IsTransitioning)
                Image.Draw(spriteBatch);
        }
    }
}
