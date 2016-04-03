using Microsoft.Xna.Framework;

namespace Engine.Model
{
    public class Tile
    {
        /// <summary>
        /// Name der Textur
        /// </summary>
        public string Texture { get; set; }

        public Rectangle SourceRectangle { get; set; }

        /// <summary>
        /// Gibt an, ob dieses Tile den Spieler an der Bewegung hindert
        /// </summary>
        public bool Blocked { get; set; }
    }
}
