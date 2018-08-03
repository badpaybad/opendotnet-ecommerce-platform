using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainDrivenDesign.TestDomain.Core.Redis
{
    [TestClass]
    public class QueueTest
    {
        bool _isStop = false;
        string queueName = "testqueue";

        [TestMethod]
        public void EnqueueDequeue()
        {
            //enqueue
            var counter = 0;
            var stw = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                counter++;
                DomainDrivenDesign.Core.Redis.RedisServices.RedisDatabase
                    .ListLeftPush(queueName,
                        counter + "-" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            stw.Stop();
            Console.WriteLine("Enqueue 1000 records in "+ stw.ElapsedMilliseconds);
            stw= Stopwatch.StartNew();
            //dequeue
            var d = DomainDrivenDesign.Core.Redis.RedisServices.RedisDatabase
                .ListRightPop(queueName);

            while (d.HasValue)
            {
                Console.WriteLine(d);
            }
            stw.Stop();
            Console.WriteLine("Dequeue 1000 records in " + stw.ElapsedMilliseconds);
        }

        [TestMethod]
        public void Do()
        {
           
            new Thread(() =>
            {
                var counter = 0;
                while (!_isStop)
                {
                    counter++;
                    DomainDrivenDesign.Core.Redis.RedisServices.RedisDatabase
                    .ListLeftPush(queueName,
                     counter + "-" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }).Start();

            new Thread(() =>
            {
                while (!_isStop)
                {
                    var d = DomainDrivenDesign.Core.Redis.RedisServices.RedisDatabase
                    .ListRightPop(queueName);

                    Console.WriteLine(d);
                    //Thread.Sleep(300);
                }
            }).Start();

            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);

            }

            _isStop = true;
        }
    }
}
