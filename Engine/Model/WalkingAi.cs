using System;
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;

namespace Engine.Model
{
    /// <summary>
    /// Standard-KI für normale Character. Char läuft so umher.
    /// </summary>
    internal class WalkingAi : Ai
    {
        private float range;

        private Vector2? center;

        private TimeSpan delay;

        private Character host;

        private RNGCryptoServiceProvider Random { get; set; }

        public WalkingAi(Character host, float range) : base(host)
        {
            this.host = host;
            this.range = range;
            Random = new RNGCryptoServiceProvider();
            delay = TimeSpan.Zero;
        }

        public override void OnUpdate(Area area, GameTime gameTime)
        {
            if(host.MoveType == Sprite.MovementType.NoMovement)
                return;

            // Initiales Zentrum festlegen
            if (!center.HasValue)
                center = Host.Position;

            if (!Walking)
            {
                // Delay abwarten
                if (delay > TimeSpan.Zero)
                {
                    delay -= gameTime.ElapsedGameTime;
                    return;
                }
                Vector2 destination;
                do
                {
                    // Neuen Zielpunkt wählen
                    Vector2 variation = new Vector2(
                        (float)(GetRandom() * 2 - 1.0),
                        (float)(GetRandom() * 2 - 1.0));

                    Vector2 variationFix = variation*2*range;

                    destination = center.Value + variationFix;

                } while (area.IsCellBlocked((int)destination.X, (int)destination.Y));

                WalkTo(destination, 0.4f);
                delay = TimeSpan.FromSeconds(2);
            }
        }

        private double GetRandom()
        {
            byte[] data = new byte[sizeof (uint)];
            Random.GetBytes(data);
            uint randUint = BitConverter.ToUInt32(data, 0);
            double rand = randUint/(uint.MaxValue + 1.0);
            return rand;
        }
    }
}