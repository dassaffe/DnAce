using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Engine.Components
{
    /// <summary>
    /// Game Komponente zur Verarbeitung von Spieler-Eingaben
    /// </summary>
    internal class InputComponent : GameComponent
    {
        private readonly Engine _gameEngine;
        private readonly Trigger _upTrigger;
        private readonly Trigger _downTrigger;
        private readonly Trigger _leftTrigger;
        private readonly Trigger _rightTrigger;
        private readonly Trigger _closeTrigger;
        private readonly Trigger _interactTrigger;
        private readonly Trigger _attackTrigger;
        private readonly Trigger _inventoryTrigger;

        /// <summary>
        /// Gibt an ob die Eingaben innerhalb dieses Update-Zykluses bereits abgearbeitet wurden.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gibt an ob der User nach oben gedrückt hat.
        /// </summary>
        public bool Up { get { return _upTrigger.Value; } }

        /// <summary>
        /// Gibt an ob der User nach unten gedrückt hat.
        /// </summary>
        public bool Down { get { return _downTrigger.Value; } }

        /// <summary>
        /// Gibt an ob der User nach links gedrückt hat.
        /// </summary>
        public bool Left { get { return _leftTrigger.Value; } }

        /// <summary>
        /// Gibt an ob der User nach rechts gedrückt hat.
        /// </summary>
        public bool Right { get { return _rightTrigger.Value; } }

        /// <summary>
        /// Gibt die vom Spieler gewünschte Bewegungsrichtung (normalisiert) an.
        /// </summary>
        public Vector2 Movement { get; private set; }

        /// <summary>
        /// Gibt an ob der User den Close-Button drückt.
        /// </summary>
        public bool Close { get { return _closeTrigger.Value; } }

        /// <summary>
        /// Gibt an ob der User den Inventar-Button drückt.
        /// </summary>
        public bool Inventory { get { return _inventoryTrigger.Value; } }

        /// <summary>
        /// Gibt an ob der User den Attack-Knopf drückt.
        /// </summary>
        public bool Attack { get { return _attackTrigger.Value; } }

        /// <summary>
        /// Gibt an ob der User den Interact-Knopf drückt.
        /// </summary>
        public bool Interact { get { return _interactTrigger.Value; } }

        public InputComponent(Engine gameEngine)
            : base(gameEngine)
        {
            _gameEngine = gameEngine;

            _upTrigger = new Trigger();
            _downTrigger = new Trigger();
            _leftTrigger = new Trigger();
            _rightTrigger = new Trigger();
            _closeTrigger = new Trigger();
            _inventoryTrigger = new Trigger();
            _attackTrigger = new Trigger();
            _interactTrigger = new Trigger();
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 movement = Vector2.Zero;
            bool up = false;
            bool down = false;
            bool left = false;
            bool right = false;
            bool close = false;
            bool inventory = false;
            bool attack = false;
            bool interact = false;

            // Gamepad Steuerung
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
            movement += gamePad.ThumbSticks.Left * new Vector2(1f, -1f);
            left |= gamePad.ThumbSticks.Left.X < -0.5f;
            right |= gamePad.ThumbSticks.Left.X > 0.5f;
            up |= gamePad.ThumbSticks.Left.Y > 0.5f;
            down |= gamePad.ThumbSticks.Left.Y < -0.5f;
            close |= gamePad.Buttons.B == ButtonState.Pressed;
            inventory |= gamePad.Buttons.Y == ButtonState.Pressed;
            attack |= gamePad.Buttons.X == ButtonState.Pressed;
            interact |= gamePad.Buttons.A == ButtonState.Pressed;

            // Keyboard Steuerung
            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Left)|| keyboard.IsKeyDown(Keys.A))
                movement += new Vector2(-1f, 0f);
            if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
                movement += new Vector2(1f, 0f);
            if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
                movement += new Vector2(0f, -1f);
            if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
                movement += new Vector2(0f, 1f);
            left |= keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A);
            right |= keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D);
            up |= keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W);
            down |= keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S);
            close |= keyboard.IsKeyDown(Keys.Escape) || keyboard.IsKeyDown(Keys.X);
            inventory |= keyboard.IsKeyDown(Keys.I);
            attack |= keyboard.IsKeyDown(Keys.LeftControl);
            interact |= keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Enter) || keyboard.IsKeyDown(Keys.C);

            // Mouse Steuerung
            MouseState mouse = Mouse.GetState();
            close |= mouse.RightButton == ButtonState.Pressed;
            inventory |= mouse.MiddleButton == ButtonState.Pressed;
            // Attack ist später gesondert zu behandeln Left + Right sollen darauf reagieren, je nach Möglichkeit
            //attack |= keyboard.IsKeyDown(Keys.LeftControl);
            interact |= mouse.LeftButton == ButtonState.Pressed;

            // Normalisierung der Bewegungsrichtung
            if (movement.Length() > 1f)
                movement.Normalize();

            // Properties setzen
            Movement = movement;
            _upTrigger.Value = up;
            _downTrigger.Value = down;
            _leftTrigger.Value = left;
            _rightTrigger.Value = right;
            _closeTrigger.Value = close;
            _inventoryTrigger.Value = inventory;
            _interactTrigger.Value = interact;
            _attackTrigger.Value = attack;

            // Handle-Flag zurück setzen.
            Handled = false;
        }

        /// <summary>
        /// Kapselt den Druck auf einen Button und sorgt dafür, 
        /// dass ein true nur ein einziges mal abgerufen werden kann.
        /// </summary>
        private class Trigger
        {
            // Speichert den letzten Wert aus set
            private bool _lastValue;

            // Speichert, ob der Trigger ausgelöst wurde.
            private bool _triggered;

            /// <summary>
            /// Liefert den Wert eines ausgelösten bool zurück.
            /// </summary>
            public bool Value
            {
                get
                {
                    // Gibt den Wert des Triggers aus und setzt den Auslöser auf false zurück.
                    bool result = _triggered;
                    _triggered = false;
                    return result;
                }
                set
                {
                    // Schaut, ob der Trigger neu gesetzt werden darf.
                    if (_lastValue != value)
                        _lastValue = _triggered = value;
                }
            }
        }
    }
}