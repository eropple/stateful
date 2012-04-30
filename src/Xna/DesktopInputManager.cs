using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ed.Stateful.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Ed.Stateful.Xna
{
    /// <summary>
    /// A polling-based input manager for MonoGame on Windows, Linux, and MacOS.
    /// </summary>
    /// <remarks>
    /// Probably can hoist some code out of this and share it between the desktop
    /// and mobile versions.
    /// </remarks>
    public class DesktopInputManager : IInputEventer
    {
        private Dictionary<Int32, GamePadState> _lastGamePadStates;
        private KeyboardState _lastKeyboardState;
        private MouseState _lastMouseState;

        private PlayerIndex[] _playerIndexes = (PlayerIndex[])Enum.GetValues(typeof(PlayerIndex));
        private Buttons[] _buttons = (Buttons[])Enum.GetValues(typeof(Buttons));
        private MouseButtons[] _mouseButtons = (MouseButtons[])Enum.GetValues(typeof(MouseButtons));
        private Keys[] _keys = (Keys[])Enum.GetValues(typeof(Keys));

        public event GamePadConnectionChanged ControllerConnected;
        public event GamePadConnectionChanged ControllerDisconnected;
        public event GamePadThumbStickMoved ControllerLeftThumbstickMoved;
        public event GamePadThumbStickMoved ControllerRightThumbstickMoved;
        public event GamePadTriggerMoved ControllerLeftTriggerMoved;
        public event GamePadTriggerMoved ControllerRightTriggerMoved;
        public event GamePadButtonStateChanged ControllerButtonPressed;
        public event GamePadButtonStateChanged ControllerButtonReleased;

        public event KeyboardKeyStateChanged KeyPressed;
        public event KeyboardKeyStateChanged KeyReleased;

        public event MouseMoved MouseMoved;
        public event MouseScrollWheelChanged MouseScrollWheelMoved;
        public event MouseButtonStateChanged MouseButtonPressed;
        public event MouseButtonStateChanged MouseButtonReleased;

        protected DesktopInputManager()
        {
            _lastGamePadStates = new Dictionary<Int32, GamePadState>(_playerIndexes.Length);
            foreach (PlayerIndex idx in _playerIndexes)
            {
                _lastGamePadStates.Add((Int32)idx, GamePad.GetState(idx));
            }
            _lastKeyboardState = Keyboard.GetState();
            _lastMouseState = Mouse.GetState();
        }

        public void Update(Int64 delta)
        {
            foreach (PlayerIndex idx in _playerIndexes)
            {
                this.ProcessGamePad(idx);
            }
            this.ProcessKeyboard();
            this.ProcessMouse();
        }





        private void ProcessKeyboard()
        {
            KeyboardState state = Keyboard.GetState();
            KeyboardState lastState = _lastKeyboardState;

            Boolean ctrl = state.IsKeyDown(Keys.LeftControl) || state.IsKeyDown(Keys.RightControl);
            Boolean alt = state.IsKeyDown(Keys.LeftAlt) || state.IsKeyDown(Keys.RightAlt);
            Boolean shift = state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift);

            foreach (Keys k in _keys)
            {
                if (state.IsKeyDown(k) && lastState.IsKeyUp(k) &&
                    this.KeyPressed != null)
                {
                    this.KeyPressed(k, ctrl, alt, shift);
                    continue;
                }
                if (state.IsKeyUp(k) && lastState.IsKeyDown(k) &&
                    this.KeyReleased != null)
                {
                    this.KeyReleased(k, ctrl, alt, shift);
                    continue;
                }
            }
            
            _lastKeyboardState = state;
        }

        private void ProcessMouse()
        {
            MouseState state = Mouse.GetState();
            MouseState lastState = _lastMouseState;

            Point p = new Point(state.X, state.Y);
            Point q = new Point(lastState.X, lastState.Y);

            Point delta = new Point(state.X - lastState.X, state.Y - lastState.Y);

            if ( (delta.X != 0 || delta.Y != 0) && this.MouseMoved != null)
            {
                this.MouseMoved(delta, p);
            }

            this.CheckMouseButton(p, MouseButtons.Left,     state.LeftButton,   lastState.LeftButton);
            this.CheckMouseButton(p, MouseButtons.Middle,   state.MiddleButton, lastState.MiddleButton);
            this.CheckMouseButton(p, MouseButtons.Right,    state.RightButton,  lastState.RightButton);
            this.CheckMouseButton(p, MouseButtons.XButton1, state.XButton1,     lastState.XButton1);
            this.CheckMouseButton(p, MouseButtons.XButton2, state.XButton2,     lastState.XButton2);

            Int32 detents = state.ScrollWheelValue - lastState.ScrollWheelValue;
            if (detents != 0 && this.MouseScrollWheelMoved != null)
            {
                this.MouseScrollWheelMoved(detents);
            }
            
            _lastMouseState = state;
        }

        private void CheckMouseButton(Point currentPosition, MouseButtons button,
                                      ButtonState newState, ButtonState oldState)
        {
            if (newState == ButtonState.Pressed && oldState == ButtonState.Released &&
                this.MouseButtonPressed != null)
            {
                this.MouseButtonPressed(button, currentPosition);
                return;
            }
            if (newState == ButtonState.Released && oldState == ButtonState.Pressed &&
                this.MouseButtonReleased != null)
            {
                this.MouseButtonReleased(button, currentPosition);
            }
        }

        private void ProcessGamePad(PlayerIndex idx)
        {
            GamePadState state = GamePad.GetState(idx);
            GamePadState lastState = _lastGamePadStates[(Int32)idx];

            if (state.IsConnected == true && lastState.IsConnected == false && this.ControllerDisconnected != null)
            {
                this.ControllerDisconnected(idx);
                return;
            }
            if (state.IsConnected == false && lastState.IsConnected == true && this.ControllerConnected != null)
            {
                this.ControllerConnected(idx);
                return;
            }

            if (state.IsConnected == false) return;


            Vector2 leftDelta = state.ThumbSticks.Left - lastState.ThumbSticks.Left;
            if (leftDelta != Vector2.Zero && this.ControllerLeftThumbstickMoved != null)
            {
                this.ControllerLeftThumbstickMoved(idx, leftDelta, state.ThumbSticks.Left);
            }
            Vector2 rightDelta = state.ThumbSticks.Right - lastState.ThumbSticks.Right;
            if (rightDelta != Vector2.Zero && this.ControllerRightThumbstickMoved != null)
            {
                this.ControllerRightThumbstickMoved(idx, rightDelta, state.ThumbSticks.Right);
            }

            Single leftTriggerDelta = state.Triggers.Left - lastState.Triggers.Left;
            if (Math.Abs(leftTriggerDelta) > Single.Epsilon && this.ControllerLeftTriggerMoved != null)
            {
                this.ControllerLeftTriggerMoved(idx, leftTriggerDelta, state.Triggers.Left);
            }
            Single rightTriggerDelta = state.Triggers.Right - lastState.Triggers.Right;
            if (Math.Abs(rightTriggerDelta) > Single.Epsilon && this.ControllerRightTriggerMoved != null)
            {
                this.ControllerRightTriggerMoved(idx, rightTriggerDelta, state.Triggers.Right);
            }

            foreach (Buttons b in _buttons)
            {
                if (state.IsButtonDown(b) && lastState.IsButtonUp(b) &&
                    this.ControllerButtonPressed != null)
                {
                    this.ControllerButtonPressed(idx, b);
                    continue;
                }
                if (state.IsButtonUp(b) && lastState.IsButtonDown(b) &&
                    this.ControllerButtonReleased != null)
                {
                    this.ControllerButtonReleased(idx, b);
                    continue;
                }
            }


            _lastGamePadStates.Remove((Int32)idx);
            _lastGamePadStates.Add((Int32)idx, state);
        }

        public static readonly DesktopInputManager Instance;
        static DesktopInputManager()
        {
            Instance = new DesktopInputManager();
        }
    }
}
