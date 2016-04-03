using Microsoft.Xna.Framework;

namespace Engine.Model
{
    public class Layer
    {
        private int Width { get; set; }
        private int Height { get; set; }

        /// <summary>
        /// A map consists of multiple tiles
        /// </summary>
        public int[,] Tiles { get; private set; }

        public Layer(int width, int height)
        {
            Width = width;
            Height = height;

            Tiles = new int[width, height];
        }

        public Point GetLayerDimensions()
        {
            return new Point(Width, Height);
        }
    }
}