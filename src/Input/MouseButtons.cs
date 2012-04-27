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
        Left,
        Right,
        Middle,
        XButton1,
        XButton2
    }
}
