using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Microsoft.Xna.Framework;

namespace Engine.Controls
{
    /// <summary>
    /// Horizontale Spezialisierung des ListControls.
    /// </summary>
    internal abstract class HorizontalListControl<T> : ListControl<T> where T : ListItem
    {
        public HorizontalListControl(ScreenComponent manager)
            : base(manager)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // Alle potentiell selektierbaren Elemente ermitteln
            List<T> availableItems = Items.Where(i => i.Enabled && i.Visible).ToList();

            // Links-Klick verarbeiten
            if (Manager.GameEngine.Input.Left)
            {
                // Wenn nichts selektiert ist wird der letzte Eintrag aus der Liste markiert.
                if (SelectedItem == null)
                {
                    SelectedItem = availableItems.LastOrDefault();
                }
                else
                {
                    // Ermittlung des Index des aktuellen Elementes
                    int index = availableItems.IndexOf(SelectedItem);
                    index = Math.Max(0, index - 1);
                    SelectedItem = availableItems[index];
                }
                Manager.GameEngine.Input.Handled = true;
            }

            // Rechts-Klick verarbeiten
            if (Manager.GameEngine.Input.Right)
            {
                // Wenn nichts selektiert ist wird der erste Eintrag aus der Liste markiert.
                if (SelectedItem == null)
                {
                    SelectedItem = availableItems.FirstOrDefault();
                }
                else
                {
                    // Ermittlung des Index des aktuellen Elementes
                    int index = availableItems.IndexOf(SelectedItem);
                    index = Math.Min(availableItems.Count - 1, index + 1);
                    SelectedItem = availableItems[index];
                }
                Manager.GameEngine.Input.Handled = true;
            }

            base.Update(gameTime);
        }
    }
}