using System;
using Engine.Interface;
using Engine.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Rendering
{
    /// <summary>
    /// Item Renderer für die komplexeren Character-SpriteSheets.
    /// </summary>
    internal class CharacterRenderer : SpriteRenderer
    {
        private readonly Character _character;

        private Animation _animation;

        private Direction _direction;

        private int _frameCount;

        private int _animationRow;

        public CharacterRenderer(Character character, Camera camera, Texture2D texture, SpriteFont font)
            : base(character, camera, texture, font, new Point(64, 64), 50, new Point(32, 55), 2f)
        {
            _character = character;
            _animation = Animation.Idle;
            _direction = Direction.South;
            _frameCount = 1;
            _animationRow = 8;
        }

        /// <summary>
        /// Render-Methode für diesen Character
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch Referenz</param>
        /// <param name="offset">Der Offset der View</param>
        /// <param name="gameTime">Aktuelle GameTime</param>
        /// <param name="highlight">Soll der Name angezeigt werden?</param> 
        public override void Draw(SpriteBatch spriteBatch, Point offset, GameTime gameTime, bool highlight)
        {
            // kommende Animation ermitteln
            Animation nextAnimation = Animation.Idle;
            if (_character.Velocity.Length() > 0f)
            {
                nextAnimation = Animation.Walk;

                // Diagonale durch das Koordinatensystem
                // X>Y => rechte, obere Hälfte
                // -X>Y => linke, obere Hälfte

                // Spieler in Bewegung -> Ausrichtung des Spielers ermitteln
                if (_character.Velocity.X > _character.Velocity.Y)
                {
                    // Rechts oben
                    if (-_character.Velocity.X > _character.Velocity.Y)
                        // Links oben -> Oben
                        _direction = Direction.North;
                    else
                        // Rechts unten -> Rechts
                        _direction = Direction.East;
                }
                else
                {
                    // Links unten
                    if (-_character.Velocity.X > _character.Velocity.Y)
                        // Links oben -> links
                        _direction = Direction.West;
                    else
                        // Rechts unten -> Unten
                        _direction = Direction.South;
                }
            }

            // Schlag-Animation
            if (_character is IAttacker && (_character as IAttacker).Recovery > TimeSpan.Zero)
            {
                nextAnimation = Animation.Hit;
            }

            // Für den Fall, dass dieser Character Tod ist
            if (_character is IAttackable && (_character as IAttackable).Hitpoints <= 0)
            {
                nextAnimation = Animation.Die;
                _direction = Direction.North;
            }

            // Animation bei Änderung resetten
            if (_animation != nextAnimation)
            {
                _animation = nextAnimation;
                AnimationTime = 0;
                switch (_animation)
                {
                    case Animation.Walk:
                        _frameCount = 9;
                        _animationRow = 8;
                        break;
                    case Animation.Die:
                        _frameCount = 6;
                        _animationRow = 20;
                        break;
                    case Animation.Idle:
                        _frameCount = 1;
                        _animationRow = 8;
                        break;
                    case Animation.Hit:
                        _frameCount = 6;
                        _animationRow = 12;
                        break;
                }
            }

            // Animationszeile ermitteln
            int row = _animationRow + (int)_direction;
            if (_animation == Animation.Die)
                row = _animationRow;

            // Frame ermitteln
            int frame = 0; // Standard für Idle
            switch (_animation)
            {
                case Animation.Walk:
                    // Animationsgeschwindigkeit an Laufgeschwindigkeit gekoppelt
                    float speed = _character.Velocity.Length() / _character.MaxSpeed;
                    AnimationTime += (int)(gameTime.ElapsedGameTime.TotalMilliseconds * speed);
                    frame = (AnimationTime / FrameTime) % _frameCount;
                    break;
                case Animation.Hit:
                    // TODO: Animationsverlauf definieren
                    IAttacker attacker = Sprite as IAttacker;
                    double animationPosition = 1d - (attacker.Recovery.TotalMilliseconds / attacker.TotalRecovery.TotalMilliseconds);
                    frame = (int)(_frameCount * animationPosition);
                    break;
                case Animation.Die:
                    // Animation stoppt mit dem letzten Frame
                    AnimationTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds / 2;
                    frame = Math.Min((AnimationTime / FrameTime), _frameCount - 1);
                    break;
            }

            // Bestimmung der Position des Spieler-Mittelpunktes in View-Koordinaten
            int posX = (int)((Sprite.Position.X) * Camera.Scale) - offset.X;
            int posY = (int)((Sprite.Position.Y) * Camera.Scale) - offset.Y;
            //int radius = (int)(Sprite.Radius * Camera.Scale);

            Vector2 scale = new Vector2(Camera.Scale / FrameSize.X, Camera.Scale / FrameSize.Y) * FrameScale;

            Rectangle sourceRectangle = new Rectangle(
                frame * FrameSize.X,
                row * FrameSize.Y,
                FrameSize.X,
                FrameSize.Y);

            Rectangle destinationRectangle = new Rectangle(
                posX - (int)(ItemOffset.X * scale.X),
                posY - (int)(ItemOffset.Y * scale.Y),
                (int)(FrameSize.X * scale.X),
                (int)(FrameSize.Y * scale.Y));

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);

            // Highlight
            if (highlight && !string.IsNullOrEmpty(_character.Name))
            {
                Vector2 textSize = Font.MeasureString(_character.Name);
                Vector2 location = new Vector2(
                    posX - (int)(textSize.X / 2),
                    posY - (int)(ItemOffset.Y * scale.Y));
                spriteBatch.DrawString(Font, _character.Name, location, Color.White);
            }
        }
    }

    /// <summary>
    /// Auflistung möglicher Animationen
    /// </summary>
    internal enum Animation
    {
        Idle,
        Walk,
        Hit,
        Die
    }

    /// <summary>
    /// Auflistung von Blickrichtungen
    /// </summary>
    internal enum Direction
    {
        North = 0,
        West = 1,
        South = 2,
        East = 3
    }
}