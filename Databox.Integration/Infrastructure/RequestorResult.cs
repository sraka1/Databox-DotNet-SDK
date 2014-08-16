using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Databox.Integration.Infrastructure
{
    public class RequestorResult
    {
        public RequestorResult(int code)
        {
            this.Code = code;
        }

        public RequestorResult(HttpStatusCode code, string responseContent) : this((int)code)
        {
            this.Content = responseContent;
        }

        public int Code { get; private set; }
        public string Content { get; private set; }
        public bool Succeess = true;
    }
}
