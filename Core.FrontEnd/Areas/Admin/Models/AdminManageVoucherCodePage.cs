using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.FrontEnd.Models;

namespace Core.FrontEnd.Areas.Admin.Models
{
    public class AdminManageVoucherCodePage
    {

        public Guid? SelectedMethodId;

        public List<FeShoppingCartCheckoutPage.FeIdAndDescription> VoucherMethods;

        public class VoucherCodeDisplay
        {
          
            public Guid Id { get; set; }
        
            public string Code { get; set; }
            public Guid VoucherMethodId { get; set; }
            public string VoucherMethodName { get; set; }
            public string VoucherMethodDescription { get; set; }
            public long Value { get; set; }
            public Guid AppliedForUserId { get; set; }
            public string AppliedForOrderCode { get; set; }
            public bool Applied { get; set; }
            public DateTime CreatedDate { get; set; }

        }
    }

    public class FeAuditLog
    {
        public Guid Id;
        public string CommandType;

        public string CommandData;

        public Guid UserId;
        public string Username;
        public string Email;
        public string Phone;
        public DateTime CreatedDate;
    }
}