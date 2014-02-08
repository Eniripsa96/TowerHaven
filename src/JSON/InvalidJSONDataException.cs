using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSON
{
    /// <summary>
    /// Exception for bad JSON data
    /// </summary>
    class InvalidJSONDataException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">error message</param>
        public InvalidJSONDataException(string message)
            : base(message)
        { }
    }
}
