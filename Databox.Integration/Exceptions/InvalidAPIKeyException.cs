using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Databox.Integration.Exceptions
{
    /// <summary>
    /// This exception is thrown when the API key is invalid or missing.
    /// </summary>
    public sealed class InvalidAPIKeyException : Exception
    {
        public InvalidAPIKeyException() : base("The API Key provided was invalid or missing.")
        {
        }
    }
}
