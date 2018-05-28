using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.FrontEnd.Areas.Admin.Models
{
    public class MonitorOrderItem
    {
        public Guid ProductId;
        public string Title;
        public string ProductCode;
        public string UrlImage;

        public int Quantity { get; set; }
    }
}