using System;
using Engine.Interface;
using Microsoft.Xna.Framework;

namespace Engine.Model
{
    /// <summary>
    /// Basisklasse für alle Sprites auf dem Spielfeld
    /// </summary>
    public class Sprite : ICollidable, IMapElement
    {
        // Internes Feld zur Haltung des temporären Move-Vektors.
        internal Vector2 Move = Vector2.Zero;

        /// <summary>
        /// Anzeigename dieses Items.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Die Masse des Objektes.
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        /// Iconname dieses Items.
        /// </summary>
        public string Icon { get; set; }
        public bool Fixed { get; set; }
        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public string Texture { get; set; }
        public MovementType MoveType { get; set; }

        /// <summary>
        /// Action die bei jedem Schleifendurchlauf aufgerufen wird.
        /// </summary>
        public Action<Engine, Area, Item, GameTime> Update { get; set; }

        public Sprite()
        {
            // Standard-Werte für Kollisionselemente
            Fixed = false;
            Mass = 1f;
            Radius = 0.25f;
        }

        public enum MovementType
        {
            NoMovement,
            WalkAround
        }
    }
}
