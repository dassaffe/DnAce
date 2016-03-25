using Microsoft.Xna.Framework;

namespace Engine.Interface
{
    public interface IMapElement
    {
        /// <summary>
        /// Gibt an, ob dieses Element verschiebbar oder am Spielfeld fixiert ist.
        /// </summary>
        bool Fixed { get; }

        /// <summary>
        /// Position des Spielelementes.
        /// </summary>
        Vector2 Position { get; }

        /// <summary>
        /// Kollisionsradius des Spielelementes.
        /// </summary>
        float Radius { get; }

        /// <summary>
        /// Name der zu verwendenden Textur.
        /// </summary>
        string Texture { get; }
    }
}