using System.Collections.Generic;

namespace Engine.Interface
{
    /// <summary>
    /// Interface für alle Spielelemente, die aktiv mit anderen Items interagieren.
    /// </summary>
    public interface IInteractor
    {
        /// <summary>
        /// Interne auflistung aller Items im Interaktionsradius.
        /// </summary>
        ICollection<IInteractable> InteractableSprites { get; }

        /// <summary>
        /// Interaktionsradius in dem interagiert werden kann.
        /// </summary>
        float InteractionRange { get; }

        /// <summary>
        /// Interner Flag, um bevorstehenden Interact zu signalisieren
        /// </summary>
        bool InteractSignal { get; set; }
    }
}