using System;

namespace SaftPT.Tests.Helpers
{
    /// <summary>
    /// Throwed if the field is not found.
    /// </summary>
    /// <seealso cref="Exception" />
    public class FieldNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public FieldNotFoundException(string message) 
            : base(message) { }
    }
}