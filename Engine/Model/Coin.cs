namespace Engine.Model
{
    /// <summary>
    /// Repräsentiert die Münzen im Spiel.
    /// </summary>
    internal class Coin : Item
    {
        public Coin()
        {
            //Mass = 0.5f;
            Texture = "coin_silver";
            Name = "Coin";
            Icon = "coinicon";
        }
    }
}