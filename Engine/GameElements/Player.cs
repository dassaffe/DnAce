using Engine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Engine.GameElements
{
    public class Player
    {
        public Image Image;
        public Vector2 Velocity;
        public float MoveSpeed;

        public Player()
        {
            Image = new Image();
            Velocity = Vector2.Zero;
            MoveSpeed = 100;
        }

        public void LoadContent()
        {
            Image.LoadContent();
        }

        public void UnloadContent()
        {
            Image.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            Image.IsActive = true;
            if (InputManager.Instance.KeyDown(Keys.Up))
            {
                Velocity.X = 0;
                Velocity.Y = -MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Image.SpriteSheetEffect.CurrentFrame.Y = 3;
            }
            else if (InputManager.Instance.KeyDown(Keys.Down))
            {
                Velocity.X = 0;
                Velocity.Y = MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Image.SpriteSheetEffect.CurrentFrame.Y = 0;
            }
            else if (InputManager.Instance.KeyDown(Keys.Left))
            {
                Velocity.X = -MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Velocity.Y = 0;
                Image.SpriteSheetEffect.CurrentFrame.Y = 1;
            }
            else if (InputManager.Instance.KeyDown(Keys.Right))
            {
                Velocity.X = MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Velocity.Y = 0;
                Image.SpriteSheetEffect.CurrentFrame.Y = 2;
            }
            else
            {
                Velocity.X = 0;
                Velocity.Y = 0;
            }

            if ((int)Velocity.X == 0 && (int)Velocity.Y == 0)
                Image.IsActive = false;

            Image.Update(gameTime);
            Image.Position += Velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }
    }
}