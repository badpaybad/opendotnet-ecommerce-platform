using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Core.FrontEnd.Controllers
{
    public class EventStreamController : Controller
    {
        readonly static ConcurrentDictionary<string, ConcurrentQueue<string>> _eventDatas = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

        public static void Push(string eventName, string data)
        {
            ConcurrentQueue<string> queue;
            if (_eventDatas.TryGetValue(eventName, out queue) && queue != null)
            {
                queue.Enqueue(data);
            }
            else
            {
                var concurrentQueue = new ConcurrentQueue<string>();
                concurrentQueue.Enqueue(data);
                _eventDatas[eventName] = concurrentQueue;
            }
        }
        
        public async Task Subscribe(string eventName)
        {
            Response.ContentType = "text/event-stream";
            Response.Expires = -1;
            
            while (true)
            {
                try
                {
                    ConcurrentQueue<string> queue;
                    if (_eventDatas.TryGetValue(eventName, out queue))
                    {
                        string data;
                        if (queue.TryDequeue(out data))
                        {
                            try
                            {
                                Response.Write("data:" + data);
                                Response.Write("\n\n");
                                Response.Flush();
                            }
                            catch
                            {
                                //handle error: try enqueue to send again
                                Push(eventName,data);
                            }
                        }
                    }
                }
                finally
                {
                    Thread.Sleep(500);
                    await Task.Delay(500);
                }
            }
        }

        [HttpPost]
        public JsonResult Publish(string eventName, string eventData)
        {
            Push(eventName, eventData);

            return  Json( new { Ok = true }, JsonRequestBehavior.AllowGet );
        }

        public ActionResult Index()
        {
            return View();
        }

    }
}