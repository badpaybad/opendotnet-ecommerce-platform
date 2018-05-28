using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Events;
using DomainDrivenDesign.Core.Utils;
using DomainDrivenDesign.CorePermission.Events;

namespace DomainDrivenDesign.CorePermission
{
    public class DomainUser : AggregateRoot
    {
        private string _hasedPassword;
        private DateTime _createdDate;
        private string _username;
        private bool _active;
        private bool _deleted;
        private string _tokenSession;
        private DateTime _tokenSessionExpiredDate;
        private string _activeCode;
        private string _websiteUrl;
        private string _resetPasswordConfirmCode;
        private string _phone;
        private string _email;
        private bool _registeredFromGoogle;
        private bool _registeredFromFacebook;
        private const string _keyCription = "@Jul02$07#2016";
        public DomainUser()
        {
        }

        public override string Id { get; set; }

        void Apply(UserCreated e)
        {
            Id = e.Id.ToString();
            _username = e.Username;
            _email = e.Email;
            _phone = e.Phone;
            _hasedPassword = e.Password;
            _createdDate = e.CreatedDate;
            _websiteUrl = e.WebsiteUrl;
        }

        void Apply(UserUpdated e)
        {
            _email = e.Email;
            _phone = e.Phone;
        }

        void Apply(UserRegistered e)
        {
            Id = e.Id.ToString();
            _username = e.Username;
            _email = e.Email;
            _phone = e.Phone;
            _hasedPassword = e.Password;
            _createdDate = e.RegisteredDate;
            _activeCode = e.ActiveCode;
            _websiteUrl = e.WebsiteUrl;
        }

        void Apply(UserRegisteredFromGoogle e)
        {
            _registeredFromGoogle = true;
        }
        void Apply(UserRegisteredFromFacebook e)
        {
            _registeredFromFacebook = true;
        }

        void Apply(UserChangedPassword e)
        {
            _hasedPassword = e.Password;
        }
        void Apply(UserLogedin e)
        {
            _tokenSession = e.TokenSession;
            _tokenSessionExpiredDate = e.TokenSessionExpiredDate;
        }

        void Apply(UserLogedout e)
        {
            _tokenSession = e.TokenSession;
            _tokenSessionExpiredDate = e.TokenSessionExpiredDate;
        }

        void Apply(UserActived e)
        {
            _active = true;
            _activeCode = string.Empty;
        }

        void Apply(UserDeactived e)
        {
            _active = false;
        }

        void Apply(UserDeleted e)
        {
            _deleted = true;
        }
        void Apply(UserCreatedResetPasswordConfirmCode e)
        {
            _resetPasswordConfirmCode = e.ConfirmCode;
        }
        void Apply(UserResetPasswordDone e)
        {
            _resetPasswordConfirmCode = string.Empty;
        }

