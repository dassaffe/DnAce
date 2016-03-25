using Engine.Components;
using Engine.Controls;
using Microsoft.Xna.Framework;

namespace Engine.Screens
{
    internal class MainMenuScreen : Screen
    {
        private readonly ListItem _newGameItem = new ListItem { Text = "Neues Spiel" };
        private readonly ListItem _networkItem = new ListItem { Text = "Mehrspieler", Enabled = false };
        private readonly ListItem _optionsItem = new ListItem { Text = "Optionen", Enabled = false };
        private readonly ListItem _exitItem = new ListItem { Text = "Beenden" };

        public MainMenuScreen(ScreenComponent manager) : base(manager, new Point(400, 300))
        {
            MenuList menu;
            Controls.Add(new Panel(manager) { Position = new Rectangle(20, 20, 360, 40) });
            Controls.Add(new Label(manager) { Text = "HAUPTMENÜ", Position = new Rectangle(40, 30, 0, 0) });
            Controls.Add(menu = new MenuList(manager) { Position = new Rectangle(20, 70, 360, 200) });

            menu.Items.Add(_newGameItem);
            menu.Items.Add(_networkItem);
            menu.Items.Add(_optionsItem);
            menu.Items.Add(_exitItem);

            menu.SelectedItem = _newGameItem;

            menu.OnInteract += OnInteract;
        }

        private void OnInteract(ListItem item)
        {
            if (item == _newGameItem)
            {
                Manager.GameEngine.Simulation.NewGame();
                Manager.CloseScreen();
            }

            if (item == _exitItem)
            {
                Manager.Game.Exit();
                Manager.CloseScreen();
            }
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

        public override void OnShow()
        {
            Manager.GameEngine.Music.OpenMenu();
        }

        public override void OnHide()
        {
            Manager.GameEngine.Music.CloseMenu();
        }
    }
}