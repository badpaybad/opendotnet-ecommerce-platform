using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainDrivenDesign.Core.Exceptions
{
  public  static class ExceptionExtensions
    {
        public static string ToMessage(this Exception ex)
        {
            if (ex == null) return string.Empty;
            var temp = ex.Message;
            while (ex.InnerException!=null)
            {
                ex = ex.InnerException;
                temp +="\r\n"+ ex.Message;
            }
            return temp;
        }
    }
}
