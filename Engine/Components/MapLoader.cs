using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Engine.Interface;
using Engine.Model;
using Engine.Screens;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Engine.Components
{
    internal class MapLoader
    {
        private Engine gameEngine;
        private string mapPath;

        public MapLoader(Engine gameEngine, string mapPath)
        {
            this.gameEngine = gameEngine;
            this.mapPath = mapPath;
        }

        public IEnumerable<Area> LoadAll()
        {
            // Alle json-Files im Map-Folder suchen
            //string mapPath = Path.Combine(gameEning, "Maps");
            string[] files = Directory.GetFiles(mapPath, "*.json");

            // Alle gefundenen json-Files laden
            Area[] result = new Area[files.Length];
            for (int i = 0; i < files.Length; i++)
                result[i] = LoadFromJson(files[i]);

            return result;
        }

        private Area LoadFromJson(string file)
        {
            using (Stream stream = File.OpenRead(file))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    // json Datei auslesen
                    string json = sr.ReadToEnd();

                    // Deserialisieren
                    FileArea result = JsonConvert.DeserializeObject<FileArea>(json);

                    // Neue Area öffnen und mit den Root-Daten füllen
                    FileLayer[] tileLayer = result.layers.Where(l => l.type == "tilelayer").ToArray();
                    FileLayer objectLayer = result.layers.FirstOrDefault(l => l.type == "objectgroup");
                    Area area = new Area(tileLayer.Length, result.width, result.height);


                    // Properties auslesen
                    if (result.properties != null)
                    {
                        area.Name = result.properties.MapName;
                        area.Song = result.properties.Song;
                        area.Identifier = result.properties.Identifier;
                    }

                    // Hintergrundfarbe interpretieren
                    area.Background = new Color(128, 128, 128);
                    if (!string.IsNullOrEmpty(result.backgroundcolor))
                    {
                        // Hexwerte als Farbwert parsen
                        area.Background = new Color(
                            Convert.ToInt32(result.backgroundcolor.Substring(1, 2), 16),
                            Convert.ToInt32(result.backgroundcolor.Substring(3, 2), 16),
                            Convert.ToInt32(result.backgroundcolor.Substring(5, 2), 16));
                    }

                    // Tiles zusammen suchen
                    foreach (FileTileset tileset in result.tilesets)
                    {
                        int start = tileset.firstgid;
                        int perRow = tileset.imagewidth / tileset.tilewidth;
                        int width = tileset.tilewidth;

                        for (int j = 0; j < tileset.tilecount; j++)
                        {
                            int x = j % perRow;
                            int y = j / perRow;

                            // Block-Status ermitteln
                            bool block = false;
                            if (tileset.tileproperties != null)
                            {
                                FileTileProperty property;
                                if (tileset.tileproperties.TryGetValue(j, out property))
                                    block = property.Block;
                            }

                            // Tile erstellen
                            Tile tile = new Tile
                            {
                                Texture = tileset.image,
                                SourceRectangle = new Rectangle(x * width, y * width, width, width),
                                Blocked = block
                            };

                            // In die Auflistung aufnehmen
                            area.Tiles.Add(start + j, tile);
                        }
                    }

                    // TileLayer erstellen
                    for (int l = 0; l < tileLayer.Length; l++)
                    {
                        FileLayer layer = tileLayer[l];

                        for (int i = 0; i < layer.data.Length; i++)
                        {
                            int x = i % area.Width;
                            int y = i / area.Width;
                            area.Layers[l].Tiles[x, y] = layer.data[i];
                        }
                    }

                    // Object Layer analysieren
                    if (objectLayer != null)
                    {
                        // Portals - Übertragungspunkte zu anderen Karten
                        foreach (Obj portal in objectLayer.objects.Where(o => o.type.ToLower() == "portal"))
                        {
                            Rectangle box = new Rectangle(
                                                portal.x / result.tilewidth,
                                                portal.y / result.tileheight,
                                                portal.width / result.tilewidth,
                                                portal.height / result.tileheight
                                            );

                            area.Portals.Add(new Portal { DestinationArea = portal.name, Box = box });
                        }

                        // Items (Spielelemente)
                        foreach (Obj item in objectLayer.objects.Where(o => o.type.ToLower() == "item"))
                        {
                            Vector2 pos = new Vector2(
                                (item.x + (item.width / 2f)) / result.tilewidth,
                                (item.y + (item.height / 2f)) / result.tileheight);

                            switch (item.name)
                            {
                                case "coin": area.Items.Add(new Item { Position = pos, Texture = "coin_silver", Name = "Münze", Icon = "coinicon" }); break;
                                case "goldencoin":
                                    area.Items.Add(new Item
                                    {
                                        Position = pos,
                                        Texture = "coin_gold",
                                        Name = "Goldene Münze",
                                        Icon = "coinicon",
                                        //OnCollect = (game, character) =>
                                        //{
                                        //    var quest = gameEngine.Simulation.World.Quests.SingleOrDefault(q => q.Name == "Heidis Quest");
                                        //    quest.Progress("return");
                                        //}
                                    }); break;
                            }
                        }

                        // NPCs + Enemys
                        foreach (Obj npc in objectLayer.objects.Where(o => (o.type.ToLower() == "npc") || (o.type.ToLower() == "enemy")))
                        {
                            Vector2 pos = new Vector2(
                                (npc.x + (npc.width / 2f)) / result.tilewidth,
                                (npc.y + (npc.height / 2f)) / result.tileheight);

                            string npcName = string.Empty;
                            float radius = 1f;
                            bool isFixed = false;
                            string icon = string.Empty;
                            string texture = string.Empty;
                            string moveType = string.Empty;

                            // Properties auslesen
                            if (npc.properties != null)
                            {
                                isFixed = npc.properties.Fixed;
                                icon = npc.properties.Icon;
                                npcName = npc.properties.NpcName;
                                radius = npc.properties.Radius;
                                texture = npc.properties.Texture;
                                moveType = npc.properties.MoveType;
                            }

                            if (npc.type.ToLower() == "enemy")
                                area.Sprites.Add(new Enemy(gameEngine)
                                {
                                    Fixed = isFixed,
                                    Icon = icon,
                                    MoveType = GetMoveTypeByString(moveType),
                                    Name = npcName,
                                    Position = pos,
                                    Radius = radius,
                                    Texture = texture
                                });
                            else
                            {
                                var newNpc = new Npc(gameEngine)
                                {
                                    Fixed = isFixed,
                                    Icon = icon,
                                    MoveType = GetMoveTypeByString(moveType),
                                    Name = npcName,
                                    Position = pos,
                                    Radius = radius,
                                    Texture = texture
                                };

                                newNpc.OnInteract = (engine, interactor, interactable) => gameEngine.Screen.ShowScreen(new ShoutScreen(gameEngine.Screen, newNpc, "Das ist ein Test"));

                                area.Sprites.Add(newNpc);
                            }
                        }

                        // Player (Startpunkte)
                        foreach (Obj player in objectLayer.objects.Where(o => o.type.ToLower() == "player"))
                        {
                            Vector2 pos = new Vector2((player.x + (player.width / 2)) / result.tilewidth, (player.y + (player.height / 2)) / result.tileheight);

                            area.Startpoints.Add(pos);
                        }
                    }

                    return area;
                }
            }
        }

        private static Sprite.MovementType GetMoveTypeByString(string moveType)
        {
            switch (moveType)
            {
                case "WalkAround":
                    return Sprite.MovementType.WalkAround;
                case "":
                case "NoMovement":
                default:
                    return Sprite.MovementType.NoMovement;
            }
        }

        // ReSharper disable InconsistentNaming
        // ReSharper disable ClassNeverInstantiated.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        /// <summary>
        /// Root Objekt der Area-Datei.
        /// </summary>
        private class FileArea
        {
            /// <summary>
            /// Hintergrundfarbe der Karte als Hexcode
            /// </summary>
            public string backgroundcolor { get; set; }

            /// <summary>
            /// Abzahl Zellen in der Breite
            /// </summary>
            public int width { get; set; }

            /// <summary>
            /// Anzahl Zellen in der Höhe
            /// </summary>
            public int height { get; set; }

            /// <summary>
            /// Breite eines einzelnen Tiles
            /// </summary>
            public int tilewidth { get; set; }

            /// <summary>
            /// Höhe eines einzelnen Tiles
            /// </summary>
            public int tileheight { get; set; }

            /// <summary>
            /// Auflistung der Layer.
            /// </summary>
            public FileLayer[] layers { get; set; }

            /// <summary>
            /// Auflistung der Tilesets.
            /// </summary>
            public FileTileset[] tilesets { get; set; }

            /// <summary>
            /// Auflistung zusätzlicher Properties
            /// </summary>
            public FileAreaProperty properties { get; set; }
        }

        /// <summary>
        /// Layerdaten
        /// </summary>
        private class FileLayer
        {
            /// <summary>
            /// Fortlaufende Index-Liste der Tiles.
            /// </summary>
            public int[] data { get; set; }

            /// <summary>
            /// Gibt den Layer-Typ an (tilelayer oder objectgroup)
            /// </summary>
            public string type { get; set; }

            /// <summary>
            /// Auflistung der Objekte (für den Fall eines Object-Layers)
            /// </summary>
            public Obj[] objects { get; set; }
        }

        /// <summary>
        /// Tilesetdaten
        /// </summary>
        private class FileTileset
        {
            /// <summary>
            /// Erste Id der enthaltenen Tiles.
            /// </summary>
            public int firstgid { get; set; }

            /// <summary>
            /// Name der Textur.
            /// </summary>
            public string image { get; set; }

            /// <summary>
            /// Breite eines einzelnen Tiles.
            /// </summary>
            public int tilewidth { get; set; }

            /// <summary>
            /// Breite des Bildes.
            /// </summary>
            public int imagewidth { get; set; }

            /// <summary>
            /// Anzahl enthaltener Tiles.
            /// </summary>
            public int tilecount { get; set; }

            /// <summary>
            /// Auflistung zusätzlicher Properties von Tiles.
            /// </summary>
            public Dictionary<int, FileTileProperty> tileproperties { get; set; }
        }

        /// <summary>
        /// Ein Object auf einem Object-Layer
        /// </summary>
        private class Obj
        {
            /// <summary>
            /// Name
            /// </summary>
            public string name { get; set; }

            /// <summary>
            /// Object-Type
            /// </summary>
            public string type { get; set; }

            /// <summary>
            /// X-Position
            /// </summary>
            public int x { get; set; }

            /// <summary>
            /// Y-Position
            /// </summary>
            public int y { get; set; }

            /// <summary>
            /// Breite
            /// </summary>
            public int width { get; set; }

            /// <summary>
            /// Höhe
            /// </summary>
            public int height { get; set; }

            public ObjProperties properties { get; set; }
        }

        /// <summary>
        /// Zusätzliche Properties eines Objektes
        /// </summary>
        private class ObjProperties
        {
            public bool Fixed { get; set; }
            public string Icon { get; set; }
            public string NpcName { get; set; }
            public float Radius { get; set; }
            public string Texture { get; set; }
            public string MoveType { get; set; }
        }

        /// <summary>
        /// Zusätzliche "Custom Properties"
        /// </summary>
        private class FileTileProperty
        {
            /// <summary>
            /// Gibt an ob das Tile den Spieler blockiert
            /// </summary>
            public bool Block { get; set; }
        }

        /// <summary>
        /// Zusätzliche Properties in der Area
        /// </summary>
        private class FileAreaProperty
        {
            /// <summary>
            /// Eindeutiger Identifier der Map (wird für Portal benutzt)
            /// </summary>
            public string Identifier { get; set; }

            /// <summary>
            /// Name der Karte
            /// </summary>
            public string MapName { get; set; }

            /// <summary>
            /// Name des zu verwendenden Songs.
            /// </summary>
            public string Song { get; set; }
        }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
        // ReSharper restore ClassNeverInstantiated.Local
        // ReSharper restore InconsistentNaming
    }
}