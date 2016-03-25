﻿using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Microsoft.Xna.Framework;

namespace Engine.Controls
{
    /// <summary>
    /// Vertikale Spezialisierung des ListControls.
    /// </summary>
    internal abstract class VerticalListControl<T> : ListControl<T> where T : ListItem
    {
        public VerticalListControl(ScreenComponent manager)
            : base(manager)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // Alle potentiell selektierbaren Elemente ermitteln
            List<T> availableItems = Items.Where(i => i.Enabled && i.Visible).ToList();

            // Oben-Klick verarbeiten
            if (Manager.GameEngine.Input.Up)
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

            // Unten-Klick verarbeiten
            if (Manager.GameEngine.Input.Down)
            {
                // Wenn nichts selektiert ist wird der letzte Eintrag aus der Liste markiert.
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