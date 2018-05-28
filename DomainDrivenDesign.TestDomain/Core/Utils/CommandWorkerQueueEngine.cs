using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DomainDrivenDesign.Core.Commands;

namespace DomainDrivenDesign.TestDomain.Core.Utils
{
    public static class CommandWorkerQueueEngine
    {
        static readonly ConcurrentDictionary<Type, ConcurrentQueue<ICommand>> _cmdDataQueue = new ConcurrentDictionary<Type, ConcurrentQueue<ICommand>>();
        static readonly ConcurrentDictionary<Type, List<Thread>> _cmdWorker = new ConcurrentDictionary<Type, List<Thread>>();
        static readonly ConcurrentDictionary<Type, bool> _stopWorker = new ConcurrentDictionary<Type, bool>();
        static readonly ConcurrentDictionary<Type, int> _workerCounterStoped = new ConcurrentDictionary<Type, int>();
        static readonly ConcurrentDictionary<Type, bool> _workerStoped = new ConcurrentDictionary<Type, bool>();
        static readonly ConcurrentDictionary<string, Type> _cmdTypeName = new ConcurrentDictionary<string, Type>();
        static readonly object _locker = new object();

        static readonly ConcurrentDictionary<Type, Action<ICommand>> _cmdHandles = new ConcurrentDictionary<Type, Action<ICommand>>();

        public static void Push(ICommand cmd)
        {
            var type = cmd.GetType();
            if (_cmdHandles.ContainsKey(type) == false)
            {
                throw new NotImplementedException($"No handle register for type: {type.FullName}");
            }

            if (_cmdDataQueue.ContainsKey(type) && _cmdDataQueue[type] != null)
            {
                _cmdDataQueue[type].Enqueue(cmd);
            }
            else
            {
                _cmdTypeName[type.FullName] = type;

                _cmdDataQueue[type] = new ConcurrentQueue<ICommand>();
                _cmdDataQueue[type].Enqueue(cmd);

                InitFirstWorker(type);
            }

        }

        private static void InitFirstWorker(Type type)
        {
            while (_stopWorker.ContainsKey(type) && _stopWorker[type])
            {
                Thread.Sleep(100);
                //wait stopping
            }

            lock (_locker)
            {

                if (!_cmdWorker.ContainsKey(type) || _cmdWorker[type] == null || _cmdWorker[type].Count == 0)
                {
                    _stopWorker[type] = false;
                    _workerCounterStoped[type] = 0;
                    _workerStoped[type] = false;

                    _cmdWorker[type] = new List<Thread>();
                }

                var firstThread = new Thread(() => { WorkerDo(type); });

                _cmdWorker[type].Add(firstThread);

                firstThread.Start();
            }
        }

        static CommandWorkerQueueEngine()
        {

        }

        static void WorkerDo(Type type)
        {
            var handle = _cmdHandles[type];

            while (true)
            {
                try
                {
                    while (_stopWorker.ContainsKey(type) == false || _stopWorker[type] == false)
                    {
                        try
                        {
                            if (_cmdDataQueue.TryGetValue(type, out ConcurrentQueue<ICommand> cmdQueue) &&
                                cmdQueue != null)
                            {
                                if (cmdQueue.TryDequeue(out ICommand cmd) && cmd != null)
                                {
                                    handle(cmd);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        finally
                        {
                            Thread.Sleep(1);
                        }
                    }

                    if (!_workerCounterStoped.ContainsKey(type))
                    {
                        _workerCounterStoped[type] = 0;
                    }
                    if (_workerStoped[type] == false)
                    {
                        var counter = _workerCounterStoped[type];
                        counter++;
                        _workerCounterStoped[type] = counter;

                        lock (_locker)
                        {
                            if (_cmdWorker.TryGetValue(type, out List<Thread> listThread))
                            {
                                if (listThread.Count == counter)
                                {
                                    _workerStoped[type] = true;
                                    _workerCounterStoped[type] = 0;
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
                finally
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// reset thread to one. each command have one thread to process
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool ResetToOneWorker(Type type)
        {
            _stopWorker[type] = true;

            while (!_workerStoped.ContainsKey(type) || _workerStoped[type] == false)
            {
                Thread.Sleep(100);
                //wait all worker done its job
            }

            List<Thread> threads;

            if (_cmdWorker.TryGetValue(type, out threads))
            {
                foreach (var t in threads)
                {
                    try
                    {
                        t.Abort();
                    } catch { }
                }
            }
            
            _workerCounterStoped[type] = 0;
            _workerStoped[type] = false;
            _cmdWorker[type].Clear();
            _stopWorker[type] = false;

            InitFirstWorker(type);

            return true;
        }

        public static bool AddAndStartWorker(Type type)
        {
            if (!_cmdWorker.ContainsKey(type) || _cmdWorker[type] == null || _cmdWorker[type].Count == 0)
            {
                InitFirstWorker(type);
            }
            else
            {
                lock (_locker)
                {
                    _workerStoped[type] = false;
                    var thread = new Thread(() => WorkerDo(type));
                    _cmdWorker[type].Add(thread);
                    thread.Start();
                }
            }

            return true;
        }

        public static void CountStatistic(Type type, out int queueDataCount, out int workerCount)
        {
            queueDataCount = 0;
            workerCount = 0;
            if (_cmdWorker.TryGetValue(type, out List<Thread> list) && list != null)
            {
                workerCount = list.Count;
            }

            if (_cmdDataQueue.TryGetValue(type, out ConcurrentQueue<ICommand> queue) && queue != null)
            {
                queueDataCount = queue.Count;
            }
        }

        public static bool IsWorkerStopping(Type type)
        {
            bool val;
            if (_stopWorker.TryGetValue(type, out val))
            {
                return val;
            }

            return false;
        }

        public static void Init()
        {
            //use reflection to auto register handle
        }

        public static void RegisterHandle<T>( Action<T> handle) where T:ICommand
        {
            var type = typeof(T);

            if (_cmdHandles.ContainsKey(type))
            {
                throw new Exception($"Already existed to handle for type: {type.FullName}.");
            }

            _cmdHandles[type] = command => handle ((T)command);
        }

        public static List<string> ListAllCommandName()
        {
            lock (_locker)
            {
                return _cmdTypeName.Select(i => i.Key).ToList();
            }
        }

        public static Type GetType(string fullName)
        {
            lock (_locker)
            {
                return _cmdTypeName[fullName];
            }
        }
    }
}