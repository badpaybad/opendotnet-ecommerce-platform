using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.CoreEcommerce.Commands;
using DomainDrivenDesign.CoreEcommerce.Ef;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminSupplierController : AdminBaseController
    {
        // GET: Admin/AdminSupplier
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult save(bool isCreate, Guid? id, string addressName, string address, double addressLat, double addressLng,
            string phone, string email, string note)
        {
            if (isCreate)
            {
                id= Guid.NewGuid();
                MemoryMessageBuss.PushCommand(new CreateSupplier(id.Value,addressName,email,phone,address, addressLat, addressLng
                    ,note, CurrentUserId, DateTime.Now));
            }
            else
            {
                MemoryMessageBuss.PushCommand(new UpdateSupplier(id.Value, addressName, email, phone, address, addressLat, addressLng
                    , note, CurrentUserId, DateTime.Now));
            }
            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddSuppliersToProduct(Guid productId, List<Guid> supplierIds)
        {
            supplierIds = (supplierIds ?? new List<Guid>()).Where(i => i != Guid.Empty).Distinct().ToList(); ;

           MemoryMessageBuss.PushCommand(new AddSuppliersToProduct(productId, supplierIds, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = productId }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Delete(Guid id)
        {
            MemoryMessageBuss.PushCommand(new DeleteSupplier(id, CurrentUserId,DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult List( string keywords,
            int? skip, int? take, string sortField, string orderBy)
        {
            var xtake = 10;
            var xskip = 0;
            long total = 0;
            if (skip != null)
            {
                xskip = skip.Value;
            }
            if (take != null)
            {
                xtake = take.Value;
            }
            if (string.IsNullOrEmpty(sortField))
            {
                sortField = nameof(Supplier.CreatedDate);
            }
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = "desc";
            }

            List<Supplier> rows = new List<Supplier>();

            using (var db = new CoreEcommerceDbContext())
            {
                var query = db.Suppliers.AsNoTracking().AsQueryable();
                if (!string.IsNullOrEmpty(keywords))
                {
                    query = query.Where(i => i.Address.Contains(keywords)|| i.AddressName.Contains(keywords)
                    || i.Email.Contains(keywords) || i.Phone.Contains(keywords) || i.Note.Contains(keywords));
                }
                rows = query.ToList();
            }
            
            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}