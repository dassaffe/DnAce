using Microsoft.Xna.Framework;

namespace Engine.Rendering
{
    /// <summary>
    /// Repräsentiert die Kamera auf die Szene
    /// </summary>
    public class Camera
    {
        private readonly Vector2 _viewSizeHalf;

        private Vector2 _areaSize;

        public int BorderX
        {
            get;
            set;
        }
        public int BorderY
        {
            get;
            set;
        }

        /// <summary>
        /// Position des Zentrums in World-Koordinaten.
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// Multiplikator für World -> View Koordinaten.
        /// </summary>
        public float Scale { get; }

        /// <summary>
        /// Der Render-Versatz in World-Koordinaten.
        /// </summary>
        public Vector2 Offset { get { return Position - ViewSizeHalf; } }

        /// <summary>
        /// Halbe Bildschirmgröße in World-Koordinaten.
        /// </summary>
        public Vector2 ViewSizeHalf { get { return _viewSizeHalf / Scale; } }

        /// <summary>
        /// Kamera-Konstruktor.
        /// </summary>
        /// <param name="viewSize">Größe des Viewports in Pixel</param>
        public Camera(Point viewSize)
        {
            _viewSizeHalf = new Vector2(viewSize.X / 2f, viewSize.Y / 2f);
            Scale = 64f;
            BorderX = viewSize.X / 3;
            BorderY = viewSize.Y / 3;
        }

        /// <summary>
        /// Setzt den Fokus auf den gegebenen Punkt in World-Koordinaten.
        /// </summary>
        /// <param name="position">Fokuspunkt</param>
        public void SetFocusExplizit(Vector2 position, Vector2 areaSize)
        {
            _areaSize = areaSize;
            Position = position;
            SetFocus(position);
        }

        /// <summary>
        /// Stellt sicher, dass der angegebene Punkt in World-Koordinaten sichtbar ist.
        /// </summary>
        /// <param name="position">Fokuspunkt</param>
        public void SetFocus(Vector2 position)
        {
            Vector2 viewSize = ViewSizeHalf * 2f;
            float worldBorderX = BorderX / Scale;
            float worldBorderY = BorderY / Scale;

            // Kamerakorrekturen auf X-Achse
            if (_areaSize.X > viewSize.X)
            {
                // Position nach links verschieben, falls neue Position aus dem Bildschirmrand läuft.
                float left = position.X - Offset.X - worldBorderX;
                if (left < 0f)
                    Position = new Vector2(Position.X + left, Position.Y);

                // Position nach rechts verschieben, falls neue Position aus dem Bildschirmrand läuft.
                float right = viewSize.X - position.X + Offset.X - worldBorderX;
                if (right < 0f)
                    Position = new Vector2(Position.X - right, Position.Y);

                // Weiter nach rechts schieben, sollte die Position den Hintergrund frei legen.
                left = Offset.X;
                if (left < 0f)
                    Position = new Vector2(Position.X - left, Position.Y);

                // Weiter nach links schieben, sollte die Position den Hintergrund frei legen.
                right = _areaSize.X - Offset.X - viewSize.X;
                if (right < 0f)
                    Position = new Vector2(Position.X + right, Position.Y);
            }
            else
            {
                // Spielfeld zu klein für korrekturen -> Zentrieren
                Position = new Vector2(_areaSize.X / 2f, Position.Y);
            }

            if (_areaSize.Y > viewSize.Y)
            {
                // Position nach oben verschieben, falls neue Position aus dem Bildschirmrand läuft.
                float top = position.Y - Offset.Y - worldBorderY;
                if (top < 0f)
                    Position = new Vector2(Position.X, Position.Y + top);

                // Position nach unten verschieben, falls neue Position aus dem Bildschirmrand läuft.
                float bottom = viewSize.Y - position.Y + Offset.Y - worldBorderY;
                if (bottom < 0f)
                    Position = new Vector2(Position.X, Position.Y - bottom);

                // Weiter nach unten schieben, sollte die Position den Hintergrund frei legen.
                top = Offset.Y;
                if (top < 0f)
                    Position = new Vector2(Position.X, Position.Y - top);

                // Weiter nach oben schieben, sollte die Position den Hintergrund frei legen.
                bottom = _areaSize.Y - Offset.Y - viewSize.Y;
                if (bottom < 0f)
                    Position = new Vector2(Position.X, Position.Y + bottom);
            }
            else
            {
                // Spielfeld zu klein für korrekturen -> Zentrieren
                Position = new Vector2(Position.X, _areaSize.Y / 2f);
            }
        }
    }
}

