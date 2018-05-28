using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.FrontEnd.Models
{
    public class FeOrderPromotion
    {
        public Guid Id { get; set; }
        public long AmountToDiscount { get; set; }
        public long DiscountAmount { get; set; }
        public bool FreeShip { get; set; }
        public bool Actived { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
    }
}