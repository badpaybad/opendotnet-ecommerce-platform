using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainDrivenDesign.Core.Utils
{
   public static class ExceptionExtensions
    {
        public static string GetMessages(this Exception ex)
        {
            var msg = ex.Message;
            while (ex.InnerException!=null)
            {
                msg +=" "+ ex.InnerException.Message;
                ex = ex.InnerException;
            }

            return msg;
        }
    }
}
