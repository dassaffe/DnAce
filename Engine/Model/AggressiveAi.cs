using System.Collections.Generic;
using System.Linq;
using Engine.Interface;
using Microsoft.Xna.Framework;

namespace Engine.Model
{
    /// <summary>
    /// Agressive KI für Gegner. Reagiert bei Sichtkontakt
    /// </summary>
    internal class AggressiveAi : Ai
    {
        private Vector2? _startPoint;

        private readonly IAttacker _attacker;

        private Sprite _target;

        private readonly float _range;

        public AggressiveAi(Character host, float range) : base(host)
        {
            _range = range;
            _attacker = (IAttacker)host;
        }

        public override void OnUpdate(Area area, GameTime gameTime)
        {
            // Startpunkt ermitteln
            if (!_startPoint.HasValue)
                _startPoint = Host.Position;

            // Nach Zielen ausschau halten
            if (_target == null)
            {
                IEnumerable<IAttackable> potentialTargets = area.Sprites.
                    Where(i => (i.Position - Host.Position).LengthSquared() < _range * _range). // Filter nach Angriffsreichweite
                    Where(i => i.GetType() != Host.GetType()). // Items vom selben Typ verschonen
                    OrderBy(i => (i.Position - Host.Position).LengthSquared()). // Sortiert nach Entfernung
                    OfType<IAttackable>(). // Gefiltert nach Angreifbarkeit
                    Where(a => a.Hitpoints > 0);    // Nur lebende

                _target = potentialTargets.FirstOrDefault() as Sprite;
            }

            // Ziel angreifen
            if (_target != null)
            {
                _attacker.AttackSignal = true;

                // Bei zu großem Abstand vom Ziel ablassen
                if ((_target.Position - Host.Position).LengthSquared() > _range * _range ||
                    ((_target is IAttackable) && (_target as IAttackable).Hitpoints <= 0))
                {
                    _target = null;
                    WalkTo(_startPoint.Value, 0.4f);
                }
                else
                {
                    WalkTo(_target.Position, 0.6f);
                }
            }
        }
    }
}