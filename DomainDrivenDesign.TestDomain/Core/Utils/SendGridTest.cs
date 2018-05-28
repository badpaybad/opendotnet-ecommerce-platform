using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace DomainDrivenDesign.TestDomain.Core.Utils
{
    [TestClass]
    public class SendGridTest
    {
        [TestMethod]
        public void SendEmail()
        {
            var res = SendGridHelper.SendEmail(new List<SendGridRequest.Email>(){ new SendGridRequest.Email()
            {
                email="badpaybad@gmail.com"
            }}, "[test] subject hello", "<b>Hello Content</b>");

            Console.WriteLine(JsonConvert.SerializeObject(res));
        }
    }
}
