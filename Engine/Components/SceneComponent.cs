using System.Collections.Generic;
using System.IO;
using System.Linq;
using Engine.Interface;
using Engine.Model;
using Engine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Components
{
    /// <summary>
    /// Game Komponente zur Ausgabe der Spiel-Szene an den Bildschirm.
    /// </summary>
    internal class SceneComponent : DrawableGameComponent
    {
        private readonly Engine _gameEngine;

        private readonly Dictionary<string, Texture2D> _textures;

        private readonly Dictionary<string, Texture2D> _items;
        private readonly Dictionary<Item, ItemRenderer> _itemRenderer;

        private readonly Dictionary<string, Texture2D> _sprites;
        private readonly Dictionary<Sprite, SpriteRenderer> _spriteRenderer;

        private SpriteBatch _spriteBatch;

        private Area _currentArea;

        private SpriteFont _font;

        /// <summary>
        /// Kamera-Einstellungen für diese Szene.
        /// </summary>
        private Camera Camera { get; set; }

        public SceneComponent(Engine gameEngine)
            : base(gameEngine)
        {
            _gameEngine = gameEngine;
            _textures = new Dictionary<string, Texture2D>();

            _items = new Dictionary<string, Texture2D>();
            _itemRenderer = new Dictionary<Item, ItemRenderer>();

            _sprites = new Dictionary<string, Texture2D>();
            _spriteRenderer = new Dictionary<Sprite, SpriteRenderer>();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Camera = new Camera(GraphicsDevice.Viewport.Bounds.Size);
            _font = Game.Content.Load<SpriteFont>("Fonts/KenPixel");

            //// Initiale Kameraposition (temporär)
            //Vector2 areaSize = new Vector2(
            //    gameEngine.Simulation.World.Areas[0].Width,
            //    gameEngine.Simulation.World.Areas[0].Height);
            //Camera.SetFocusExplizit(gameEngine.Local.Player.Position, areaSize);

            // Erforderliche Texturen ermitteln
            List<string> requiredTextures = new List<string>();
            List<string> requiredItems = new List<string>();
            List<string> requiredSprites = new List<string>();
            foreach (Area area in _gameEngine.Simulation.World.Areas)
            {
                // Tile-Textures
                foreach (Tile tile in area.Tiles.Values)
                {
                    if (!requiredTextures.Contains(tile.Texture))
                    {
                        requiredTextures.Add(tile.Texture);
                    }
                }

                // Item Textures
                foreach (Item item in area.Items)
                {
                    if (!string.IsNullOrEmpty(item.Texture) && !requiredItems.Contains(item.Texture))
                    {
                        requiredItems.Add(item.Texture);
                    }
                }

                // Sprite Textures
                foreach (Sprite sprite in area.Sprites)
                {
                    if (!string.IsNullOrEmpty(sprite.Texture) && !requiredSprites.Contains(sprite.Texture))
                    {
                        requiredSprites.Add(sprite.Texture);
                    }
                }
            }

            // Erforderliche Texturen laden
            foreach (string textureName in requiredTextures)
            {
                string path = "Tilesets/" + Path.GetFileNameWithoutExtension(textureName);
                _textures.Add(textureName, Game.Content.Load<Texture2D>(path));
            }

            // Erforderliche Items laden
            foreach (string itemName in requiredItems)
            {
                if (!_items.ContainsKey(itemName))
                {
                    string path = "Sprites/" + Path.GetFileNameWithoutExtension(itemName);
                    _items.Add(itemName, Game.Content.Load<Texture2D>(path));
                }
            }

            // Erforderliche Sprites laden
            foreach (string spriteName in requiredSprites)
            {
                string path = "Sprites/" + Path.GetFileNameWithoutExtension(spriteName);
                _sprites.Add(spriteName, Game.Content.Load<Texture2D>(path));
            }
        }

        public override void Update(GameTime gameTime)
        {
            //// Nur wenn Komponente aktiviert wurde.
            //if (!Enabled)
            //    return;

            //// Nur arbeiten, wenn es eine Welt, einen Player und eine aktive Area gibt.
            Area area = _gameEngine.Local.GetCurrentArea();
            //if (_gameEngine.Simulation.World == null || _gameEngine.Local.Player == null || area == null)
            //    return;

            if (_currentArea != area)
            {
                // Aktuelle Area wechseln
                _currentArea = area;

                // Initiale Kameraposition (temporär)
                Vector2 areaSize = new Vector2(_currentArea.Width, _currentArea.Height);
                Camera.SetFocusExplizit(_gameEngine.Local.Player.Position, areaSize);
            }

            // Platziert den Kamerafokus auf den Spieler.
            Camera.SetFocus(_gameEngine.Local.Player.Position);
        }

        public override void Draw(GameTime gameTime)
        {
            // Bildschirm leeren
            GraphicsDevice.Clear(_currentArea.Background);

            _spriteBatch.Begin();

            // Berechnet den Render-Offset mit Hilfe der Kamera-Einstellungen
            Point offset = (Camera.Offset * Camera.Scale).ToPoint();

            // Alle Layer der Render-Reihenfolge nach durchlaufen
            for (int l = 0; l < _currentArea.Layers.Length; l++)
            {
                RenderLayer(_currentArea, _currentArea.Layers[l], offset);
                if (l == 4)
                {
                    RenderSprites(_currentArea, offset, gameTime);
                }
            }

            _spriteBatch.End();
        }

        /// <summary>
        /// Rendert einen Layer der aktuellen Szene
        /// </summary>
        private void RenderLayer(Area area, Layer layer, Point offset)
        {
            // ToDo: Nur den sichtbaren Bereich rendern
            for (int x = 0; x < area.Width; x++)
            {
                for (int y = 0; y < area.Height; y++)
                {
                    // Prüfen, ob diese Zelle ein Tile enthält
                    int tileId = layer.Tiles[x, y];
                    if (tileId == 0)
                        continue;

                    // Tile ermitteln
                    Tile tile = area.Tiles[tileId];
                    Texture2D texture = _textures[tile.Texture];

                    // Position ermitteln
                    int offsetX = (int)(x * Camera.Scale) - offset.X;
                    int offsetY = (int)(y * Camera.Scale) - offset.Y;

                    // Zelle mit der Standard-Textur (Gras) ausmalen
                    _spriteBatch.Draw(texture, new Rectangle(offsetX, offsetY, (int)Camera.Scale, (int)Camera.Scale), tile.SourceRectangle, Color.White);
                }
            }
        }

        /// <summary>
        /// Rendert die Spielelemente der aktuellen Szene
        /// </summary>
        private void RenderSprites(Area area, Point offset, GameTime gameTime)
        {
            List<object> itemsSprites = new List<object>();
            itemsSprites.AddRange(area.Items);
            itemsSprites.AddRange(area.Sprites);

            // Von hinten nach vorne rendern
            foreach (object element in itemsSprites.OrderBy(i => ((IMapElement)i).Position.Y))
            {
                if (element is Item)
                {
                    Item item = element as Item;
                    if (item.Texture != null)
                    {
                        // Renderer ermitteln und ggf. neu erzeugen
                        ItemRenderer renderer;
                        if (!_itemRenderer.TryGetValue(item, out renderer))
                        {
                            // ACHTUNG: Hier können potentiell neue Sprites nachträglich hinzu kommen zu denen die Textur noch fehlt
                            // Das muss geprüft und ggf nachgeladen werden.
                            Texture2D texture = _items[item.Texture];

                            renderer = new SimpleItemRenderer(item, Camera, texture);

                            _itemRenderer.Add(item, renderer);
                        }

                        // Sprite rendern
                        renderer.Draw(_spriteBatch, offset, gameTime);
                    }
                }
                else if (element is Sprite)
                {
                    Sprite sprite = element as Sprite;
                    if (sprite.Texture != null)
                    {
                        // Renderer ermitteln und ggf. neu erzeugen
                        SpriteRenderer renderer;
                        if (!_spriteRenderer.TryGetValue(sprite, out renderer))
                        {
                            // ACHTUNG: Hier können potentiell neue Sprites nachträglich hinzu kommen zu denen die Textur noch fehlt
                            // Das muss geprüft und ggf nachgeladen werden.
                            Texture2D texture = _sprites[sprite.Texture];

                            if (sprite is Character)
                                renderer = new CharacterRenderer(sprite as Character, Camera, texture, _font);
                            else
                                renderer = new SimpleSpriteRenderer(sprite, Camera, texture, _font);

                            _spriteRenderer.Add(sprite, renderer);
                        }

                        // Ermitteln, ob Sprite im Interaktionsbereich ist
                        bool highlight = sprite is IInteractable && _gameEngine.Local.Player.InteractableSprites.Contains(sprite as IInteractable) ||
                            sprite is IAttackable && _gameEngine.Local.Player.AttackableSprites.Contains(sprite as IAttackable);

                        // Sprite rendern
                        renderer.Draw(_spriteBatch, offset, gameTime, highlight);
                    }
                }
            }

            // ToDo: Nicht mehr verwendete Renderer entfernen
        }
    }
}

