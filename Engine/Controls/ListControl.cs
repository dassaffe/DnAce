using System.Collections.Generic;
using Engine.Components;
using Microsoft.Xna.Framework;

namespace Engine.Controls
{
    /// <summary>
    /// Basis-Klasse für alle listenbasierten Controls.
    /// </summary>
    internal abstract class ListControl<T> : Control where T : ListItem
    {
        private T _selectedItem;

        /// <summary>
        /// Auflistung aller List-Items.
        /// </summary>
        public List<T> Items { get; private set; }

        /// <summary>
        /// Der selektierte Eintrag.
        /// </summary>
        public T SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    Manager.GameEngine.Sound.PlaySound("clock");
                    if (OnSelectionChanged != null)
                    {
                        OnSelectionChanged(_selectedItem);
                    }
                }
            }
        }

        public ListControl(ScreenComponent manager)
            : base(manager)
        {
            Items = new List<T>();
            SelectedItem = default(T);
        }

        public override void Update(GameTime gameTime)
        {
            // Ermittelt, ob der User den aktuellen Eintrag bestätigt hat.
            if (Manager.GameEngine.Input.Interact)
            {
                // Interact, falls ein Item selektiert ist.
                if (SelectedItem != null && OnInteract != null)
                {
                    Manager.GameEngine.Sound.PlaySound("click");
                    OnInteract(SelectedItem);
                }

                Manager.GameEngine.Input.Handled = true;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Signalisiert den Wechsel des selektierten Eintrags.
        /// </summary>
        public event ItemDelegate<T> OnSelectionChanged;

        /// <summary>
        /// Signalisiert die Auswahl eines Eintrags vom Spieler.
        /// </summary>
        public event ItemDelegate<T> OnInteract;

        /// <summary>
        /// Standard Delegat für alle Listen-Events.
        /// </summary>
        public delegate void ItemDelegate<TV>(TV item);
    }

    /// <summary>
    /// Einzelner Eintrag in einem ListControl
    /// </summary>
    internal class ListItem
    {
        /// <summary>
        /// Platzhalter für diverse Referenzen.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gibt an ob dieser Eintrag aktiv ist.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gibt an ob dieser Eintrag sichtbar ist.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Gibt den Anzeigetext dieses Eintrags an.
        /// </summary>
        public string Text { get; set; }

        public ListItem()
        {
            Enabled = true;
            Visible = true;
        }

        public override string ToString()
        {
            return Text ?? string.Empty;
        }
    }
}

