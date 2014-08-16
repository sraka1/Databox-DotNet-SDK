using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Databox.Integration;

namespace CoffeePush
{
    class Program
    {
        static void Main(string[] args)
        {
            Databox.Integration.IDataboxService svc = new DataboxService("apikey", "uniqueurl"); 
        }
    }
}
