using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.GameElements;
using Engine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Screens
{
    public class GameplayScreen : GameScreen
    {
        private Player _player;
        private Map _map;

        public override void LoadContent()
        {
            base.LoadContent();

            var playerLoader = new XmlManager<Player>();
            var mapLoader = new XmlManager<Map>();
            _player = playerLoader.Load(Constants.ContentFolder + "/Gameplay/Player.ply");
            _map = mapLoader.Load(Constants.ContentFolder + "/Gameplay/Maps/Map2.map");
            _player.LoadContent();
            _map.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            _player.UnloadContent();
            _map.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _player.Update(gameTime);
            _map.Update(ref _player);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            _map.Draw(spriteBatch, DrawType.UnderLay);
            _player.Draw(spriteBatch);
            _map.Draw(spriteBatch, DrawType.OverLay);
        }
    }
}
