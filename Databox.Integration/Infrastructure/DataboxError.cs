using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Databox.Integration.Infrastructure
{
    public class DataboxError
    {
        public DataboxError(int code, string content)
        {
            switch (code)
            {
                //case HttpStatusCode.BadRequest:
                //    this.Message = 
                case (int)HttpStatusCode.OK:
                    throw new ArgumentOutOfRangeException("Not an error.");
                default:
                    this.Message = code.ToString();
                    this.Content = content;
                    break;
            }
        }

        public string Message { get; set; }
        public string Content { get; set; }

        internal static DataboxError FromResponse(RequestorResult result)
        {
            if (result.Code == (int)HttpStatusCode.OK) return null;
            return new DataboxError(result.Code, result.Content);
        }
    }
}
