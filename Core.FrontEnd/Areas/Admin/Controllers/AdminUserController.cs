using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CoreCms.Commands;
using DomainDrivenDesign.CorePermission.Comands;
using Core.FrontEnd.Areas.Admin.Models;
using Core.FrontEnd.Models;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    public class AdminUserController : AdminBaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(Guid id)
        {
            var model = new UserEditAdminPage();

            using (var db = new CoreDbContext())
            {
                var temp = db.Users.SingleOrDefault(i => i.Id == id);
                if (temp != null)
                {
                    model.Id = temp.Id;
                    model.Username = temp.Username;
                    model.Phone = temp.Phone;
                    model.Email = temp.Email;
                    model.Actived = temp.Actived;
                    model.Deleted = temp.Deleted;
                }

                model.AllRoles = db.Roles.ToList();

                model.Roles = db.Roles.Join(db.RelationShips, rl => rl.Id, rs => rs.ToId,
                        (rl, rs) => new {Rl = rl, Rs = rs})
                    .Where(i => i.Rs.FromId == id).Select(i => i.Rl).ToList();
            }

            return View(model);
        }

        public JsonResult List(Guid? roleId, string keywords,
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
                sortField = nameof(DomainDrivenDesign.Core.Implements.Models.User.Username);
            }
            if (string.IsNullOrEmpty(orderBy))
            {
                orderBy = "desc";
            }

            Expression<Func<User, bool>> predicate = u => true;
            if (!string.IsNullOrEmpty(keywords))
            {
                predicate = predicate.And(i => i.Username.Contains(keywords)
                || i.Email.Contains(keywords)
                || i.Phone.Contains(keywords));
            }


            List<User> rows = null;
            using (var db = new DomainDrivenDesign.CoreCms.Ef.CoreCmsDbContext())
            {
                if (roleId.HasValue)
                {
                    var xroleId = roleId.Value;
                    var queryable = db.Users.Join(db.RelationShips, u => u.Id, rs => rs.FromId, (u, rs) => new { U = u, Rs = rs })
                        .Where(i => i.Rs.ToId == xroleId).Select(m => m.U)
                        .Where(predicate);
                    total = queryable.LongCount();

                    rows = queryable
                        .OrderBy(i => i.Username)
                        .Skip(xskip).Take(xtake).ToList();
                }
                else
                {
                    var queryable = db.Users.Where(predicate);
                    total = queryable.LongCount();

                    rows = queryable
                        .OrderBy(i => i.Username)
                        .Skip(xskip).Take(xtake).ToList();
                }
            }

            return Json(new { total, rows, success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(Guid id)
        {
            MemoryMessageBuss.PushCommand(new DeleteUser(id, CurrentUserId,DateTime.Now));
            User u;
            using (var db = new CoreDbContext())
            {
                u = db.Users.SingleOrDefault(i => i.Id == id);
            }
            return Json(new { Ok = true, Data = new { Id = id, IsDelete = u.Deleted }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult AssignRoles(Guid id, List<Guid> roles)
        {
            if (roles == null || roles.Count == 0)
            {
                return Json(new { Ok = false, Data = new { Id = id }, Message = "No role(s) checked" }, JsonRequestBehavior.AllowGet);

            }

            MemoryMessageBuss.PushCommand(new AssignUserToRoles(id, roles, CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult RemoveAllRoles(Guid id)
        {
            MemoryMessageBuss.PushCommand(new AssignUserToRoles(id, new List<Guid>(), CurrentUserId, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ChangePassword(Guid id, string password)
        {
            password = password.Trim();
            if (string.IsNullOrEmpty(password))
            {
                return Json(new { Ok = false, Data = new { Id = id }, Message = "Password is require" }, JsonRequestBehavior.AllowGet);
            }

            MemoryMessageBuss.PushCommand(new ChangePasswordByAdmin(id, password, CurrentUserId, DateTime.Now));
            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult CreateOrUpdate(Guid id, string username, string password, string phone, string email)
        {
            password = password.Trim();
            
            if (id == Guid.Empty)
            {
                id= Guid.NewGuid();
                if (string.IsNullOrEmpty(password))
                {
                    return Json(new { Ok = false, Data = new { Id = id }, Message = "Password is require" }, JsonRequestBehavior.AllowGet);
                }

                MemoryMessageBuss.PushCommand(new CreateUser(id,username,password, phone, email, SiteDomainUrl, DateTime.Now, CurrentUserId));
            }
            else
            {
                MemoryMessageBuss.PushCommand(new UpdateUser(id,phone,email, CurrentUserId, DateTime.Now));
                if (!string.IsNullOrEmpty(password))
                {
                    MemoryMessageBuss.PushCommand(new ChangePasswordByAdmin(id, password, CurrentUserId, DateTime.Now));
                }
            }

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Active(Guid id, bool isActive)
        {
            if (isActive)
            {
                MemoryMessageBuss.PushCommand(new ActiveUser(id, CurrentUserId, DateTime.Now));
            }
            else
            {
                MemoryMessageBuss.PushCommand(new DeactiveUser(id, CurrentUserId, DateTime.Now));
            }
            User u;
            using (var db = new CoreDbContext())
            {
                u = db.Users.SingleOrDefault(i => i.Id == id);
            }
            return Json(new { Ok = true, Data = new { Id = id, IsActive = u.Actived }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

    }
}