using System;
using System.Linq;
using Engine.Interface;
using Engine.Screens;
using Microsoft.Xna.Framework;

namespace Engine.Model
{
    class Npc : Character, IInteractable
    {
        public Action<Engine, IInteractor, IInteractable> OnInteract { get; set; }

        private Dialog dialog;

        //private Dialog before;

        //private Dialog after;

        //private Quest quest;

        public Npc(Engine gameEngine) : base(gameEngine)
        {
            GameEngine = gameEngine;

            Texture = "char";
            Name = "NPC";
            Icon = "charicon";

            OnInteract = DoInteract;

            Ai = new WalkingAi(this, 0.4f);

            //dialog = new Dialog { Message = "Hallo junger Held.", CanExit = true };
            //dialog.Options.Add(new Dialog
            //{
            //    Option = "Erzähl mal etwas über das Dorf",
            //    Message = "Hier war alles friedlich, bis die Orks kamen...",
            //    Back = dialog
            //});
            //dialog.Options.Add(before = new Dialog
            //{
            //    Option = "Kann ich helfen?",
            //    Message = "Ja, bitte. Mir würde meine goldene Münze geraubt",
            //    Back = dialog,
            //    OnShow = (engine, character) =>
            //    {
            //        // Questfortschritt setzen
            //        quest.Progress("search");
            //        before.Visible = false;
            //    }
            //});
            //dialog.Options.Add(after = new Dialog
            //{
            //    Option = "Hier ist deine Münze",
            //    Message = "Wow! Ich und das Dorf werden dir das nicht vergessen! *schmatz*",
            //    Back = dialog,
            //    OnShow = (engine, character) =>
            //    {
            //        // Questgegenstand entfernen
            //        Player player = character as Player;
            //        var coin = player.Inventory.SingleOrDefault(i => i.Name.Equals("Goldene Münze"));
            //        if (coin != null)
            //            player.Inventory.Remove(coin);

            //        // Quest Fortschritt auf Success 
            //        quest.Success("success");
            //        after.Visible = false;
            //    }
            //});
        }

        private void DoInteract(Engine gameEngine, IInteractor interactor, IInteractable interactable)
        {
            // Zum Spieler drehen
            Ai.WalkTo(new Vector2(
                (gameEngine.Local.Player.Position.X > Position.X ? Position.X + 0.00001f : Position.X - 0.000000001f),
                (gameEngine.Local.Player.Position.Y > Position.Y ? Position.Y + 0.00001f : Position.Y - 0.000000001f)), 1.0f);

            //quest = gameEngine.Simulation.World.Quests.SingleOrDefault(q => q.Name == "Heidis Quest");
            //if (quest != null)
            //{
            //    before.Visible = quest.State == QuestState.Inactive;
            //    after.Visible = quest.State == QuestState.Active && quest.CurrentProgress.Id == "return";
            //}

            gameEngine.Screen.ShowScreen(new ShoutScreen(gameEngine.Screen, this, "Das ist ein Dummytext"));

            //gameEngine.Screen.ShowScreen(new DialogScreen(gameEngine.Screen, this, dialog));
        }
    }
}