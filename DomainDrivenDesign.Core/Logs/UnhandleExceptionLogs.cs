using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DomainDrivenDesign.Core.Logs
{
    public static class UnhandleExceptionLogs
    {
        static ConcurrentQueue<string> _errors = new ConcurrentQueue<string>();

        public static void Log(string error)
        {
            _errors.Enqueue(error);
        }

        static UnhandleExceptionLogs()
        {
            var dirLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/Logs");
            if (Directory.Exists(dirLog) == false)
            {
                Directory.CreateDirectory(dirLog);
            }

            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                       
                        string err;
                        List<string> errors = new List<string>();
                        while (_errors.TryDequeue(out err) && !string.IsNullOrEmpty(err))
                        {
                            errors.Add(err);
                        }

                        try
                        {
                            var dtNow = DateTime.Now;
                            var file = Path.Combine(dirLog, dtNow.ToString("yyyy-MM-dd") + ".txt");

                            using (var sw = new StreamWriter(file, true))
                            {
                                foreach (var e in errors)
                                {
                                    sw.WriteLine(e);
                                }
                                sw.Flush();
                            }

                            var oldfile = Path.Combine(dirLog, dtNow.AddDays(-2).ToString("yyyy-MM-dd") + ".txt");

                            File.Delete(oldfile);

                        }
                        catch { }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        Thread.Sleep(1000);
                    }
                }
            }).Start();
        }
    }
}
