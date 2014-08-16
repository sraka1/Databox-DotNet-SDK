using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Databox.Integration.Exceptions
{
    public class InvalidUniqueUrlException : Exception
    {
        public InvalidUniqueUrlException() : base("The specified URL is invalid.")
        {
        }
    }
}
