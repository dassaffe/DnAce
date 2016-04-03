using System;

namespace Engine.Interface
{
    /// <summary>
    /// Interface für alle Spielelemente mit denen interagiert werden kann.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Delegat für aktiven Interaktionsversuch des Spielers.
        /// </summary>
        Action<Engine, IInteractor, IInteractable> OnInteract { get; }
    }
}