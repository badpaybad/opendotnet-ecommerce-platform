using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenDesign.Core.Utils
{
    public static class HttpClientManager
    {
        static List<HttpClientItem> _resources = new List<HttpClientItem>();

        static ConcurrentQueue<string> _jsonDataQueue = new ConcurrentQueue<string>();

        static HttpClientManager()
        {
            //init httpclient pool to use
            lock (_resources)
            {
                for (int i = 0; i < 100; i++)
                {
                    _resources.Add(new HttpClientItem());
                }
            }

            new Thread(() =>
            {
                // multi thread - create other thread to process data
                while (true)
                {
                    try
                    {
                        var httpClient = GetAvaiable();
                        if (httpClient != null)
                        {
                            try
                            {
                                httpClient.Lock();
                                string data;
                                if (_jsonDataQueue.TryDequeue(out data))
                                {
                                    try
                                    {
                                        httpClient.DoSomething(data);
                                    }
                                    catch
                                    {
                                        //if fail enqueue to reprocess
                                        _jsonDataQueue.Enqueue(data);
                                    }
                                }
                            }
                            finally
                            {
                                httpClient.Release();
                            }

                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        Thread.Sleep(1);
                    }
                }

            }).Start();
        }

        static HttpClientItem GetAvaiable()
        {
            lock (_resources)
            {
                return _resources.FirstOrDefault(i => i.InUse == false);
            }
        }

        static void AddDataToProcess(string jsonData)
        {
            _jsonDataQueue.Enqueue(jsonData);
        }

    }

    public class HttpClientItem
    {
        public bool InUse;
        private HttpClient _httpClient;

        public HttpClientItem()
        {
            _httpClient = new HttpClient();
        }

        public void DoSomething(string jsonData)
        {
            //eg: jsonData only url
            _httpClient.BaseAddress = new Uri(jsonData);
            var xxx = _httpClient.GetStringAsync(jsonData).Result;
            //do more with httpclient
        }


        public void Lock()
        {
            InUse = true;
        }

        public void Release()
        {
            InUse = false;
        }
    }
}
