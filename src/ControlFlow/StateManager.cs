using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ed.Stateful.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Ed.Stateful.ControlFlow
{
    public class StateSystem
    {
        protected readonly LinkedList<Controller> States;
        protected readonly IInputEventer Input;

        /// <summary>
        /// Fired after a controller's LoadContent but before it's pushed onto
        /// the state stack.
        /// </summary>
        public event ControllerPushedDelegate OnControllerPush;
        /// <summary>
        /// Fired after a controller is popped off the state stack but before it
        /// is disposed.
        /// </summary>
        public event ControllerPoppedDelegate OnControllerPop;

        /// <summary>
        /// Fired when there are no states left in the state stack. This is where
        /// you should hook in your cleanup and shutdown code.
        /// </summary>
        public event StateManagerEmptiedDelegate OnEmpty;

        public StateSystem(IInputEventer input, Controller baseController)
            : base()
        {
            Input = input;
            States = new LinkedList<Controller>();
            States.AddLast(baseController);

            Input.KeyPressed += new KeyboardKeyStateChanged(KeyPressed);
            Input.KeyReleased += new KeyboardKeyStateChanged(KeyReleased);
            Input.MouseMoved += new MouseMoved(MouseMoved);
            Input.MouseScrollWheelMoved += new MouseScrollWheelChanged(MouseScrollWheelMoved);
            Input.MouseButtonPressed += new MouseButtonStateChanged(MouseButtonPressed);
            Input.MouseButtonReleased += new MouseButtonStateChanged(MouseButtonReleased);
            Input.ControllerConnected += new GamePadConnectionChanged(ControllerConnected);
            Input.ControllerDisconnected += new GamePadConnectionChanged(ControllerDisconnected);
            Input.ControllerLeftThumbstickMoved += new GamePadThumbStickMoved(ControllerLeftThumbstickMoved);
            Input.ControllerRightThumbstickMoved += new GamePadThumbStickMoved(ControllerRightThumbstickMoved);
            Input.ControllerLeftTriggerMoved += new GamePadTriggerMoved(ControllerLeftTriggerMoved);
            Input.ControllerRightTriggerMoved += new GamePadTriggerMoved(ControllerRightTriggerMoved);
            Input.ControllerButtonPressed += new GamePadButtonStateChanged(ControllerButtonPressed);
            Input.ControllerButtonReleased += new GamePadButtonStateChanged(ControllerButtonReleased);
        }

        public void Push(Controller controller)
        {
            if (controller.IsDisposed)
            {
                throw new InvalidOperationException("You cannot push back onto the stack a previously popped state.");
            }

            controller.LoadContent();

            if (this.OnControllerPush != null)
                this.OnControllerPush(controller, States.Last.Value);

            States.AddLast(controller);
        }

        public Controller Pop()
        {
            if (States.Count == 0)
            {
                throw new InvalidOperationException("Cannot pop from the state stack when no states are left.");
            }
            Controller last = States.Last.Value;
            States.RemoveLast();

            if (this.OnControllerPop != null)
                this.OnControllerPop(last, States.Last.Value);

            last.Dispose();
            return last;
        }

        /// <summary>
        /// Pops the previous top of the state stack and replaces it with a new one. Useful for switching maps,
        /// etc. where you'd rather not keep the old one around.
        /// </summary>
        public Controller PopAndPush(Controller controller)
        {
            Controller c = this.Pop();
            this.Push(controller);
            return c;
        }

        public void Update(Int64 delta)
        {
            if (States.Count < 1)
            {
                if (this.OnEmpty == null)
                {
                    throw new StatefulExitException("States.Count == 1; exiting.");
                }
            }

            LinkedListNode<Controller> node = States.Last;

            while (node != null)
            {
                node.Value.RunUpdate(delta, node == States.Last);
                node = node.Previous;
            }
        }
        public void Draw(Int64 delta)
        {
            LinkedListNode<Controller> node = States.Last;

            do
            {
                if (node.Value.StopLowerDraws == true)
                {
                    break;
                }
                node = node.Previous;
            } while (node != null);

            if (node == null)
            {
                node = States.First;
            }

            do
            {
                node.Value.RunDraw(delta, node == States.Last);
                node = node.Next;
            } while (node != null);
        }

        #region Input Handling (spammy)
        void ControllerButtonReleased(PlayerIndex index, Buttons button)
        {
            States.Last.Value.ControllerButtonReleased(index, button);
        }

        void ControllerButtonPressed(PlayerIndex index, Buttons button)
        {
            States.Last.Value.ControllerButtonPressed(index, button);
        }

        void ControllerRightTriggerMoved(PlayerIndex index, float delta, float newPosition)
        {
            States.Last.Value.ControllerRightTriggerMoved(index, delta, newPosition);
        }

        void ControllerLeftTriggerMoved(PlayerIndex index, float delta, float newPosition)
        {
            States.Last.Value.ControllerLeftTriggerMoved(index, delta, newPosition);
        }

        void ControllerRightThumbstickMoved(PlayerIndex index, Vector2 delta, Vector2 newPosition)
        {
            States.Last.Value.ControllerRightThumbstickMoved(index, delta, newPosition);
        }

        void ControllerLeftThumbstickMoved(PlayerIndex index, Vector2 delta, Vector2 newPosition)
        {
            States.Last.Value.ControllerLeftThumbstickMoved(index, delta, newPosition);
        }

        void ControllerDisconnected(PlayerIndex index)
        {
            States.Last.Value.ControllerDisconnected(index);
        }

        void ControllerConnected(PlayerIndex index)
        {
            States.Last.Value.ControllerConnected(index);
        }

        void MouseButtonReleased(MouseButtons button, Point currentPosition)
        {
            States.Last.Value.MouseButtonReleased(button, currentPosition);
        }

        void MouseButtonPressed(MouseButtons button, Point currentPosition)
        {
            States.Last.Value.MouseButtonPressed(button, currentPosition);
        }

        void MouseScrollWheelMoved(int delta)
        {
            States.Last.Value.MouseScrollWheelMoved(delta);
        }

        void MouseMoved(Point delta, Point newPosition)
        {
            States.Last.Value.MouseMoved(delta, newPosition);
        }

        void KeyReleased(Keys key, bool ctrl, bool alt, bool shift)
        {
            States.Last.Value.KeyReleased(key, ctrl, alt, shift);
        }

        void KeyPressed(Keys key, bool ctrl, bool alt, bool shift)
        {
            States.Last.Value.KeyPressed(key, ctrl, alt, shift);
        }
        #endregion
    }

    /// <summary>
    /// Delegate to handle when a controller is popped off the state stack.
    /// </summary>
    /// <param name="poppedController">The controller popped off the stack (losing focus).</param>
    /// <param name="topController">The new top of the state stack (getting focus).</param>
    public delegate void ControllerPoppedDelegate(Controller poppedController, Controller topController);
    /// <summary>
    /// Delegate to handle when a controller is pushed onto the state stack.
    /// </summary>
    /// <param name="poppedController">The controller pushed onto the stack (gaining focus).</param>
    /// <param name="topController">The previous top of the state stack (losing focus).</param>
    public delegate void ControllerPushedDelegate(Controller pushedController, Controller formerTopController);

    /// <summary>
    /// Delegate to handle the case where there are no controllers left on the
    /// state stack. Should generally be used for cleanup/shutdown.
    /// </summary>
    public delegate void StateManagerEmptiedDelegate();
}
