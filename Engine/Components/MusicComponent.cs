using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Engine.Components
{
    /// <summary>
    /// Komponente zur Musikwiedergabe
    /// </summary>
    public class MusicComponent : GameComponent
    {
        //private Engine _gameEngine;

        // Gibt die Zeitspanne in ms an die für einen Fade benötigt werden soll.
        private readonly float _totalFadeTime = 1500f; // 1,5 Sekunden
        // Gibt die Maximallautstärke für die Hintergrundsongs an
        private readonly float _maxVolume;
        // Hält die Liste verfügbarer Songs
        private readonly Dictionary<string, SoundEffect> _songs;
        // Hält die Instanz des aktuell laufenden Songs
        private SoundEffectInstance _currentSong;
        // Hält den Soundeffect des aktuellen Songs
        private SoundEffect _currentEffect;
        // Hält den Soundeffekt des kommenden Songs
        private SoundEffect _nextEffect;
        // Gibt den Soundeffect des letzten Areacalls an
        private SoundEffect _areaEffect;
        // Gibt an, ob das Menü ofen ist
        private bool _menu;

        public MusicComponent(Engine gameEngine) : base(gameEngine)
        {
            //_gameEngine = gameEngine;
            _maxVolume = 0.0f;

            // Songs laden
            _songs = new Dictionary<string, SoundEffect>
            {
                {"town", gameEngine.Content.Load<SoundEffect>("Music/townloop")},
                {"menu", gameEngine.Content.Load<SoundEffect>("Music/menuloop")},
                {"wood", gameEngine.Content.Load<SoundEffect>("Music/woodloop")},
                {"house", gameEngine.Content.Load<SoundEffect>("Music/houseloop")}
            };
        }

        public override void Update(GameTime gameTime)
        {
            // Override verhindern
            if (_currentEffect == _nextEffect)
                _nextEffect = null;

            // Ausfaden
            if (_currentEffect != null && _nextEffect != null)
            {
                float currentVolume = _currentSong.Volume;
                currentVolume -= (float)gameTime.ElapsedGameTime.TotalMilliseconds / _totalFadeTime;
                if (currentVolume <= 0f)
                {
                    // Ausschalten
                    _currentSong.Volume = 0;
                    _currentSong.Stop();
                    _currentSong.Dispose();
                    _currentSong = null;
                    _currentEffect = null;
                }
                else
                {
                    // Leiser
                    _currentSong.Volume = currentVolume;
                }
            }

            // Einschalten
            if (_currentEffect == null && _nextEffect != null)
            {
                _currentEffect = _nextEffect;
                _nextEffect = null;

                // Initialisieren mit Lautstärke 0
                _currentSong = _currentEffect.CreateInstance();
                _currentSong.IsLooped = true;
                _currentSong.Volume = 0f;
                _currentSong.Play();
            }

            // Einfaden
            if (_currentEffect != null && _nextEffect == null && _currentSong != null && _currentSong.Volume < _maxVolume)
            {
                float currentVolume = _currentSong.Volume;
                currentVolume += (float)gameTime.ElapsedGameTime.TotalMilliseconds / _totalFadeTime;
                currentVolume = Math.Min(currentVolume, _maxVolume);
                _currentSong.Volume = currentVolume;
            }
        }

        /// <summary>
        /// Spielt den angegebenen Song ab
        /// </summary>
        /// <param name="song"></param>
        public void Play(string song)
        {
            // Den laufenden Song stoppen
            //if (currentSong != null)
            //{
            //    currentSong.Stop();
            //    currentSong.Dispose();
            //}

            // Den neuen Song laden
            SoundEffect soundEffect;
            if (_songs.TryGetValue(song, out soundEffect))
            {
                // Den Areaeffect auf diesen umstellen
                _areaEffect = soundEffect;

                // Sollte das Menü nicht offen sein, dann den Song einreihen
                if (!_menu)
                {
                    _nextEffect = soundEffect;
                }


                //currentSong = soundEffect.CreateInstance();
                //currentSong.IsLooped = true;
                //currentSong.Volume = volume;
                //currentSong.Play();
            }
        }

        public void OpenMenu()
        {
            // Menusong einreihen
            _nextEffect = _songs["menu"];
            _menu = true;
        }

        public void CloseMenu()
        {
            // Vorherigen einreihen
            _nextEffect = _areaEffect;
            _menu = false;
        }
    }
}