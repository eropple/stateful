using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ed.Stateful.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Ed.Stateful.ControlFlow
{
    /// <summary>
    /// The base controller in Stateful.
    /// </summary>
    /// <remarks>
    /// I wouldn't recommend using this for your applications directory; you're
    /// probably better off creating a custom class from which you base your own
    /// logic.
    /// </remarks>
    public abstract class Controller : IDisposable
    {
        public bool StopLowerDraws { get; set; }
        public bool StopLowerUpdates { get; set; }

        protected Controller()
        {
        }


        protected internal abstract void LoadContent();
        protected internal abstract void Update(Int64 delta, Boolean topOfStack);
        protected internal abstract void Draw(Int64 delta, Boolean topOfStack);

        internal void RunUpdate(Int64 delta, Boolean topOfStack)
        {
            Update(delta, topOfStack);
        }

        internal void RunDraw(Int64 delta, Boolean topOfStack)
        {
            Draw(delta, topOfStack);
        }

        #region Input Handling (spammy; defaults to virtual)
        protected internal virtual void ControllerButtonReleased(PlayerIndex index, Buttons button)
        {
        }

        protected internal virtual void ControllerButtonPressed(PlayerIndex index, Buttons button)
        {
        }

        protected internal virtual void ControllerRightTriggerMoved(PlayerIndex index, float delta, float newPosition)
        {
        }

        protected internal virtual void ControllerLeftTriggerMoved(PlayerIndex index, float delta, float newPosition)
        {
        }

        protected internal virtual void ControllerRightThumbstickMoved(PlayerIndex index, Vector2 delta, Vector2 newPosition)
        {
        }

        protected internal virtual void ControllerLeftThumbstickMoved(PlayerIndex index, Vector2 delta, Vector2 newPosition)
        {
        }

        protected internal virtual void ControllerDisconnected(PlayerIndex index)
        {
        }

        protected internal virtual void ControllerConnected(PlayerIndex index)
        {
        }

        protected internal virtual void MouseButtonReleased(MouseButtons button, Point currentPosition)
        {
        }

        protected internal virtual void MouseButtonPressed(MouseButtons button, Point currentPosition)
        {
        }

        protected internal virtual void MouseScrollWheelMoved(int delta)
        {
        }

        protected internal virtual void MouseMoved(Point delta, Point newPosition)
        {
        }

        protected internal virtual void KeyReleased(Keys key, bool ctrl, bool alt, bool shift)
        {
        }

        protected internal virtual void KeyPressed(Keys key, bool ctrl, bool alt, bool shift)
        {
        }
        #endregion

        public Boolean IsDisposed { get; protected set; }
        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            HandleDispose();
        }
        protected abstract void HandleDispose();
    }
}
