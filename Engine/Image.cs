using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Engine.Effects;
using Engine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine
{
    public class Image
    {
        private readonly Dictionary<string, ImageEffect> _effectList;
        [XmlIgnore]
        public float Alpha;
        public string Effects;

        public FadeEffect FadeEffect;
        public string FontName;
        [XmlIgnore]
        public bool IsActive;
        public string Path;
        [XmlIgnore]
        public Vector2 Position;
        [XmlIgnore]
        public Vector2 Scale;
        [XmlIgnore]
        public Vector2 Size;
        [XmlIgnore]
        public Rectangle SourceRect;
        public SpriteSheetEffect SpriteSheetEffect;
        public string Text;
        [XmlIgnore]
        public Texture2D Texture;
        private ContentManager _content;
        private SpriteFont _font;
        private Vector2 _origin;
        private RenderTarget2D _renderTarget;

        public Image()
        {
            Path = String.Empty;
            Effects = String.Empty;
            FontName = "Fonts/Arial";
            Text = String.Empty;
            Position = Vector2.Zero;
            Scale = Vector2.One;
            Alpha = 1.0f;
            SourceRect = Rectangle.Empty;
            _effectList = new Dictionary<string, ImageEffect>();
        }

        private void SetEffect<T>(ref T effect)
        {
            if (effect == null)
            {
                effect = (T)Activator.CreateInstance(typeof(T));
            }
            else
            {
                (effect as ImageEffect).IsActive = true;
                Image obj = this;
                (effect as ImageEffect).LoadContent(ref obj);
            }

            _effectList.Add(effect.GetType().ToString().Replace(Constants.DefaultNamespace + ".", ""),
                (effect as ImageEffect));
        }

        public void ActivateEffect(string effect)
        {
            if (_effectList.ContainsKey(effect))
            {
                _effectList[effect].IsActive = true;
                Image obj = this;
                _effectList[effect].LoadContent(ref obj);
            }
        }

        public void DeactivateEffect(string effect)
        {
            if (_effectList.ContainsKey(effect))
            {
                _effectList[effect].IsActive = false;
                _effectList[effect].UnloadContent();
            }
        }

        public void StoreEffects()
        {
            Effects = String.Empty;
            foreach (var effect in _effectList)
            {
                if (effect.Value.IsActive)
                    Effects += effect.Key + ":";
            }

            if (!String.IsNullOrEmpty(Effects))
                Effects.Remove((Effects.Length - 1));
        }

        public void RestoreEffects()
        {
            foreach (var effect in _effectList)
                DeactivateEffect(effect.Key);

            string[] split = Effects.Split(':');

            foreach (string s in split)
            {
                ActivateEffect(s);
            }
        }

        public void LoadContent()
        {
            _content = new ContentManager(ScreenManager.Instance.Content.ServiceProvider, "Content");

            if (!String.IsNullOrEmpty(Path))
                Texture = _content.Load<Texture2D>(Path);

            _font = _content.Load<SpriteFont>(FontName);

            Vector2 dimensions = Vector2.Zero;

            if (Texture != null)
                dimensions.X += Texture.Width;
            dimensions.X += _font.MeasureString(Text).X;

            dimensions.Y = Texture != null
                ? Math.Max(Texture.Height, _font.MeasureString(Text).Y)
                : _font.MeasureString(Text).Y;

            Size = dimensions;

            if (SourceRect == Rectangle.Empty)
                SourceRect = new Rectangle(0, 0, (int)dimensions.X, (int)dimensions.Y);

            _renderTarget = new RenderTarget2D(ScreenManager.Instance.GraphicsDevice, (int)dimensions.X,
                (int)dimensions.Y);
            ScreenManager.Instance.GraphicsDevice.SetRenderTarget(_renderTarget);
            ScreenManager.Instance.GraphicsDevice.Clear(Color.Transparent);
            ScreenManager.Instance.SpriteBatch.Begin();
            if (Texture != null)
                ScreenManager.Instance.SpriteBatch.Draw(Texture, Vector2.Zero, Color.White);
            //ScreenManager.Instance.SpriteBatch.DrawString(_font, Text, Vector2.Zero, Color.White);
            ScreenManager.Instance.SpriteBatch.End();

            Texture = _renderTarget;

            ScreenManager.Instance.GraphicsDevice.SetRenderTarget(null);

            SetEffect(ref FadeEffect);
            SetEffect(ref SpriteSheetEffect);

            if (!String.IsNullOrEmpty(Effects))
            {
                string[] split = Effects.Split(':');
                foreach (string item in split)
                {
                    ActivateEffect(item);
                }
            }
        }

        public void UnloadContent()
        {
            _content.Unload();
            foreach (var effect in _effectList)
            {
                DeactivateEffect(effect.Key);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var effect in _effectList)
            {
                if (effect.Value.IsActive)
                    effect.Value.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
            spriteBatch.Draw(Texture, Position + _origin, SourceRect, Color.White * Alpha, 0.0f, _origin, Scale,
                SpriteEffects.None, 0.0f);
        }
    }
}