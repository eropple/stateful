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
        event GamePadConnectionChanged ControllerConnected;
        event GamePadConnectionChanged ControllerDisconnected;
        event GamePadThumbStickMoved ControllerLeftThumbstickMoved;
        event GamePadThumbStickMoved ControllerRightThumbstickMoved;
        event GamePadTriggerMoved ControllerLeftTriggerMoved;
        event GamePadTriggerMoved ControllerRightTriggerMoved;
        event GamePadButtonStateChanged ControllerButtonPressed;
        event GamePadButtonStateChanged ControllerButtonReleased;

        event KeyboardKeyStateChanged KeyPressed;
        event KeyboardKeyStateChanged KeyReleased;

        event MouseMoved MouseMoved;
        event MouseScrollWheelChanged MouseScrollWheelMoved;
        event MouseButtonStateChanged MouseButtonPressed;
        event MouseButtonStateChanged MouseButtonReleased;
    }

    public delegate void GamePadConnectionChanged(PlayerIndex index);
    public delegate void GamePadThumbStickMoved(PlayerIndex index, Vector2 delta, Vector2 newPosition);
    public delegate void GamePadTriggerMoved(PlayerIndex index, Single delta, Single newPosition);
    public delegate void GamePadButtonStateChanged(PlayerIndex index, Buttons button);

    public delegate void KeyboardKeyStateChanged(Keys key, Boolean ctrl, Boolean alt, Boolean shift);

    public delegate void MouseMoved(Point delta, Point newPosition);
    public delegate void MouseButtonStateChanged(MouseButtons button, Point currentPosition);
    public delegate void MouseScrollWheelChanged(Int32 delta);
}

