using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.EventSourcingRepository;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CorePermission.Models;
using Newtonsoft.Json;

namespace DomainDrivenDesign.CorePermission.Comands
{
    public class UserCommandHandles : ICommandHandle<CreateUser>, ICommandHandle<UpdateUser>
        , ICommandHandle<ActiveUser>, ICommandHandle<ChangePasswordByUser>,
        ICommandHandle<DeactiveUser>, ICommandHandle<DeleteUser>
        , ICommandHandle<LoginUser>, ICommandHandle<LogoutUser>
        , ICommandHandle<AddUserToRole>, ICommandHandle<RemoveUserFromRole>
        , ICommandHandle<AssignUserToRoles>, ICommandHandle<RegisterUser>
        , ICommandHandle<ChangePasswordByAdmin>
        , ICommandHandle<ActiveUserAccount>
        , ICommandHandle<RequestResetUserPassword>, ICommandHandle<ResetUserPassword>
        , ICommandHandle<LoginFromGoogle>, ICommandHandle<LoginFromFacebook>
    {
        ICqrsEventSourcingRepository<DomainUser> _repo = new CqrsEventSourcingRepository<DomainUser>(new EventPublisher());
        public void Handle(CreateUser c)
        {
            _repo.CreateNew(new DomainUser(c.Id, c.Username, c.Password, c.Phone, c.Email, c.WebsiteUrl));
        }

        public void Handle(UpdateUser c)
        {
            _repo.GetDoSave(c.Id, obj => { obj.Update(c.Phone, c.Email); });
        }

        public void Handle(ActiveUser c)
        {
            _repo.GetDoSave(c.Id, obj => { obj.Active(); });
        }

        public void Handle(ChangePasswordByUser c)
        {
            _repo.GetDoSave(c.Id, obj => { obj.UserChangePassword(c.OldPassword, c.NewPassword); });
        }

        public void Handle(ChangePasswordByAdmin c)
        {
            _repo.GetDoSave(c.Id, obj => { obj.ChangePassword(c.NewPassword); });
        }

        public void Handle(DeactiveUser c)
        {
            _repo.GetDoSave(c.Id, obj => { obj.Deactive(); });
        }

        public void Handle(DeleteUser c)
        {
            _repo.GetDoSave(c.Id, obj => { obj.Delete(); });
        }

        public void Handle(LoginUser c)
        {
            if (string.IsNullOrEmpty(c.Username) || string.IsNullOrEmpty(c.Password)) throw new Exception("Username or password empty");

            Guid id = Guid.Empty;

            User temp = null;
            using (var db = new CoreDbContext())
            {
                temp = db.Users.SingleOrDefault(i => i.Username.Equals(c.Username)
               && i.Actived && i.Deleted == false);
                if (temp == null || temp.Id == Guid.Empty)
                {
                    throw new Exception("Not found user, or not actived");
                }
            }
            id = temp.Id;
            _repo.GetDoSave(id, obj => { obj.Login(c.Username, c.Password); });
        }

        public void Handle(LogoutUser c)
        {
            if (string.IsNullOrEmpty(c.TokenSession)) throw new Exception("Token empty");

            Guid id = Guid.Empty;
            User temp = null;

            using (var db = new CoreDbContext())
            {
                temp = db.Users.SingleOrDefault(i => i.TokenSession.Equals(c.TokenSession)
               && i.Actived && i.Deleted == false && !string.IsNullOrEmpty(i.TokenSession));

                if (temp == null || temp.Id == Guid.Empty)
                {
                    throw new Exception("Not found user");
                }

            }
            id = temp.Id;
            _repo.GetDoSave(id, obj =>
            {
                obj.Logout();
            });
        }

        public void Handle(AddUserToRole c)
        {
            _repo.GetDoSave(c.Id, obj => { obj.AddToRole(c.RoleId); });
        }

        public void Handle(RemoveUserFromRole c)
        {
            _repo.GetDoSave(c.Id, obj => { obj.RemoveFromeRole(c.OldRoleId, c.RoleId); });
        }

        public void Handle(AssignUserToRoles c)
        {
            _repo.GetDoSave(c.Id, obj => { obj.AssignRoles(c.Roles); });
        }

        public void Handle(RegisterUser c)
        {
            var resgisterUser = new DomainUser();
            resgisterUser.Register(c.Id, c.Email, c.Password, c.Phone, c.Address, c.WebsiteUrl);
            _repo.CreateNew(resgisterUser);
        }

        public void Handle(ActiveUserAccount c)
        {
            _repo.GetDoSave(c.UserId, obj => { obj.ActiveAccount(c.ActiveCode); });
        }

        public void Handle(RequestResetUserPassword c)
        {
            User u;
            using (var db = new CoreDbContext())
            {
                u = db.Users.SingleOrDefault(i => i.Username.Equals(c.Username, StringComparison.OrdinalIgnoreCase));
            }
            if (u == null) throw new Exception("Username do not exist");

            _repo.GetDoSave(u.Id, o => o.RequestResetPassword());
        }

        public void Handle(ResetUserPassword c)
        {
            _repo.GetDoSave(c.Id, o => o.ResetPassword(c.ConfirmCode));
        }

        public void Handle(LoginFromGoogle c)
        {
            GoogleAuthResponse res = null;
            using (var req = new HttpClient())
            {
                var url = "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=" + c.IdToken;
                req.BaseAddress = new Uri(url);
                var resx = req.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)).Result;
                if (resx.StatusCode == HttpStatusCode.OK)
                {
                    res = JsonConvert.DeserializeObject<GoogleAuthResponse>(resx.Content.ReadAsStringAsync().Result);
                }
            }
            if(res==null || string.IsNullOrEmpty(res.email)) throw new Exception("Can not confirm with google");
            User u;
            using (var db = new CoreDbContext())
            {
                u = db.Users.FirstOrDefault(i => i.Username.Equals(c.Email));
            }
            if (u == null)
            {
                u = new User();
                u.Id = Guid.NewGuid();
                u.Username = c.Email;
                u.Email = c.Email;

                var domainUser = new DomainUser();
                domainUser.RegisterFromGoogle(u.Id, c.GoogleId, c.Name, c.Email, c.AvatarUrl, c.IdToken, c.WebsiteUrl);

                _repo.CreateNew(domainUser);
            }

