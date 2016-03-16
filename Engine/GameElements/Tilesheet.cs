using System;
using System.Xml.Serialization;
using Engine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.GameElements
{
    public class Tilesheet
    {
        [XmlIgnore]
        public float Alpha;
        [XmlIgnore]
        public Texture2D AutoTexture;
        public string Path;
        [XmlIgnore]
        public Vector2 Position;
        [XmlIgnore]
        public Vector2 Scale;
        [XmlIgnore]
        public Rectangle SourceRect;
        [XmlIgnore]
        public Texture2D Texture;

        private ContentManager _content;
        private Vector2 _origin;
        private RenderTarget2D _renderTarget;

        public Tilesheet()
        {
            Path = String.Empty;
            SourceRect = Rectangle.Empty;
            Position = Vector2.Zero;
            Alpha = 1.0f;
            Scale = Vector2.One;
        }

        public void LoadContent(TileType tileType)
        {
            _content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");

            if (!String.IsNullOrEmpty(Path))
            {
                if (tileType == TileType.Map)
                    Texture = _content.Load<Texture2D>(Path);
                else if (tileType == TileType.Autotile)
                    AutoTexture = _content.Load<Texture2D>(Path);
            }

            Vector2 dimensions = Vector2.Zero;

            if (Texture != null)
            {
                dimensions.X += Texture.Width;
                dimensions.Y += Texture.Height;
            }
            else if (AutoTexture != null)
            {
                dimensions.X += AutoTexture.Width;
                dimensions.Y += AutoTexture.Height;
            }

            if (SourceRect == Rectangle.Empty)
                SourceRect = new Rectangle(0, 0, (int)dimensions.X, (int)dimensions.Y);

            _renderTarget = new RenderTarget2D(ScreenManager.Instance.GraphicsDevice, (int)dimensions.X,
                (int)dimensions.Y);
            ScreenManager.Instance.GraphicsDevice.SetRenderTarget(_renderTarget);
            ScreenManager.Instance.GraphicsDevice.Clear(Color.Transparent);
            ScreenManager.Instance.SpriteBatch.Begin();
            if (Texture != null)
                ScreenManager.Instance.SpriteBatch.Draw(Texture, Vector2.Zero, Color.White);
            else if (AutoTexture != null)
                ScreenManager.Instance.SpriteBatch.Draw(AutoTexture, Vector2.Zero, Color.White);
            ScreenManager.Instance.SpriteBatch.End();

            if (tileType == TileType.Map)
                Texture = _renderTarget;
            else if (tileType == TileType.Autotile)
                AutoTexture = _renderTarget;

            ScreenManager.Instance.GraphicsDevice.SetRenderTarget(null);
        }

        public void UnloadContent()
        {
            if (_content != null)
                _content.Unload();
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
            if (Texture != null)
            {
                spriteBatch.Draw(Texture, Position + _origin, SourceRect, Color.White * Alpha, 0.0f, _origin, Scale,
                   SpriteEffects.None, 0.0f);
            }
            else if (AutoTexture != null)
            {
                spriteBatch.Draw(AutoTexture, Position + _origin, SourceRect, Color.White * Alpha, 0.0f, _origin, Scale,
                    SpriteEffects.None, 0.0f);
            }
        }
    }
}