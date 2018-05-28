using System;
using System.Data.Entity;
using System.Threading;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.CoreCms.Commands;
using DomainDrivenDesign.TestDomain.Core.Utils;

namespace DomainDrivenDesign.TestDomain
{
    public class Program : ICommandHandle<Program.CmdTest1>, ICommandHandle<Program.CmdTest2>
    {
        public class CmdTest1 : ICommand
        {
            public readonly string Name;

            public CmdTest1(string name)
            {
                Name = name;
            }
        }
        public class CmdTest2 : ICommand
        {
            public readonly string Name;

            public CmdTest2(string name)
            {
                Name = name;
            }
        }

        public static void Main(params string[] args)
        {
            MemoryMessageBuss.AutoRegister();

            CommandWorkerQueueEngine.Init();

            var phandle=new Program();

            CommandWorkerQueueEngine.RegisterHandle<CmdTest1>(phandle.Handle);
            CommandWorkerQueueEngine.RegisterHandle<CmdTest2>(phandle.Handle);
          
            MemoryMessageBuss.PushCommand(new CmdTest1("Cmd 1"), true);
            MemoryMessageBuss.PushCommand(new CmdTest2("Cmd 2"), true);
            MemoryMessageBuss.PushCommand(new CmdTest1("Cmd 1.1"), true);
            MemoryMessageBuss.PushCommand(new CmdTest2("Cmd 2.1"), true);
            Console.ReadLine();
        }

        public void Handle(CmdTest1 c)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + c.Name);
            Thread.Sleep(5000);
        }

        public void Handle(CmdTest2 c)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + c.Name);
        }
    }


}