            _repo.GetDoSave(u.Id, o => o.LoginFromGoogle(c.GoogleId, c.Name, c.Email, c.AvatarUrl, c.IdToken));
        }

        public void Handle(LoginFromFacebook c)
        {
            var url = $"https://graph.facebook.com/v2.12/{c.UserId}?access_token={c.AccessToken}&fields=id,name,email";
            FacebookAuthResponse res=null;
            using (var wr = new HttpClient())
            {
                wr.BaseAddress=new Uri(url);
                var resx = wr.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)).Result;

                res = JsonConvert.DeserializeObject<FacebookAuthResponse>(resx.Content.ReadAsStringAsync().Result);
            }

            if (res == null|| string.IsNullOrEmpty(res.email)) throw new Exception("Can not confirm with facebook");

            User u;
            using (var db = new CoreDbContext())
            {
                u = db.Users.FirstOrDefault(i => i.Username.Equals(c.Email));
            }

            if (u == null)
            {
                u = new User();
                u.Id = Guid.NewGuid();
                u.Username = c.Email;
                u.Email = c.Email;

                var domainUser = new DomainUser();
                domainUser.RegisterFromFacebook(u.Id, c.Email,c.Name,c.AvatarUrl,c.UserId, c.SiteDomainUrl, c.AccessToken);

                _repo.CreateNew(domainUser);
            }

            _repo.GetDoSave(u.Id, o => o.LoginFromFacebook(c.UserId, c.Name, c.Email, c.AvatarUrl, c.AccessToken));
        }
    }

    public class ActiveUserAccount : ICommand
    {
        public Guid UserId { get; }
        public string ActiveCode { get; }
        public DateTime CreatedDate { get; }

        public ActiveUserAccount(Guid userId, string activeCode, DateTime createdDate)
        {
            UserId = userId;
            ActiveCode = activeCode;
            CreatedDate = createdDate;
        }
    }

    public class RequestResetUserPassword : ICommand
    {
        public string Username { get; }
        public DateTime CreateDate { get; }

        public RequestResetUserPassword(string username, DateTime createDate)
        {
            Username = username;
            CreateDate = createDate;
        }
    }

    public class ResetUserPassword : ICommand
    {
        public Guid Id { get; private set; }
        public string ConfirmCode { get; private set; }
        public DateTime CreateDate { get; }

        public ResetUserPassword(Guid id, string confirmCode, DateTime createDate)
        {
            Id = id;
            ConfirmCode = confirmCode;
            CreateDate = createDate;
        }
    }

    public class LoginFromGoogle : ICommand
    {
        public LoginFromGoogle(string email, string name, string googleId, string idToken, string avatarUrl, string websiteUrl)
        {
            Email = email;
            Name = name;
            GoogleId = googleId;
            IdToken = idToken;
            AvatarUrl = avatarUrl;
            WebsiteUrl = websiteUrl;
        }

        public string Email { get; }
        public string Name { get; }
        public string GoogleId { get; }
        public string IdToken { get; }
        public string AvatarUrl { get; }
        public string WebsiteUrl { get; }
    }


    public class LoginFromFacebook : ICommand
    {
        public string UserId { get; }
        public string AccessToken { get; }
        public string Name { get; }
        public string Email { get; }
        public string AvatarUrl { get; }
        public string SiteDomainUrl { get; }

        public LoginFromFacebook(string userId, string accessToken, string name, string email, string avatarUrl, string siteDomainUrl)
        {
            UserId = userId;
            AccessToken = accessToken;
            Name = name;
            Email = email;
            AvatarUrl = avatarUrl;
            SiteDomainUrl = siteDomainUrl;
        }
    }
}