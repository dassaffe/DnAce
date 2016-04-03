using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Engine.Interface;
using Engine.Model;
using Microsoft.Xna.Framework;

namespace Engine.Components
{
    /// <summary>
    /// Game Komponente zur ständigen Berechnung des Spielverlaufs im Model.
    /// </summary>
    public class SimulationComponent : GameComponent
    {
        // Sicherheitslücke gegen Rundungsfehler
        private readonly float _gap = 0.00001f;

        private readonly Engine _gameEngine;

        /// <summary>
        /// Referenz auf das zentrale Spielmodell.
        /// </summary>
        public World World { get; private set; }

        ///// <summary>
        ///// Referenz auf den aktuellen Spieler.
        ///// </summary>
        //public Player Player { get; private set; }

        ///// <summary>
        ///// Referenz auf die aktuelle Area in der sich der Spieler gerade befindet
        ///// </summary>
        //public Area Area { get; private set; }

        public SimulationComponent(Engine gameEngine) : base(gameEngine)
        {
            _gameEngine = gameEngine;
            NewGame();
        }

        public void NewGame()
        {
            if (World == null)
                World = new World();

            World.Areas.Clear();

            World = new World();
            MapLoader mapLoader = new MapLoader(_gameEngine, Path.Combine(_gameEngine.Content.RootDirectory, "Maps"));
            World.Areas.AddRange(mapLoader.LoadAll());

            // Quests erstellen
            Quest quest = new Quest
            {
                Name = "Heidis Quest"
            };
            World.Quests.Add(quest);

            quest.QuestProgresses.Add(new QuestProgress { Id = "search", Description = "Gehe auf die Suche nach der goldenen Münze" });
            quest.QuestProgresses.Add(new QuestProgress { Id = "return", Description = "Bring die Münze zurück" });
            quest.QuestProgresses.Add(new QuestProgress { Id = "success", Description = "Das Dorf wird dir ewig dankbar sein" });
            quest.QuestProgresses.Add(new QuestProgress { Id = "fail", Description = "Die Münze ist für immer verloren" });
        }

        /// <summary>
        /// Fügt den Player an eine der Startstellen ein
        /// </summary>
        /// <returns>Area in die der Spieler eingefügt wurde</returns>
        /// <param name="player">Player-Instanz</param>
        internal Area InsertPlayer(Player player)
        {
            Area target = World.Areas.FirstOrDefault(a => a.Startpoints.Count > 0);
            if (target != null)
            {
                player.Position = target.Startpoints[0];
                target.Sprites.Add(player);
            }
            return target;
        }

