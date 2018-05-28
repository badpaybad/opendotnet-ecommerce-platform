using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.FrontEnd.Areas.Admin.Models
{
    public class AdminPageList<T>
    {
        public List<T> Items;

       
    }
    public class FePaymentMethod
    {
        public Guid Id;
        public string Description;
        public string Name;
        public string AssemblyType;
    }

    public class FeVoucherMethod
    {
        public Guid Id;
        public string Description;
        public string Name;
        public string AssemblyType;
    }

    public class FeShippingMethod
    {
        public Guid Id;
        public string Description;
        public string Name;
        public string AssemblyType;
        public long UnitCost;
    }
}