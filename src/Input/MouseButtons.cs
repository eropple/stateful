using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ed.Stateful.Input
{
    /// <summary>
    /// For some reason, XNA overloads mouse buttons into the Keys enum. We
    /// shall not do that.
    /// </summary>
    public enum MouseButtons
    {
        /// <summary>
        /// Left mouse button.
        /// </summary>
        Left,
        /// <summary>
        /// Right mouse button.
        /// </summary>
        Right,
        /// <summary>
        /// Middle mouse button (scroll wheel click).
        /// </summary>
        Middle,
        /// <summary>
        /// Fourth mouse button (hardware-dependent).
        /// </summary>
        XButton1,
        /// <summary>
        /// Fifth mouse button (hardware-dependent).
        /// </summary>
        XButton2
    }
}
