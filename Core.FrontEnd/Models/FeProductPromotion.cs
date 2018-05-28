using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.FrontEnd.Models
{
    public class FeProductPromotion
    {
        public Guid Id { get; set; }
        public long ProductQuantity { get; set; }
        public long DiscountValue { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
    }
}