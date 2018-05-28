using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CorePermission.Comands;
using DomainDrivenDesign.CorePermission.Events;

namespace DomainDrivenDesign.CorePermission
{
    public class UserSessionContext : IEventHandle<UserUpdated>, IEventHandle<UserLogedout>, IEventHandle<UserLogedin>
    {
        public static readonly string UrlAdminLogin = "/Admin/AdminHome/Login/";
        public static readonly string UrlFrontEndLogin = "/User/Login/";

        public static User CurrentUser(string tokenSession = "")
        {
            if (string.IsNullOrEmpty(tokenSession))
            {
                if (HttpContext.Current != null)
                {
                     tokenSession = HttpContext.Current.Session["__tokenSession"]==null
                        ?string.Empty: HttpContext.Current.Session["__tokenSession"].ToString();

                    if (string.IsNullOrEmpty(tokenSession))
                    {
                        var requestCookie = HttpContext.Current.Request.Cookies["__tokenSession"];
                        if (requestCookie != null)
                        {
                            tokenSession = requestCookie.Value;
                        }
                        else
                        {
                            var responseCookie = HttpContext.Current.Response.Cookies["__tokenSession"];
                            if (responseCookie != null)
                                tokenSession = responseCookie.Value;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(tokenSession)) return null;


            User u = CacheManager.Get<User>(tokenSession);

            if (u != null) return u;

            using (var db = new CoreDbContext())
            {
                u = db.Users.SingleOrDefault(
                    i => i.TokenSession.Equals(tokenSession, StringComparison.OrdinalIgnoreCase));

            }
            if (u == null) return null;

            if (u.TokenSessionExpiredDate < DateTime.Now) return null;

            CacheManager.Set(tokenSession, u);

            return u;
        }

        public static List<Role> CurrentUserRoles(string tokenSession = "")
        {
            var u = CurrentUser(tokenSession);
            if (u == null) return null;

            var roleKey = u.TokenSession + "_role";

            var r = CacheManager.Get<List<Role>>(roleKey);
            if (r != null) return r;

            List<Role> roles = null;
            using (var db = new CoreDbContext())
            {
                roles = db.Roles.Join(db.RelationShips, rl => rl.Id, rs => rs.ToId, (rl, rs) => new { Rl = rl, Rs = rs })
                    .Where(m => m.Rs.FromId == u.Id).Select(i => i.Rl).ToList();
            }

            CacheManager.Set(roleKey, roles);

            return roles;
        }

        public static List<Right> CurrentUserRights(string tokenSession = "")
        {
            var u = CurrentUser(tokenSession);
            if (u == null) return null;
            var rightKey = u.TokenSession + "_right";

            var r = CacheManager.Get<List<Right>>(rightKey);
            if (r != null) return r;

            var roles = CurrentUserRoles(tokenSession);

            if (roles == null) return null;

            var roleIds = roles.Select(i => i.Id).ToList();
            List<Right> rights = null;
            using (var db = new CoreDbContext())
            {
                rights = db.Rights.Join(db.RelationShips, rt => rt.Id, rs => rs.ToId,
                        (rt, rs) => new { Rt = rt, Rs = rs })
                    .Where(i => roleIds.Contains(i.Rs.FromId)).Select(i => i.Rt).ToList();
            }

            CacheManager.Set(rightKey, rights);

            return rights;
        }

        public static Dictionary<string, Right> ListAllRights()
        {
            var rightKey = "system_right_list_all";

            var r = CacheManager.Get<Dictionary<string, Right>>(rightKey);
            if (r != null) return r;

            List<Right> temp = null;
            using (var db = new CoreDbContext())
            {
                temp = db.Rights.ToList();
            }

            Dictionary<string, Right> rights = new Dictionary<string, Right>();
            foreach (var tr in temp)
            {
                rights[tr.KeyName.ToLower()] = tr;
            }
            CacheManager.Set(rightKey, rights);

            return rights;
        }

        public static bool CurrentUserIsSysAdmin(string tokenSession = "")
        {
            var roles = CurrentUserRoles(tokenSession);
            if (roles == null) return false;

            var isSupperAdmin = roles.Any(i => i.Id == EngineeCurrentContext.AdminRoleId);
            return isSupperAdmin;
        }

        public static Guid CurrentUserId(string tokenSession = "")
        {
            var u = CurrentUser(tokenSession);

            if (u != null) return u.Id;

            return Guid.Empty;
        }

        public static string CurrentUsername(string tokenSession = "")
        {
            var u = CurrentUser(tokenSession);

            if (u != null) return u.Username;

            return string.Empty;
        }

        public static User Dologin(string username, string password)
        {
            MemoryMessageBuss.PushCommand(new LoginUser(username, password));

            User u = null;
            using (var db = new CoreDbContext())
            {
                u = db.Users.SingleOrDefault(i => i.Actived && i.Deleted == false &&
               i.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

                if (u == null) return null;
            }
            SetHttpSession(u);
            return u;
        }

        public static User DoLoginFromGoogle(string googleId, string name, string email, string avatarUrl, string idToken, string websiteUrl)
        {
            email = email.Trim();
            MemoryMessageBuss.PushCommand(new LoginFromGoogle(email, name, googleId, idToken, avatarUrl, websiteUrl));

            User u = null;
            using (var db = new CoreDbContext())
            {
                u = db.Users.SingleOrDefault(i => i.Actived && i.Deleted == false &&
                                                  i.Username.Equals(email, StringComparison.OrdinalIgnoreCase));

                if (u == null) return null;
            }
            SetHttpSession(u);
            return u;
        }


        public static User DoLoginFromFacebook(string userId, string accessToken, string name, string email, string avatarUrl, string siteDomainUrl)
        {
            email = email.Trim();
            MemoryMessageBuss.PushCommand(new LoginFromFacebook(userId,accessToken,name,email, avatarUrl, siteDomainUrl));

            User u = null;
            using (var db = new CoreDbContext())
            {
                u = db.Users.SingleOrDefault(i => i.Actived && i.Deleted == false &&
                                                  i.Username.Equals(email, StringComparison.OrdinalIgnoreCase));

                if (u == null) return null;
            }
            SetHttpSession(u);
            return u;
        }

        private static void SetHttpSession(User u)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session["__tokenSession"] = u.TokenSession;
                HttpContext.Current.Response.Cookies.Set(new HttpCookie("__tokenSession", u.TokenSession));
                FormsAuthentication.SetAuthCookie(u.Username, false);
            }
        }

        public static void Dologout(string tokenSession = "")
        {
            var u = CurrentUser(tokenSession);
            if (u != null)
            {
                tokenSession = u.TokenSession;
            }
            else
            {
                var requestCookie = HttpContext.Current.Request.Cookies["__tokenSession"];
                if (requestCookie != null)
                    tokenSession = requestCookie.Value;
            }
            MemoryMessageBuss.PushCommand(new LogoutUser(tokenSession));
            CleanHttpSession(tokenSession);
        }


        public void Handle(UserUpdated e)
        {
            //reload info in session
            var currentUser = CurrentUser();
            if (currentUser == null) return;

            var tokenSession = currentUser.TokenSession;

            User u;
            using (var db = new CoreDbContext())
            {
                u = db.Users.SingleOrDefault(
                    i => i.TokenSession.Equals(tokenSession, StringComparison.OrdinalIgnoreCase));
            }

            CacheManager.Set(tokenSession, u);
        }

        public void Handle(UserLogedout e)
        {
            var tokenSession = e.TokenSession;
            CleanHttpSession(tokenSession);
        }

        public static void CleanHttpSession(string tokenSession)
        {
            HttpContext.Current.Session["__tokenSession"] = null;
            HttpContext.Current.Response.Cookies.Set(new HttpCookie("__tokenSession", null));
            CacheManager.Set<User>(tokenSession, null);

            var roleKey = tokenSession + "_role";
            var rightKey = tokenSession + "_right";
            CacheManager.Set<List<Role>>(roleKey, null);
            CacheManager.Set<List<Right>>(rightKey, null);
        }

        public void Handle(UserLogedin e)
        {
            User u = null;
            using (var db = new CoreDbContext())
            {
                u = db.Users.SingleOrDefault(i => i.Actived && i.Deleted == false &&
                                                  i.TokenSession.Equals(e.TokenSession, StringComparison.OrdinalIgnoreCase));

                if (u == null) return;
            }
            SetHttpSession(u);
        }

    }

}
