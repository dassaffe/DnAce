using System.Diagnostics;
using System.Linq;
using Engine.Components;
using Engine.Controls;
using Engine.Model;
using Microsoft.Xna.Framework;

namespace Engine.Screens
{
    /// <summary>
    /// Dialog-Screen 
    /// </summary>
    internal class DialogScreen : Screen
    {
        private Sprite speaker;

        private Dialog current;

        private Label message;

        private DialogList list;
        private Sprite.MovementType oldMoveType;

        public DialogScreen(ScreenComponent manager, Sprite speaker, Dialog entry)
            : base(manager)
        {
            this.speaker = speaker;
            oldMoveType = this.speaker.MoveType;
            current = entry;

            Controls.Add(new Icon(manager) { Position = new Rectangle(10, 10, 24, 24), Texture = speaker.Icon });
            Controls.Add(message = new Label(manager) { Position = new Rectangle(40, 10, manager.GraphicsDevice.Viewport.Width - 50, 30) });
            Controls.Add(list = new DialogList(manager));

            list.OnInteract += OnInteract;

            this.speaker.MoveType = Sprite.MovementType.NoMovement;
            Refill();
        }

        /// <summary>
        /// Rekonfiguriert den Dialog auf Basis des aktuellen Dialog-Schritts.
        /// </summary>
        private void Refill()
        {
            message.Text = current.Message;
            var measure = Manager.Font.MeasureString(message.Text);

            var posY = 50;
            if (measure.Y > 30)
                posY += (int)measure.Y - 30;

            list.SelectedItem = null;
            list.Items.Clear();
            int height = 0;
            foreach (var entry in current.Options.Where(o => o.Visible))
            {
                list.Items.Add(new ListItem { Text = entry.Option, Tag = entry });
                height += 30;
            }

            // Optionaler Back-Button
            if (current.Back != null)
            {
                list.Items.Add(new ListItem { Text = "Zurück", Tag = current.Back });
                height += 30;
            }

            // Optionaler Beenden-Button
            if (current.CanExit)
            {
                list.Items.Add(new ListItem { Text = "Beenden" });
                height += 30;
            }

            // Ersten Eintrag selektieren
            if (list.Items.Count > 0)
                list.SelectedItem = list.Items[0];

            Position = new Rectangle(10, Manager.GraphicsDevice.Viewport.Height - height - 70, Manager.GraphicsDevice.Viewport.Width - 20, height + 60);
            //list.Position = new Rectangle(15, 50, Position.Width - 30, height);
            list.Position = new Rectangle(15, posY, Position.Width - 30, height);
        }

        private void OnInteract(ListItem item)
        {
            Dialog dialog = item.Tag as Dialog;
            if (dialog != null)
            {
                // Auswahl einer Dialog-Option
                current = dialog;
                Refill();
            }
            else
            {
                // Beenden-Eitnrag ausgewählt
                Manager.CloseScreen();
                speaker.MoveType = oldMoveType;
            }
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}