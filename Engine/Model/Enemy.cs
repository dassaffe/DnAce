using System;
using System.Collections.Generic;
using Engine.Interface;

namespace Engine.Model
{
    public class Enemy : Character, IAttackable, IAttacker
    {
        public int MaxHitpoints { get; set; }
        public int Hitpoints { get; set; }
        public Action<Engine, IAttacker, IAttackable> OnHit { get; }
        public ICollection<IAttackable> AttackableSprites { get; }
        public float AttackRange { get; set; }
        public int AttackValue { get; set; }
        public TimeSpan TotalRecovery { get; set; }
        public TimeSpan Recovery { get; set; }
        public bool AttackSignal { get; set; }

        public Enemy(Engine gameEngine) : base(gameEngine)
        {
            GameEngine = gameEngine;
            AttackableSprites = new List<IAttackable>();
            MaxHitpoints = 2;
            Hitpoints = 2;
            AttackRange = 0.8f;
            AttackValue = 1;
            TotalRecovery = TimeSpan.FromSeconds(0.6);
            Texture = "orc";
            Name = "Orc";
            Icon = "orcicon";
            Ai = new AggressiveAi(this, 4f);
        }
    }
}