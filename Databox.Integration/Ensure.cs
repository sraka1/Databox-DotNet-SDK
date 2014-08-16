using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Databox.Integration
{
    public static class Ensure
    {
        public static void IsNotNull<TException>(string value) where TException : System.Exception
        {
            if (String.IsNullOrEmpty(value))
            {
                throw default(TException);
            }
        }

        internal static void IsNotNullOrEmpty(Array array)
        {
            if (array == null)
                throw new System.ArgumentNullException();
            if (array.Length == 0)
                throw new System.ArgumentNullException();
        }

        internal static void IsNotNull<TException>(object value) where TException : System.Exception
        {
            if (value == null)
            {
                throw default(TException);
            }
        }

        internal static void IsNotNull(object value)
        {
            IsNotNull<ArgumentNullException>(value);
        }
    }
}
