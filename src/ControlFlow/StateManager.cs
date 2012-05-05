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

        private Boolean _skipDrawFrame = false;

        /// <summary>
        /// Fired before a controller's LoadContent, after which it is pushed
        /// onto the stack. (Recommended insertion point for any "readonly"
        /// data that should be in your controller, like a Contentious context.)
        /// </summary>
        public event ControllerPushedDelegate OnControllerPush;
        /// <summary>
        /// Fired after a controller is popped off the state stack but before it
        /// is disposed.
        /// </summary>
        public event ControllerPoppedDelegate OnControllerPop;
        
        public event PreUpdateDelegate OnUpdateStart;
        public event PostUpdateDelegate OnUpdateEnd;
        
        public event PreDrawDelegate OnDrawStart;
        public event PostDrawDelegate OnDrawEnd;

        /// <summary>
        /// Fired when there are no states left in the state stack. This is where
        /// you should hook in your cleanup and shutdown code.
        /// </summary>
        public event StateManagerEmptiedDelegate OnEmpty;

        public StateSystem(IInputEventer input)
            : base()
        {
            Input = input;
            States = new LinkedList<Controller>();

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

            if (this.OnControllerPush != null)
                this.OnControllerPush(controller, (States.Count > 0) ? States.Last.Value : null);

            controller.LoadContent();

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
                this.OnControllerPop(last, (States.Count > 0) ? States.Last.Value : null);

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

        /// <summary>
        /// If called during the Update() phase, will cause the draw phase to be
        /// skipped this frame. Useful when a controller is pushed or popped from
        /// the stack.
        /// </summary>
        public void SkipDrawThisFrame()
        {
            _skipDrawFrame = true;
        }

        public void Update(Int64 delta)
        {
            if (States.Count < 1)
            {
                if (this.OnEmpty != null)
                {
                    OnEmpty();
                }
                else
                {
                    throw new StatefulExitException("States.Count == 0");
                }
            }

            _skipDrawFrame = false;
            
            if (OnUpdateStart != null)
                OnUpdateStart(delta);

            LinkedListNode<Controller> node = States.Last;

            while (node != null)
            {
                node.Value.RunUpdate(delta, node == States.Last);
                node = node.Previous;
            }
            
            if (OnUpdateEnd != null)
                OnUpdateEnd(delta);
        }
        public void Draw(Int64 delta)
        {
            if (_skipDrawFrame)
                return;
            
            if (OnDrawStart != null)
                OnDrawStart(delta);
            

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
            
            if (OnDrawEnd != null)
                OnDrawEnd(delta);
        }

        #region Input Handling (spammy)
        void ControllerButtonReleased(PlayerIndex index, Buttons button)
        {
            if (States.Count == 0)
                return;
            States.Last.Value.ControllerButtonReleased(index, button);
        }

        void ControllerButtonPressed(PlayerIndex index, Buttons button)
        {
            if (States.Count == 0)
                return;
            States.Last.Value.ControllerButtonPressed(index, button);
        }

        void ControllerRightTriggerMoved(PlayerIndex index, float delta, float newPosition)
        {
            if (States.Count == 0)
                return;
            States.Last.Value.ControllerRightTriggerMoved(index, delta, newPosition);
        }

        void ControllerLeftTriggerMoved(PlayerIndex index, float delta, float newPosition)
        {
            if (States.Count == 0)
                return;
            States.Last.Value.ControllerLeftTriggerMoved(index, delta, newPosition);
        }

        void ControllerRightThumbstickMoved(PlayerIndex index, Vector2 delta, Vector2 newPosition)
        {
            if (States.Count == 0)
                return;
            States.Last.Value.ControllerRightThumbstickMoved(index, delta, newPosition);
        }

        void ControllerLeftThumbstickMoved(PlayerIndex index, Vector2 delta, Vector2 newPosition)
        {
            if (States.Count == 0)
                return;
            States.Last.Value.ControllerLeftThumbstickMoved(index, delta, newPosition);
        }

        void ControllerDisconnected(PlayerIndex index)
        {
            if (States.Count == 0)
                return;
            States.Last.Value.ControllerDisconnected(index);
        }

        void ControllerConnected(PlayerIndex index)
        {
            if (States.Count == 0)
                return;
            States.Last.Value.ControllerConnected(index);
        }

        void MouseButtonReleased(MouseButtons button, Point currentPosition)
        {
            if (States.Count == 0)
                return;
            States.Last.Value.MouseButtonReleased(button, currentPosition);
        }

        void MouseButtonPressed(MouseButtons button, Point currentPosition)
        {
            if (States.Count == 0)
                return;
            States.Last.Value.MouseButtonPressed(button, currentPosition);
        }

        void MouseScrollWheelMoved(int delta)
        {
            if (States.Count == 0)
                return;
            States.Last.Value.MouseScrollWheelMoved(delta);
        }

        void MouseMoved(Point delta, Point newPosition)
        {
            if (States.Count == 0)
                return;
            States.Last.Value.MouseMoved(delta, newPosition);
        }

        void KeyReleased(Keys key, bool ctrl, bool alt, bool shift)
        {
            if (States.Count == 0)
                return;
            States.Last.Value.KeyReleased(key, ctrl, alt, shift);
        }

        void KeyPressed(Keys key, bool ctrl, bool alt, bool shift)
        {
            if (States.Count == 0)
                return;
            States.Last.Value.KeyPressed(key, ctrl, alt, shift);
        }
        #endregion
    }

    /// <summary>
    /// Delegate to handle when a controller is popped off the state stack.
    /// </summary>
    /// <param name="poppedController">The controller popped off the stack (losing focus).</param>
    /// <param name="topController">
    /// The new top of the state stack (getting focus). This can be null if
    /// the last state has just been pushed off. If a state is not pushed onto
    /// the stack before the next Update(), will fire OnEmpty.
    /// </param>
    public delegate void ControllerPoppedDelegate(Controller poppedController, Controller topController);
    /// <summary>
    /// Delegate to handle when a controller is pushed onto the state stack.
    /// </summary>
    /// <param name="pushedController">The controller pushed onto the stack (gaining focus).</param>
    /// <param name="formerTopController">
    /// The previous top of the state stack (losing focus). If null, means that
    /// this is the first state push of the application /or/ the user is attempting
    /// to rescue a shutdown by pushing back onto an empty stack.
    /// </param>
    public delegate void ControllerPushedDelegate(Controller pushedController, Controller formerTopController);
 
    
    /// <summary>
    /// Delegate to handle events invoked before the controllers' Update()
    /// methods are fired.
    /// </summary>
    /// <param name="delta">Number of milliseconds since last Update() started.</param>
    public delegate void PreUpdateDelegate(Int64 delta);
    /// <summary>
    /// Delegate to handle events invoked before the controllers' Draw()
    /// methods are fired.
    /// </summary>
    /// /// <param name="delta">Number of milliseconds since last Draw() started.</param>
    public delegate void PreDrawDelegate(Int64 delta);
    /// <summary>
    /// Delegate to handle events invoked after the controllers' Update()
    /// methods are fired.
    /// </summary>
    /// <param name="delta">Number of milliseconds since last Update() started.</param>
    public delegate void PostUpdateDelegate(Int64 delta);
    /// <summary>
    /// Delegate to handle events invoked after the controllers' Draw()
    /// methods are fired.
    /// </summary>
    /// /// <param name="delta">Number of milliseconds since last Draw() started.</param>
    public delegate void PostDrawDelegate(Int64 delta);
    
    /// <summary>
    /// Delegate to handle the case where there are no controllers left on the
    /// state stack. Should generally be used for cleanup/shutdown.
    /// </summary>
    public delegate void StateManagerEmptiedDelegate();
}
