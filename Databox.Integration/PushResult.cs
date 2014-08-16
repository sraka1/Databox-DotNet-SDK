using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Databox.Integration
{
    /// <summary>
    /// Will hold data from the push.
    /// </summary>
    public class PushResult
    {
        public int Code { get; set; }
        public string Content { get; set; }
        public Infrastructure.DataboxError Error { get; set; }
    }
}
