using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CorePermission;
using DomainDrivenDesign.CorePermission.Comands;
using Core.FrontEnd.Areas.Admin.Models;

namespace Core.FrontEnd.Areas.Admin.Controllers
{
    [AdminLoginRequired()]
    public class AdminRoleController : AdminBaseController
    {
        // GET: Admin/AdminRole
        public ActionResult Index()
        {
            var model = new RoleListAdminPage();
            using (var db = new CoreDbContext())
            {
                model.Roles = db.Roles.OrderBy(i => i.Title).ToList();
                model.Rights = db.Rights.OrderBy(i => i.KeyName).ThenBy(i=>i.Title).ToList();
            }
            return View(model);
        }

        public ActionResult RefreshRight()
        {
            EngineePermission.RefreshRights();

            return RedirectToAction("Index");
        }


        public JsonResult FindRightByRole(Guid roleId)
        {
            List<Right> data = new List<Right>();
            using (var db = new CoreDbContext())
            {
                data = db.Rights.Join(db.RelationShips.Where(rs => rs.FromId == roleId), rl => rl.Id, rs => rs.ToId,
                        (rl, rs) => new { Rl = rl, Rs = rs })
                    .Select(i => i.Rl).ToList();
            }
            return Json(new
            {
                Ok = true,
                Data = data,
                Message = "Success"
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AssignRightsToRole(Guid roleId, List<Guid> rightIds)
        {
            MemoryMessageBuss.PushCommand(new AddRightsToRole(roleId, rightIds, CurrentUserId, DateTime.Now));
            return Json(new
            {
                Ok = true,
                Data = new { RoleId = roleId },
                Message = "Success"
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateRole(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return Json(new
                {
                    Ok = false,
                    Data = new { },
                    Message = "title empty"
                }, JsonRequestBehavior.AllowGet);
            }
            var role = new CreateRole(Guid.NewGuid(), title, title, CurrentUserId, DateTime.Now);
            MemoryMessageBuss.PushCommand(role);
            return Json(new
            {
                Ok = true,
                Data = new { RoleId = role.Id },
                Message = "Success"
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteRole(Guid id)
        {
            MemoryMessageBuss.PushCommand(new DeleteRole(id, CurrentUserId, DateTime.Now));
            return Json(new
            {
                Ok = true,
                Data = new { RoleId = id },
                Message = "Success"
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateRole(Guid id, string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return Json(new
                {
                    Ok = false,
                    Data = new { },
                    Message = "title empty"
                }, JsonRequestBehavior.AllowGet);
            }
            MemoryMessageBuss.PushCommand(new UpdateRole(id, title, title, CurrentUserId, DateTime.Now));
            return Json(new
            {
                Ok = true,
                Data = new { RoleId = id },
                Message = "Success"
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdateRight(Guid id, string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                return Json(new
                {
                    Ok = false,
                    Data = new { },
                    Message = "description empty"
                }, JsonRequestBehavior.AllowGet);
            }
            MemoryMessageBuss.PushCommand(new UpdateRight(id, description, CurrentUserId, DateTime.Now));
            return Json(new
            {
                Ok = true,
                Data = new { RoleId = id },
                Message = "Success"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}