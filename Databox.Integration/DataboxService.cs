using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading;
using Databox.Integration.Infrastructure;
using System.Net;

namespace Databox.Integration
{
    /// <summary>
    /// Represents the key Databox Service communication point.
    /// It allows configuring the custom integration and pushing
    ///     data to that configuration. 
    /// </summary>
    public class DataboxService : IDataboxService
    {
        private Thread fThread;
        private List<KPI> fKPIQueue = new List<KPI>();
        private RequestorService fRequestorService = null;
        private RequestorService RequestorService
        {
            get
            {
                if (fRequestorService == null)
                    fRequestorService = new RequestorService(this);
                return fRequestorService;
            }
        }

        /// <summary>
        /// We are using this to make sure the thread doesn't run until there is something 
        /// in the queue, ready to be sent.
        /// </summary>
        private ManualResetEvent fSignal = new ManualResetEvent(false);

        /// <summary>
        /// Signals the thread to abort. This signal gets triggered upon destruction of this
        /// class' instance.
        /// </summary>
        private ManualResetEvent fTerminationSignal = new ManualResetEvent(false);
        private bool fThreadRunning = true;

        /// <summary>
        /// Constructs a new instance of the DataboxService,
        /// pulling all the configuration data out of the Application 
        /// Settings. It throws an <see cref="InvalidAPIKeyException"/>InvalidAPIKeyException</see>
        /// if the API key is missing, or invalid.
        /// </summary>
        public DataboxService()
        {
            try
            {
                this.APIKey = ConfigurationManager.AppSettings["DataboxApiKey"];
            }
            catch (NullReferenceException)
            {
                throw new Exceptions.InvalidAPIKeyException();
            }

            try
            {
                this.UniqueUrl = ConfigurationManager.AppSettings["DataboxUniqueUrl"];
            }
            catch (NullReferenceException)
            {
                throw new Exceptions.InvalidUniqueUrlException();
            }

            StartThread();
        }

        ~DataboxService() { Disconnect(); }

        /// <summary>
        /// Constructs a new instance of the DataboxService,
        /// with a specified API key that will be used for communication.
        /// </summary>
        /// <param name="apiKey">The API key given upon registering as a developer.</param>
        /// <param name="uniqueUrl">The Unique URL provided when registering the custom integration.</param>
        public DataboxService(string apiKey, string uniqueUrl)
        {
            this.APIKey = apiKey;
            this.UniqueUrl = uniqueUrl;

            StartThread();
        }

        private void StartThread()
        {
            this.fThread = new Thread(PushData);
            fThread.Start();
        }

        /// <summary>
        /// This is the function that gets executed in the thred
        /// to sync. 
        /// </summary>
        private void PushData()
        {
            while (fThreadRunning)
            {
                // don't burn CPU cycles
                ManualResetEvent.WaitAny(new WaitHandle[] { fTerminationSignal, fSignal });

                while (true)
                {
                    List<object> data = new List<object>();
                    List<KPI> copy = new List<KPI>();

                    lock (fKPIQueue)
                    {
                        // optimize locking, break on no items
                        if (fKPIQueue.Count == 0) break;
                        // get the new candidates / copy them
                        copy.AddRange(fKPIQueue);
                        // clear the queue for now
                        fKPIQueue.RemoveAll(x => true);
                    }

                    // transform the items
                    copy.ForEach(x => data.Add(new
                    {
                        key = x.Key,
                        value = x.Value,
                        date = x.Timestamp.ToString ( "s", System.Globalization.CultureInfo.InvariantCulture ),
                        attributes = x.Attributes
                    }));

                    // sync the candidate
                    var r = PushCandidates(data);
                    if (r.Code != (int)HttpStatusCode.OK)
                    {
                        // fire the event
                        this.FireOnError(copy.ToArray(), r);
                        System.Diagnostics.Debug.WriteLine("Error, retrying");
                    }
                    System.Diagnostics.Debug.WriteLine(r.Content);

                    // remove all from the copy
                    copy.RemoveAll(x => true);

                }

                // put the thread back into sleep mode
                fSignal.Reset();
            }
        }

        private PushResult PushCandidates(List<object> data)
        {
            System.Diagnostics.Debug.WriteLine("Pushing candidates.");

            // if we fail, we'll re-add the kpi values, keep in mind
            var result = RequestorService.PostJson(this.UniqueUrl, new
            {
                data = data.ToArray()
            });

            return new PushResult()
            {
                Code = result.Code,
                Content = result.Content,
                Error = DataboxError.FromResponse(result)
            };
        }

        private void EnsureAPIKeyIsValid(string key)
        {
            Ensure.IsNotNull<Exceptions.InvalidAPIKeyException>(key);
        }

        private string fApiKey = String.Empty;
        public string APIKey
        {
            get { return fApiKey; }
            set
            {
                EnsureAPIKeyIsValid(value);
                fApiKey = value;
            }
        }

        private string fUniqueUrl = String.Empty;
        public string UniqueUrl
        {
            get { return fUniqueUrl; }
            set
            {
                Ensure.IsNotNull<Exceptions.InvalidUniqueUrlException>(value);
                fUniqueUrl = value;
            }
        }

        public void Push(params KPI[] kpi)
        {
            Ensure.IsNotNullOrEmpty(kpi);

            InternalPush(kpi);

            fSignal.Set();
        }

        private void InternalPush(params KPI[] kpi)
        {
            // enqueue the data
            lock (fKPIQueue)
            {
                foreach (var k in kpi)
                {
                    fKPIQueue.Add(k);
                }
            }
        }


        public void Disconnect()
        {
            fThreadRunning = false;
            fTerminationSignal.Set();
        }

        public delegate void OnErrorDelegate(PushResult result, KPI[] failedPayload);

        /// <summary>
        /// This event is triggered if there is an error pushing data. 
        /// </summary>
        public event OnErrorDelegate OnError;

        private void FireOnError(KPI[] kpi, PushResult r)
        {
            if (this.OnError != null)
            {
                OnError(r, kpi);
            }
        }

    }
}