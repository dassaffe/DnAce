using System;
using System.Collections.Generic;
using Engine.Interface;

namespace Engine.Model
{
    public class Player : Character, IAttackable, IAttacker, IInteractor, IInventory
    {
        /// <summary>
        /// Maximale Anzahl Trefferpunkte im gesunden Zustand
        /// </summary>
        public int MaxHitpoints { get; set; }

        /// <summary>
        /// Anzahl verfügbarer Trefferpunkte
        /// </summary>
        public int Hitpoints { get; set; }

        /// <summary>
        /// Aufruf bei ankommenden Treffern.
        /// </summary>
        public Action<Engine, IAttacker, IAttackable> OnHit { get; }

        /// <summary>
        /// Zeigt an, ob der Spieler noch in einem Portal steht
        /// </summary>
        public bool InPortal { get; set; }

        /// <summary>
        /// Intern geführte Liste aller angreifbaren Elemente in der Nähe
        /// </summary>
        public ICollection<IAttackable> AttackableSprites { get; }

        /// <summary>
        /// Angriffsradius in dem Schaden ausgeteilt wird
        /// </summary>
        public float AttackRange { get; set; }

        /// <summary>
        /// Schaden der pro Angriff verursacht wird
        /// </summary>
        public int AttackValue { get; set; }

        public TimeSpan TotalRecovery { get; }
        public TimeSpan Recovery { get; set; }
        public bool AttackSignal { get; set; }
        public bool InteractSignal { get; set; }

        /// <summary>
        /// Interne auflistung aller Items im Interaktionsradius
        /// </summary>
        public ICollection<IInteractable> InteractableSprites { get; }

        /// <summary>
        /// Interaktionsradius in dem interagiert werden kann
        /// </summary>
        public float InteractionRange { get; set; }

        private readonly List<Item> _inventory;
        /// <summary>
        /// Listet die Items im Inventar auf
        /// </summary>
        public ICollection<Item> Inventory { get { return _inventory; } }

        public Player(Engine gameEngine) : base(gameEngine)
        {
            GameEngine = gameEngine;
            _inventory = new List<Item>();
            AttackableSprites = new List<IAttackable>();
            InteractableSprites = new List<IInteractable>();
            MaxHitpoints = 10;
            Hitpoints = 10;
            AttackRange = 0.5f;
            AttackValue = 1;
            InteractionRange = 0.8f;
            Texture = "char";
            TotalRecovery = TimeSpan.FromMilliseconds(400);
        }
    }
}
