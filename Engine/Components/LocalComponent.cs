using System.Linq;
using Engine.Model;
using Microsoft.Xna.Framework;

namespace Engine.Components
{
    /// <summary>
    /// Container für den lokalen Spieler
    /// </summary>
    internal class LocalComponent : GameComponent
    {
        private readonly Engine _gameEngine;

        /// <summary>
        /// Referenz auf den aktuellen Spieler.
        /// </summary>
        public Player Player { get; }

        public LocalComponent(Engine gameEngine)
            : base(gameEngine)
        {
            _gameEngine = gameEngine;

            // Den Spieler einfügen.
            gameEngine.Simulation.InsertPlayer(Player = new Player(_gameEngine));
        }

        /// <summary>
        /// Ermittelt die Area in der sich der lokale Spieler aktuell befindet.
        /// </summary>
        /// <returns>The current area.</returns>
        public Area GetCurrentArea()
        {
            return _gameEngine.Simulation.World.Areas.FirstOrDefault(a => a.Sprites.Contains(_gameEngine.Local.Player));
        }

        public override void Update(GameTime gameTime)
        {
            if (!_gameEngine.Input.Handled)
            {
                Player.Velocity = _gameEngine.Input.Movement * Player.MaxSpeed;

                // Interaktionen signalisieren
                if (_gameEngine.Input.Interact)
                    Player.InteractSignal = true;

                // Angriff signalisieren
                if (_gameEngine.Input.Attack)
                    Player.AttackSignal = true;

                _gameEngine.Input.Handled = true;
            }
            else
            {
                Player.Velocity = Vector2.Zero;
            }
        }
    }
}