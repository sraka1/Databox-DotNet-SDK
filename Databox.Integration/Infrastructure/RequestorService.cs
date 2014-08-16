using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Databox.Integration.Infrastructure
{
	public class RequestorService
	{
		private IDataboxService fDataboxService;
		public RequestorService(IDataboxService databoxService)           
		{
			Ensure.IsNotNull(databoxService);

            // double check we weren't tricked with the API key
            Ensure.IsNotNull<Exceptions.InvalidAPIKeyException>(databoxService.APIKey);

			this.fDataboxService = databoxService;
		}

		public RequestorResult PostJson(string url, object obj)
		{
			// convert the object into a JSON string
			var data = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var payload = Encoding.UTF8.GetBytes(data);

            var req = GetWebRequest(url, "POST");

            Stream dataStream = req.GetRequestStream();
            dataStream.Write(payload, 0, payload.Length);
            dataStream.Close();

            return ExecuteRequest(req);
		}

        private WebRequest GetWebRequest(string url, string method)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            
            request.Headers.Add("Authorization", GetAuthorizationHeaderValue());

            request.ContentType = "application/json";
            request.UserAgent = "Databox-CSharp-SDK/1.0";


            return request;
        }

        private RequestorResult ExecuteRequest(WebRequest request)
        {
            try
            {
                using (var response = request.GetResponse())
                {
                    string responseContent = ReadStream(response.GetResponseStream());
                    return new RequestorResult(((HttpWebResponse)response).StatusCode, responseContent);
                }
            }
            catch (WebException we)
            {
                if (we.Response != null)
                {
                    // get the error
                    var content = ReadStream(we.Response.GetResponseStream());
                    return new RequestorResult(((HttpWebResponse)we.Response).StatusCode, content)
                    {
                        Succeess = false
                    };
                }

                // guess we don't know what it is, so panic and rethrow
                throw;
            }
        }

        private string GetAuthorizationHeaderValue()
        {
            //var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:", fDataboxService.APIKey)));
            var token = String.Format("{0}:", fDataboxService.APIKey);
            return string.Format("Basic {0}", token);
        }

        private string ReadStream(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
	}
}
