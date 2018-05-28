using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainDrivenDesign.TestDomain.Core.Utils
{
    [TestClass]
    public class StringExtensionsTest
    {
        [TestMethod]
        public void EmailValid()
        {
            Console.WriteLine("badpaybad@gmail.com".IsValidEmail());
            Console.WriteLine("bad_paybad@gmail.com".IsValidEmail());
            Console.WriteLine("bad.paybad@gmail.com".IsValidEmail());
            Console.WriteLine("bad.pay-bad@gmail.com".IsValidEmail());
            Console.WriteLine("bad.pay_bad@gmail.com".IsValidEmail());
            Console.WriteLine("bad.pay_bad2112@gmail.com".IsValidEmail());
            Console.WriteLine(".bad.pay_bad_2112@gmail.com".IsValidEmail());
            Console.WriteLine(".bad.pay_bad_2112@".IsValidEmail());
            Console.WriteLine("".IsValidEmail());
        }
    }
}
