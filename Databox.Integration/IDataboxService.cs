using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Databox.Integration
{
    /// <summary>
    /// The IDataboxService interface provides an abstraction
    /// for the service class that allows it to be easily integrated
    /// with DI tools.
    /// </summary>
    public interface IDataboxService
    {
        string APIKey { get; }
        string UniqueUrl { get; }
        void Push(params KPI[] kpi);

        void Disconnect();
    }

}
