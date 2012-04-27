using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Ed.Stateful.Input
{
    /// <summary>
    /// An interface implemented by whatever is serving up events to your game.
    /// Must be provided to StateManager upon its instantiation.
    /// </summary>
    /// <remarks>
    /// The default implementations sit on top of the MonoGame/XNA APIs and
    /// poll them whenever their Update() is called, then diff any changes and
    /// dispatch them.
    /// </remarks>
    public interface IInputEventer
    {
        /// <summary>
        /// Event fired when a controller is connected.
        /// </summary>
        event GamePadConnectionChanged ControllerConnected;
        /// <summary>
        /// Event fired when a controller is disconnected.
        /// </summary>
        event GamePadConnectionChanged ControllerDisconnected;
        /// <summary>
        /// Event fired when the left thumbstick moves on any controller. This is slightly
        /// platform-dependent; for example, dead zones may differ from platform
        /// to platform.
        /// </summary>
        event GamePadThumbStickMoved ControllerLeftThumbstickMoved;
        /// <summary>
        /// Event fired when the right thumbstick moves on any controller. This is slightly
        /// platform-dependent; for example, dead zones may differ from platform
        /// to platform.
        /// </summary>
        event GamePadThumbStickMoved ControllerRightThumbstickMoved;
        /// <summary>
        /// Event fired when the left trigger moves on any controller.
        /// </summary>
        event GamePadTriggerMoved ControllerLeftTriggerMoved;
        /// <summary>
        /// Event fired when the left right moves on any controller.
        /// </summary>
        event GamePadTriggerMoved ControllerRightTriggerMoved;
        /// <summary>
        /// Event fired when a button is pressed on any controller.
        /// </summary>
        event GamePadButtonStateChanged ControllerButtonPressed;
        /// <summary>
        /// Event fired when a button is released on any controller.
        /// </summary>
        event GamePadButtonStateChanged ControllerButtonReleased;

        /// <summary>
        /// Event fired when a key is pressed on the keyboard.
        /// </summary>
        event KeyboardKeyStateChanged KeyPressed;
        /// <summary>
        /// Event fired when a key is released on the keyboard.
        /// </summary>
        event KeyboardKeyStateChanged KeyReleased;

        /// <summary>
        /// Event fired when the mouse moves.
        /// </summary>
        event MouseMoved MouseMoved;
        /// <summary>
        /// Event fired when the scroll wheel moves.
        /// </summary>
        event MouseScrollWheelChanged MouseScrollWheelMoved;
        /// <summary>
        /// Event fired when any mouse button is pressed.
        /// </summary>
        event MouseButtonStateChanged MouseButtonPressed;
        /// <summary>
        /// Event fired when any mouse button is released.
        /// </summary>
        event MouseButtonStateChanged MouseButtonReleased;
    }

    /// <summary>
    /// Delegate to handle controller connection or disconnection.
    /// </summary>
    /// <remarks>
    /// Exact state is implied by the method call on your Controller.
    /// </remarks>
    /// <param name="index">Index of the controller whose state has changed.</param>
    public delegate void GamePadConnectionChanged(PlayerIndex index);
    /// <summary>
    /// Delegate to handle thumbstick movement.
    /// </summary>
    /// <remarks>
    /// Which thumbstick moved is implied by the name of the method call on your Controller.
    /// </remarks>
    /// <param name="index">Index of the controller whose state has changed.</param>
    /// <param name="delta">Relative change in position.</param>
    /// <param name="newPosition">New absolute position.</param>
    public delegate void GamePadThumbStickMoved(PlayerIndex index, Vector2 delta, Vector2 newPosition);
    /// <summary>
    /// Delegate to handle trigger movement.
    /// </summary>
    /// <remarks>
    /// Which trigger moved is implied by the name of the method call on your Controller.
    /// </remarks>
    /// <param name="index">Index of the controller whose state has changed.</param>
    /// <param name="delta">Relative change in position.</param>
    /// <param name="newPosition">New absolute position.</param>
    public delegate void GamePadTriggerMoved(PlayerIndex index, Single delta, Single newPosition);
    /// <summary>
    /// Delegate to handle button interaction.
    /// </summary>
    /// <remarks>
    /// Exact button state is implied by the name of the method call on your Controller.
    /// </remarks>
    /// <param name="index">Index of the controller whose state has changed.</param>
    /// <param name="button">The button whose state has changed.</param>
    public delegate void GamePadButtonStateChanged(PlayerIndex index, Buttons button);

    public delegate void KeyboardKeyStateChanged(Keys key, Boolean ctrl, Boolean alt, Boolean shift);

    /// <summary>
    /// Delegate to handle mouse movement.
    /// </summary>
    /// <param name="delta">Change in position of the cursor.</param>
    /// <param name="newPosition">New absolute position of the cursor.</param>
    public delegate void MouseMoved(Point delta, Point newPosition);
    /// <summary>
    /// Delegate to handle state changes on mouse buttons.
    /// </summary>
    /// <remarks>
    /// Exact button state is implied by the name of the method call on your Controller.
    /// </remarks>
    /// <param name="button">The button pressed or released.</param>
    /// <param name="currentPosition">The absolute position of the cursor.</param>
    public delegate void MouseButtonStateChanged(MouseButtons button, Point currentPosition);
    /// <summary>
    /// Delegate to handle scroll wheel movement.
    /// </summary>
    /// <param name="delta">Amount of change in the scroll wheel, in detents.</param>
    public delegate void MouseScrollWheelChanged(Int32 delta);
}

