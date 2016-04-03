using Microsoft.Xna.Framework;

namespace Engine.Model
{
    /// <summary>
    /// Repräsentiert eine sich eigenständig bewegende Spieleinheit.
    /// </summary>
    public class Character : Sprite
    {
        public Engine GameEngine;

        /// <summary>
        /// Gibt die maximale Fortbeschwegungsgeschwindigkeit des Characters an.
        /// </summary>
        /// <value>Maximalgeschwindigkeit</value>
        public float MaxSpeed { get; set; }

        /// <summary>
        /// Geschwindigkeitsvektor
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// KI Basis
        /// </summary>
        public Ai Ai { get; set; }

        public Character(Engine gameEngine)
        {
            GameEngine = gameEngine;
            MaxSpeed = 8f;
            Radius = 0.4f;
        }
    }
}