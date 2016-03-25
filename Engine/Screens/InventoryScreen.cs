using System;
using System.Linq;
using Engine.Components;
using Engine.Controls;
using Engine.Model;
using Microsoft.Xna.Framework;

namespace Engine.Screens
{
    internal class InventoryScreen : Screen
    {
        public InventoryScreen(ScreenComponent manager)
            : base(manager, new Point(400, 300))
        {
            Controls.Add(new Panel(manager) { Position = new Rectangle(20, 20, 360, 40) });
            Controls.Add(new Label(manager) { Text = "Rucksack", Position = new Rectangle(40, 30, 0, 0) });

            InventoryList list = new InventoryList(manager) { Position = new Rectangle(20, 70, 360, 200) };
            foreach (IGrouping<string, Item> itemGroup in manager.GameEngine.Local.Player.Inventory.GroupBy(i => i.Name))
            {
                list.Items.Add(new InventoryItem
                {
                    Text = itemGroup.First().Name,
                    Icon = itemGroup.First().Icon,
                    Count = itemGroup.Count()
                });
            }
            Controls.Add(list);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Manager.GameEngine.Input.Handled)
            {
                if (Manager.GameEngine.Input.Close || Manager.GameEngine.Input.Inventory)
                {
                    Manager.CloseScreen();
                    Manager.GameEngine.Input.Handled = true;
                }
            }
        }
    }
}