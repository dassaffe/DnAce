using System;

namespace Engine.Interface
{
    /// <summary>
    /// Interface für alle angreifbaren Spielelemente
    /// </summary>
    public interface IAttackable
    {
        /// <summary>
        /// Maximale Anzahl Trefferpunkte im gesunden Zustand.
        /// </summary>
        int MaxHitpoints { get; }

        /// <summary>
        /// Anzahl verfügbarer Trefferpunkte.
        /// </summary>
        int Hitpoints { get; set; }

        /// <summary>
        /// Aufruf bei ankommenden Treffern.
        /// </summary>
        Action<Engine, IAttacker, IAttackable> OnHit { get; }
    }
}
