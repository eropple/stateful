using System;

namespace Ed.Stateful.ControlFlow
{
    /// <summary>
    /// Exception thrown when no states are left in the state stack; can be
    /// caught to be considered the "exit condition" but doing so is really ugly
    /// and I'd prefer that you attached a method to StateManager.OnStatesEmptied 
    /// instead.
    /// </summary>
    /// <remarks>
    /// This exception does not have any custom properties, 
    /// thus it does not implement ISerializable.
    /// </remarks>
    [Serializable]
    public class StatefulExitException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatefulExitException"/> class.
        /// </summary>
        public StatefulExitException() : base()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="StatefulExitException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public StatefulExitException(string message) : base(message)
        {}
        
        /// <summary>
        /// Initializes a new instance of the <see cref="StatefulExitException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public StatefulExitException(string message, Exception innerException) : base(message, innerException)
        {}
    }
}