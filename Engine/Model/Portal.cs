using Microsoft.Xna.Framework;

namespace Engine.Model
{
    /// <summary>
    /// Portalbereich für den Übergang zwischen 2 Maps
    /// </summary>
    public class Portal
    {
        /// <summary>
        /// Name der Zielarea zu der das Portal führt
        /// </summary>
        public string DestinationArea { get; set; }

        /// <summary>
        /// Portalbereich
        /// </summary>
        public Rectangle Box { get; set; }
    }
}