        public override void Update(GameTime gameTime)
        {
            List<Action> actions = new List<Action>();

            //var visibleArea = _gameEngine.Local.GetCurrentArea();

            #region CharacterInput

            foreach (Area area in World.Areas)
            {
                // Schleife über alle sich aktiv bewegenden Spielelemente
                foreach (Character character in area.Sprites.OfType<Character>())
                {
                    if (character is IAttackable && (character as IAttackable).Hitpoints <= 0)
                        continue;

                    // KI Update
                    if (character.Ai != null)
                        character.Ai.Update(area, gameTime);

                    // Neuberechnung der Characterposition
                    character.Move += character.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Attacker identifizieren
                    IAttacker attacker = null;
                    if (character is IAttacker)
                    {
                        attacker = (IAttacker)character;
                        attacker.AttackableSprites.Clear();

                        // Recoverytime aktualisieren
                        attacker.Recovery -= gameTime.ElapsedGameTime;
                        if (attacker.Recovery < TimeSpan.Zero)
                            attacker.Recovery = TimeSpan.Zero;
                    }

                    // Interactor identifizieren
                    IInteractor interactor = null;
                    if (character is IInteractor)
                    {
                        interactor = (IInteractor)character;
                        interactor.InteractableSprites.Clear();
                    }

                    // Kollisionsprüfung mit allen restlichen Sprites
                    foreach (Sprite sprite in area.Sprites)
                    {
                        // Kollision mit sich selbst ausschließen
                        if (sprite == character) continue;

                        // Überschneidung berechnen
                        Vector2 distance = (sprite.Position + sprite.Move) - (character.Position + character.Move);

                        // Ermittlung der angreifbaren Items.
                        if (attacker != null && sprite is IAttackable &&
                            distance.Length() - attacker.AttackRange - sprite.Radius < 0f)
                        {
                            attacker.AttackableSprites.Add(sprite as IAttackable);
                        }

                        // Ermittlung der interagierbaren Items.
                        if (interactor != null && sprite is IInteractable &&
                            //(sprite as IInteractable).OnInteract != null &&
                            distance.Length() - interactor.InteractionRange - sprite.Radius < 0f)
                        {
                            interactor.InteractableSprites.Add(sprite as IInteractable);
                        }

                        float overlap = sprite.Radius + character.Radius - distance.Length();

                        // Bei Überschneidung reagieren
                        if (overlap > 0f)
                        {
                            Vector2 resolution = distance * (overlap / distance.Length());
                            if (sprite.Fixed && !character.Fixed)
                            {
                                // Sprite fixiert
                                character.Move -= resolution;
                            }
                            else if (!sprite.Fixed && character.Fixed)
                            {
                                // Character fixiert
                                sprite.Move += resolution;
                            }
                            else if (!sprite.Fixed && !character.Fixed)
                            {
                                // keins fixiert
                                float totalMass = character.Mass + sprite.Mass;
                                // Neuberechnung anhand der Masse
                                character.Move -= resolution * (sprite.Mass / totalMass);
                                sprite.Move += resolution * (character.Mass / totalMass);
                            }
                        }
                    }

                    // Kollisionsprüfung mit allen Items
                    foreach (Item item in area.Items)
                    {
                        // Überschneidung berechnen
                        Vector2 distance = item.Position - (character.Position + character.Move);
                        float overlap = item.Radius + character.Radius - distance.Length();

                        // Bei Überschneidung reagieren
                        if (overlap > 0f)
                        {
                            // Kombination aus Collectable und Iventory
                            if (item is ICollectable && character is IInventory)
                            {
                                ICollectable collectable = item as ICollectable;

                                //  -> Character sammelt Item ein
                                actions.Add(() =>
                                {
                                    area.Items.Remove(item);
                                    (character as IInventory).Inventory.Add(item);
                                    item.Position = Vector2.Zero;
                                });

                                // Event aufrufen
                                if (collectable.OnCollect != null)
                                    collectable.OnCollect(_gameEngine, character);
                            }
                        }
                    }
                }

                foreach (Sprite sprite in area.Sprites)
                {
                    bool collision = false;
                    int loops = 0;

                    do
                    {
                        Vector2 position = sprite.Position + sprite.Move;
                        int minCellX = (int)(position.X - sprite.Radius);
                        int maxCellX = (int)(position.X + sprite.Radius);
                        int minCellY = (int)(position.Y - sprite.Radius);
                        int maxCellY = (int)(position.Y + sprite.Radius);

                        collision = false;
                        float minImpact = float.MaxValue;
                        CollisionAxis minAxis = CollisionAxis.None;

                        // Schleife über alle betroffenen Zellen zur Ermittlung der ersten Kollision
                        for (int x = minCellX; x <= maxCellX; x++)
                        {
                            for (int y = minCellY; y <= maxCellY; y++)
                            {
                                // Zellen ignorieren, die den Spieler nicht blockieren
                                if (!area.IsCellBlocked(x, y)) continue;

                                // Zellen ignorieren, die vom Spieler nicht berührt werden
                                if ((position.X - sprite.Radius) > (x + 1) || (position.X + sprite.Radius) < x ||
                                    (position.Y - sprite.Radius) > (y + 1) || (position.Y + sprite.Radius) < y)
                                    continue;

                                collision = true;

                                // Kollision auf X-Achse ermitteln
                                float diffX = float.MaxValue;
                                if (sprite.Move.X > 0) diffX = position.X + sprite.Radius - x + _gap; // Sprite nach rechts
                                if (sprite.Move.X < 0) diffX = position.X - sprite.Radius - (x + 1) - _gap; // Sprite nach links
                                float impactX = 1f - (diffX / sprite.Move.X);

                                // Kollision auf Y-Achse ermitteln
                                float diffY = float.MaxValue;
                                if (sprite.Move.Y > 0) diffY = position.Y + sprite.Radius - y + _gap; // Sprite nach unten
                                if (sprite.Move.Y < 0) diffY = position.Y - sprite.Radius - (y + 1) - _gap; // Sprite nach oben
                                float impactY = 1f - (diffY / sprite.Move.Y);

                                CollisionAxis axis = CollisionAxis.None;
                                float impact = 0;
                                if (impactX > impactY)
                                {
                                    axis = CollisionAxis.X;
                                    impact = impactX;
                                }
                                else
                                {
                                    axis = CollisionAxis.Y;
                                    impact = impactY;
                                }

                                // Früheste Kollision?
                                if (impact < minImpact)
                                {
                                    minImpact = impact;
                                    minAxis = axis;
                                }
                            }
                        }

                        // Es gab eine Kollision
                        if (collision)
                        {
                            if (minAxis == CollisionAxis.X) sprite.Move *= new Vector2(minImpact, 1f);
                            if (minAxis == CollisionAxis.Y) sprite.Move *= new Vector2(1f, minImpact);
                        }
                        loops++;
                    } while (collision && loops < 2);

                    // Finaler Move-Vektor auf die Position anwenden.
                    sprite.Position += sprite.Move;
                    sprite.Move = Vector2.Zero;

                    // Portal anwenden (nur Player)
                    if (area.Portals != null && sprite is Player)
                    {
                        Player player = sprite as Player;
                        bool inPortal = false;

                        foreach (Portal portal in area.Portals)
                        {
                            if (player.Position.X > portal.Box.Left &&
                                player.Position.X <= portal.Box.Right &&
                                player.Position.Y > portal.Box.Top &&
                                player.Position.Y <= portal.Box.Bottom)
                            {
                                inPortal = true;
                                if (player.InPortal) continue;

                                // Zielarea und Portal finden
                                Area destinationArea = World.Areas.First(a => a.Identifier.Equals(portal.DestinationArea));
                                // ToDo: Portale liefern nicht immer auf die gleiche Map zurück
                                Portal destinationPortal = destinationArea.Portals.First(p => p.DestinationArea.Equals(area.Identifier));

                                // Neue Spielerposition finden
                                Vector2 position = new Vector2(
                                    destinationPortal.Box.X + (destinationPortal.Box.Width / 2f),
                                    destinationPortal.Box.Y + (destinationPortal.Box.Height / 2f));

                                // Transfer in andere Area vorbereiten
                                actions.Add(() =>
                                {
                                    area.Sprites.Remove(sprite);
                                    destinationArea.Sprites.Add(sprite);
                                    sprite.Position = position;
                                });
                            }
                        }

                        player.InPortal = inPortal;
                    }

                    // Interaktionen durchführen
                    if (sprite is IInteractor)
                    {
                        IInteractor interactor = sprite as IInteractor;
                        if (interactor.InteractSignal)
                        {
                            // Alle Items in der Nähe aufrufen
                            foreach (IInteractable interactable in interactor.InteractableSprites)
                            {
                                if (interactable.OnInteract != null)
                                    interactable.OnInteract(_gameEngine, interactor, interactable);
                            }
                        }
                        interactor.InteractSignal = false;
                    }

                    // Angriff durchführen
                    if (sprite is IAttacker)
                    {
                        IAttacker attacker = sprite as IAttacker;
                        if (attacker.AttackSignal && attacker.Recovery <= TimeSpan.Zero)
                        {
                            // Alle Items in der Nähe schlagen
                            foreach (IAttackable attackable in attacker.AttackableSprites)
                            {
                                attackable.Hitpoints -= attacker.AttackValue;
                                if (attackable.OnHit != null)
                                    attackable.OnHit(_gameEngine, attacker, attackable);
                            }

                            // Schlagerholung anstoßen
                            attacker.Recovery = attacker.TotalRecovery;
                        }
                        attacker.AttackSignal = false;
                    }

                    // Randkorrektur
                    if (sprite.Position.X < 0f)
                        sprite.Position += new Vector2(-1 * sprite.Position.X, 0f);
                    if (sprite.Position.X > area.Width)
                        sprite.Position -= new Vector2(sprite.Position.X - area.Width, 0f);
                    if (sprite.Position.Y < 0f)
                        sprite.Position += new Vector2(0f, -1 * sprite.Position.Y);
                    if (sprite.Position.Y > area.Height)
                        sprite.Position -= new Vector2(0f, sprite.Position.Y - area.Height);
                }
            }

            // Transfers durchführen
            foreach (Action action in actions)
            {
                action();
            }

            #endregion

            base.Update(gameTime);
        }

        private enum CollisionAxis
        {
            None,
            X,
            Y
        }
    }
}
