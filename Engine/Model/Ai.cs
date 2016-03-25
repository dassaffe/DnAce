using Microsoft.Xna.Framework;

namespace Engine.Model
{
    /// <summary>
    /// Basis-Klasse für eigenständige KIs
    /// </summary>
    public abstract class Ai
    {
        /// <summary>
        /// Hält eine Referenz auf den Character.
        /// </summary>
        protected Character Host { get; }

        /// <summary>
        /// Gibt an ob die KI im Wandermodus ist.
        /// </summary>
        protected bool Walking { get { return _destination.HasValue; } }

        /// <summary>
        /// Speichert den Startpunkt des Wanderauftrags.
        /// </summary>
        private Vector2? _startPoint;

        /// <summary>
        /// Speichert den Zielpunkt des Wanderauftrags.
        /// </summary>
        private Vector2? _destination;

        private float _speed;

        public Ai(Character host)
        {
            Host = host;
            _startPoint = null;
            _destination = null;
        }

        public void Update(Area area, GameTime gameTime)
        {
            OnUpdate(area, gameTime);

            // Bewegung
            if (_destination.HasValue)
            {
                Vector2 expectedDistance = _destination.Value - _startPoint.Value;
                Vector2 currentDistance = Host.Position - _startPoint.Value;

                // Prüfen ob das Ziel erreicht (oder überschritten) wurde.
                if (currentDistance.LengthSquared() > expectedDistance.LengthSquared())
                {
                    _startPoint = null;
                    _destination = null;
                    Host.Velocity = Vector2.Zero;
                }
                else
                {
                    // Kurs festlegen
                    Vector2 direction = _destination.Value - Host.Position;
                    direction.Normalize();
                    Host.Velocity = direction * _speed * Host.MaxSpeed;
                }
            }
        }

        public abstract void OnUpdate(Area area, GameTime gameTime);

        protected internal void WalkTo(Vector2 destination, float speed)
        {
            _startPoint = Host.Position;
            _destination = destination;
            _speed = speed;
        }
    }
}