using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Interface;
using Engine.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Engine.Components
{
    internal class SoundComponent : GameComponent
    {
        private Engine _gameEngine;

        private Dictionary<string, SoundEffect> _sounds;

        private float _volume;

        // Player Referenz
        private Player player;

        // Referenz auf die sichtbare Area.
        private Area area;

        // Anzahl Münzen
        private int coins;

        // Mapping von Angreifern zu deren Recovery Times.
        private Dictionary<IAttacker, TimeSpan> recoveryTimes;

        // Mapping von Angreifbaren Items zu Hitpoints.
        private Dictionary<IAttackable, int> hitpoints;

        public SoundComponent(Engine gameEngine) : base(gameEngine)
        {
            _gameEngine = gameEngine;
            _volume = 0.5f;

            _sounds = new Dictionary<string, SoundEffect>
            {
                {"click", gameEngine.Content.Load<SoundEffect>("Sounds/click")},
                {"clock", gameEngine.Content.Load<SoundEffect>("Sounds/clock")},
                {"coin", gameEngine.Content.Load<SoundEffect>("Sounds/coin")},
                {"hit", gameEngine.Content.Load<SoundEffect>("Sounds/hit")},
                {"sword", gameEngine.Content.Load<SoundEffect>("Sounds/sword")}
            };

            recoveryTimes = new Dictionary<IAttacker, TimeSpan>();
            hitpoints = new Dictionary<IAttackable, int>();
        }

        public override void Update(GameTime gameTime)
        {
            // Nur wenn Komponente aktiviert wurde.
            if (!Enabled)
                return;

            // Nur arbeiten, wenn es eine Welt, einen Player und eine aktive Area gibt.
            Area nextArea = _gameEngine.Local.GetCurrentArea();
            if (_gameEngine.Simulation.World == null || _gameEngine.Local.Player == null || nextArea == null)
                return;

            // Reset aller Variablen falls sich der Player ändert
            if (player != _gameEngine.Local.Player)
            {
                player = _gameEngine.Local.Player;
                coins = player.Inventory.Count(i => i is Coin);
                recoveryTimes.Clear();
            }

            // Reset der Item variablen falls sich die Area ändert
            if (area != nextArea)
            {
                area = nextArea;

                // Recovery Times
                recoveryTimes.Clear();
                foreach (IAttacker item in area.Items.OfType<IAttacker>())
                    recoveryTimes.Add(item, item.Recovery);

                // Hitpoints
                hitpoints.Clear();
                foreach (IAttackable item in area.Items.OfType<IAttackable>())
                    hitpoints.Add(item, item.Hitpoints);
            }

            // Coins
            int c = player.Inventory.Count(i => i is Coin);
            if (coins < c)
                PlaySound("coin");
            coins = c;

            // Sword
            foreach (IAttacker item in area.Items.OfType<IAttacker>())
            {
                TimeSpan recovery;
                if (!recoveryTimes.TryGetValue(item, out recovery))
                {
                    recovery = item.Recovery;
                    recoveryTimes.Add(item, item.Recovery);
                }

                if (recovery < item.Recovery)
                    PlaySound("sword");
                recoveryTimes[item] = item.Recovery;
            }

            // Hit
            foreach (IAttackable item in area.Items.OfType<IAttackable>())
            {
                int points;
                if (!hitpoints.TryGetValue(item, out points))
                {
                    points = item.Hitpoints;
                    hitpoints.Add(item, item.Hitpoints);
                }

                if (points > item.Hitpoints)
                    PlaySound("hit");
                hitpoints[item] = item.Hitpoints;
            }
        }

        public void PlaySound(string sound)
        {
            SoundEffect soundEffect;
            if (_sounds.TryGetValue(sound, out soundEffect))
            {
                soundEffect.Play(_volume, 0f, 0f);
            }
        }
    }
}