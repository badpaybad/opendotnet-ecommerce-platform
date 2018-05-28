using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Core.FrontEnd.Models;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CoreCms.Commands;
using DomainDrivenDesign.CoreCms.Ef;
using DomainDrivenDesign.CoreEcommerce.Ef;
using DomainDrivenDesign.CorePermission;
using DomainDrivenDesign.CorePermission.Comands;
using DomainDrivenDesign.CorePermission.Events;
using Newtonsoft.Json;

namespace Core.FrontEnd.Controllers
{
    [AllowAnonymous]
    public class UserController : CmsBaseController
    {
        [LoginRequire(false)]
        public ActionResult UserProfile()
        {
            var model = new FeUserProfile();
            var currentUser = UserSessionContext.CurrentUser();
            model.Id = currentUser.Id;
            using (var db = new CoreEcommerceDbContext())
            {
                var user = db.Users.SingleOrDefault(i => i.Id == model.Id);
                model.Email = user.Email;
                model.Phone = user.Phone;
            }

            return View(model);
        }

        // GET: User
        public ActionResult Login(string url = "")
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection formCollection, string username, string password, string url = "")
        {
            UserSessionContext.Dologin(username, password);
            if (!string.IsNullOrEmpty(url))
            {
                return Redirect(HttpUtility.UrlDecode(url));
            }
            if (UserSessionContext.CurrentUserIsSysAdmin())
            {
                return Redirect("~/Admin");
            }
            if (UserSessionContext.CurrentUser() != null)
            {
                return Redirect("~/");
            }
            return View();
        }

        public JsonResult Facebooklogin(string userId,string accessToken,string uid,string name, string email, string avatarUrl)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(accessToken))
            {
                return Json(new { Ok = false, Data = new { email }, Message = "Fail" }, JsonRequestBehavior.AllowGet);
            }
            var u = UserSessionContext.DoLoginFromFacebook(userId,accessToken,name,email,avatarUrl, SiteDomainUrl);

            if (u == null)
            {
                return Json(new { Ok = false, Data = new { email }, Message = "Fail" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Ok = true, Data = new { email }, Message = "Success" }, JsonRequestBehavior.AllowGet);

            return Json(new { Ok = true, Data = new {  }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult Googlelogin(string googleId, string name, string email, string avatarUrl, string idToken)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(idToken))
            {
                return Json(new { Ok = false, Data = new { email }, Message = "Fail" }, JsonRequestBehavior.AllowGet);
            }

            var u = UserSessionContext.DoLoginFromGoogle(googleId, name, email, avatarUrl, idToken, SiteDomainUrl);

            if (u == null)
            {
                return Json(new { Ok = false, Data = new { email }, Message = "Fail" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Ok = true, Data = new { email }, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        [LoginRequire(false)]
        public ActionResult Logout()
        {
            var currentUserIsSysAdmin = UserSessionContext.CurrentUserIsSysAdmin();

            try
            {
                UserSessionContext.Dologout();
                return Redirect("~/");
            }
            catch { }

            if (currentUserIsSysAdmin) { return Redirect(UserSessionContext.UrlAdminLogin); }

            return Redirect("~/");
        }

        [AllowAnonymous]
        public ActionResult ActiveUserAccount(Guid id, string code)
        {
            var model = new FeUserAccountCheckCode();
            try
            {
                MemoryMessageBuss.PushCommand(new ActiveUserAccount(id, code, DateTime.Now));

                User u;

                using (var db = new CoreCmsDbContext())
                {
                    u = db.Users.SingleOrDefault(i => i.Id == id);
                }

                if (u != null) model.Success = u.Actived;
                else
                {
                    model.ErrorMessage = "Not found user";
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessage = ex.GetMessages();
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(Guid id, string code)
        {
            var model = new FeUserAccountCheckCode();
            try
            {

                User u;

                using (var db = new CoreCmsDbContext())
                {
                    u = db.Users.SingleOrDefault(i => i.Id == id);
                }

                if (u == null)
                {
                    model.ErrorMessage = "Not found user";
                }

                MemoryMessageBuss.PushCommand(new ResetUserPassword(id, code, DateTime.Now));

                model.Success = true;
            }
            catch (Exception ex)
            {
                model.ErrorMessage = ex.GetMessages();
            }

            return View(model);
        }

        public JsonResult RequestToResetPassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return Json(new { Ok = false, Data = new { Username = username }, Message = "Username is empty" }, JsonRequestBehavior.AllowGet);

            }
            MemoryMessageBuss.PushCommand(new RequestResetUserPassword(username, DateTime.Now));

            return Json(new { Ok = true, Data = new { Username = username }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }
        [AllowAnonymous]
        public JsonResult Register(string email, string password, string phone, string address)
        {
            var id = Guid.NewGuid();
            MemoryMessageBuss.PushCommand(new RegisterUser(id, email, password, phone, address, SiteDomainUrl));
            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }
        [AllowAnonymous]
        public JsonResult SendContactUsInfo(string fromName, string fromEmail, string body, string fromPhone)
        {
            if (string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(fromPhone) || string.IsNullOrEmpty(body))
            {
                return Json(new { Ok = false, Data = new { Id = Guid.Empty }, Message = "Email, phong, title are required" }, JsonRequestBehavior.AllowGet);

            }
            var id = Guid.NewGuid();
            MemoryMessageBuss.PushCommand(new SendContactUsInfo(id, fromName, fromEmail, body, body, fromPhone, LanguageId));
            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        [LoginRequire(true)]
        public JsonResult SaveProfile(string phone, string email)
        {
            var id = UserSessionContext.CurrentUserId();


            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(email))
            {
                return Json(new { Ok = false, Data = new { Id = id }, Message = "Phone, email are require" }, JsonRequestBehavior.AllowGet);
            }


            MemoryMessageBuss.PushCommand(new UpdateUser(id, phone, email, id, DateTime.Now));

            return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);

        }

        [LoginRequire(true)]
        public JsonResult ChangePassword(string oldPassword, string newPassword, string newPasswordRetype)
        {
            var id = UserSessionContext.CurrentUserId();

            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) ||
                !newPassword.Equals(newPasswordRetype))
            {
                return Json(new { Ok = false, Data = new { Id = id }, Message = "Change password failed. Your passowrd is empty or not match" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                MemoryMessageBuss.PushCommand(new ChangePasswordByUser(id, oldPassword, newPassword, id, DateTime.Now));

                return Json(new { Ok = true, Data = new { Id = id }, Message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Ok = false, Data = new { Id = id }, Message = ex.GetMessages() }, JsonRequestBehavior.AllowGet);

            }
        }

    }
}