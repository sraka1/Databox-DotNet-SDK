using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Databox.Integration.Sample
{
    /// <summary>
    /// This is a sample of using the Databox integration library.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            IDataboxService svc = new DataboxService();
            Console.WriteLine(svc.APIKey);
            Console.WriteLine(svc.UniqueUrl);

            svc.Push(
                new KPI("Kpi1", "value 1"),
                new KPI("KPI2", "Value 2"),
                new KPI("KPI3", "Value 3"));

            svc.Disconnect();

            svc = new DataboxService("API Key", "http://posttestserver.com/post.php?status_code=500");
            ((DataboxService)svc).OnError += new DataboxService.OnErrorDelegate(Program_OnError);
            
            svc.Push(new KPI[] {
                new KPI("Kpi1", "Value 1"),
                new KPI("Kpi2", "Value 2")
            });

            Console.Read();

            svc.Disconnect();
        }

        static void Program_OnError(PushResult result, KPI[] failedPayload)
        {
            Console.WriteLine("Failed: \r\n{0},\n{1}", result.Code, result.Content);
        }
    }
}
