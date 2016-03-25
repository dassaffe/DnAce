using System;
using Engine.Interface;
using Microsoft.Xna.Framework;

namespace Engine.Model
{
    public class Item : ICollectable, IMapElement
    {
        /// <summary>
        /// Anzeigename dieses Items.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Iconname dieses Items.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Die Masse des Objektes.
        /// </summary>
        public float Mass { get; set; }
        public bool Fixed { get; set; }
        public Vector2 Position { get; set; }
        public float Radius { get; set; }
        public string Texture { get; set; }

        public Item()
        {
            Radius = 0.25f;
        }

        public Action<Engine, Character> OnCollect { get; set; }
    }
}