        public DomainUser(Guid id, string username, string password, string phone, string email, string websiteUrl)
        {
            username = username.Trim();
            password = password.Trim();
            email = email.Trim();
            using (var db = new CoreDbContext())
            {
                if (db.Users.Any(i => i.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                {
                    return;
                }
            }
            password = (username + password).ToPassword();

            ApplyChange(new UserCreated(id, username, password, phone, email, DateTime.Now, websiteUrl));
        }

        public void ChangePassword(string password)
        {
            password = (_username + password).ToPassword();
            var id = Guid.Parse(Id);
            ApplyChange(new UserChangedPassword(id, password));
        }

        public void UserChangePassword(string oldPass, string newPass)
        {
            var password = (_username + oldPass).ToPassword();
            if (!_hasedPassword.Equals(password, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Old password not correct");
            }

            password = (_username + newPass).ToPassword();
            var id = Guid.Parse(Id);
            ApplyChange(new UserChangedPassword(id, password));
        }

        public void Update(string phone, string email)
        {
            var id = Guid.Parse(Id);

            ApplyChange(new UserUpdated(id, phone, email));
        }

        public void Login(string username, string password)
        {
            if (!_active) throw new Exception($"{username} Do not actived");
            if (_deleted) throw new Exception($"{username} Deleted");

            var id = Guid.Parse(Id);
            password = (_username + password).ToPassword();

            if (username.Equals(_username, StringComparison.OrdinalIgnoreCase)
                && _hasedPassword.Equals(password, StringComparison.OrdinalIgnoreCase))
            {
                _tokenSession = StringCipher.Encrypt(_username, _keyCription) + "-" + Guid.NewGuid().ToString();
            }
            var tokenSessionExpired = DateTime.Now.AddDays(1);
            ApplyChange(new UserLogedin(id, _tokenSession, tokenSessionExpired));
        }

        public void LoginFromGoogle(string googleId, string name, string email, string avatarUrl, string idToken)
        {
            if (string.IsNullOrEmpty(idToken) || string.IsNullOrEmpty(email)) throw new Exception("Not valid google token or email");

            if (!_username.Equals(email)) throw new Exception("Not valid username");
            var id = Guid.Parse(Id);
            _tokenSession = StringCipher.Encrypt(_username, _keyCription) + "-" + Guid.NewGuid().ToString();
            var tokenSessionExpired = DateTime.Now.AddDays(1);
            ApplyChange(new UserLogedin(id, _tokenSession, tokenSessionExpired));

            if(_active== false) Active();
        }

        public void LoginFromFacebook(string userId, string name, string email, string avatarUrl, string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(email)) throw new Exception("Not valid google token or email");

            if (!_username.Equals(email)) throw new Exception("Not valid username");
            var id = Guid.Parse(Id);
            _tokenSession = StringCipher.Encrypt(_username, _keyCription) + "-" + Guid.NewGuid().ToString();
            var tokenSessionExpired = DateTime.Now.AddDays(1);
            ApplyChange(new UserLogedin(id, _tokenSession, tokenSessionExpired));

            if (_active == false) Active();
        }

        public void Logout()
        {
            var id = Guid.Parse(Id);

            _tokenSession = string.Empty;
            _tokenSessionExpiredDate = EngineeCurrentContext.SystemMinDate;
            ApplyChange(new UserLogedout(id, _tokenSession, _tokenSessionExpiredDate));
        }

        public void Active()
        {
            if (_active == true) return;
            var id = Guid.Parse(Id);
            _active = true;
            ApplyChange(new UserActived(id, _websiteUrl, _email, _username));
        }

        public void Deactive()
        {
            if (_active == false) return;
            var id = Guid.Parse(Id);
            _active = false;
            ApplyChange(new UserDeactived(id));
        }

        public void AddToRole(Guid roleId)
        {
            var id = Guid.Parse(Id);

            ApplyChange(new RelationShipUpdated(id, roleId, id, roleId, "User", "Role", 0));
        }

        public void RemoveFromeRole(Guid oldRoleId, Guid roleId)
        {
            var id = Guid.Parse(Id);
            ApplyChange(new RelationShipUpdated(id, oldRoleId, id, roleId, "User", "Role", 0));
        }

        public void AssignRoles(List<Guid> roleIds)
        {
            var id = Guid.Parse(Id);

            ApplyChange(new RelationShipFromRemoved(id, "User"));
            if (roleIds.Count > 0)
            {
                ApplyChange(new RelationShipAddedOneFromWithManyTo(id, roleIds, "User", "Role"));
            }
        }

        public void Delete()
        {
            if (_deleted) return;
            var id = Guid.Parse(Id);

            _deleted = true;
            ApplyChange(new UserDeleted(id));
        }

        public void Register(Guid id, string email, string password, string phone, string address, string siteUrl)
        {

            Id = id.ToString();
            var username = email.Trim();
            using (var db = new CoreDbContext())
            {
                if (db.Users.Any(i => i.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                {
                    return;
                }
            }
            password = (username + password).ToPassword();

            _activeCode = StringExtensions.RandomString(6);

            ApplyChange(new UserRegistered(id, username, email, password, phone, _activeCode, siteUrl, DateTime.Now));
            ApplyChange(new ContentLanguageUpdated(id, EngineeCurrentContext.LanguageId, "Address", address, "User"));
        }

        public void RegisterFromGoogle(Guid id, string googleId, string name, string email, string avatarUrl, string idToken, string siteUrl)
        {
            var tempPass = StringExtensions.RandomPassword(6);
            var username = email.Trim();
            var password = (username + tempPass).ToPassword();

            ApplyChange(new UserRegistered(id, username, email, password, string.Empty, _activeCode, siteUrl, DateTime.Now));
            ApplyChange(new ContentLanguageUpdated(id, EngineeCurrentContext.LanguageId, "Address", string.Empty, "User"));

            ApplyChange(new UserActived(id, _websiteUrl, _email, _username));

            ApplyChange(new UserRegisteredFromGoogle(id,_email,_username,_websiteUrl, googleId,avatarUrl, idToken));
        }
        public void RegisterFromFacebook(Guid id, string email, string name, string avatarUrl, string userId, string siteDomainUrl, string accessToken)
        {
            var tempPass = StringExtensions.RandomPassword(6);
            var username = email.Trim();
            var password = (username + tempPass).ToPassword();

            ApplyChange(new UserRegistered(id, username, email, password, string.Empty, _activeCode, siteDomainUrl, DateTime.Now));
            ApplyChange(new ContentLanguageUpdated(id, EngineeCurrentContext.LanguageId, "Address", string.Empty, "User"));

            ApplyChange(new UserActived(id, _websiteUrl, _email, _username));

            ApplyChange(new UserRegisteredFromFacebook(id, _email, _username, _websiteUrl, userId, avatarUrl, accessToken));

        }

        public void ActiveAccount(string activeCode)
        {
            if (string.IsNullOrEmpty(_activeCode)) throw new Exception("Your code expired."); ;

            if (_active == true) return;
            var id = Guid.Parse(Id);
            if (!string.IsNullOrEmpty(activeCode) && _activeCode.Equals(activeCode))
            {
                _activeCode = string.Empty;
                ApplyChange(new UserActived(id, _websiteUrl, _email, _username));
            }
            else
            {
                throw new Exception("Your active code is not correct");
            }
        }

        public void RequestResetPassword()
        {
            var id = Guid.Parse(Id);
            _resetPasswordConfirmCode = StringExtensions.RandomString(32);
            ApplyChange(new UserCreatedResetPasswordConfirmCode(id, _resetPasswordConfirmCode, _email, _username, _websiteUrl));
        }


        public void ResetPassword(string confrimCode)
        {
            if (string.IsNullOrEmpty(_resetPasswordConfirmCode)) throw new Exception("Your code expired.");

            var id = Guid.Parse(Id);
            if (!string.IsNullOrEmpty(confrimCode) &&
                _resetPasswordConfirmCode.Equals(confrimCode, StringComparison.OrdinalIgnoreCase))
            {
                var newPassword = StringExtensions.RandomString(6);

                _resetPasswordConfirmCode = string.Empty;

                ChangePassword(newPassword);

                ApplyChange(new UserResetPasswordDone(id, _username, newPassword, _websiteUrl, _email));
            }
            else
            {
                throw new Exception("Confirm code for reset password not correct. Please check confirm code in your email");
            }
        }

       
    }

   
}
