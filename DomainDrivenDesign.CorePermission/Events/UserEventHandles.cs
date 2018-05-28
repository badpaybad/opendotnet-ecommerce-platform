using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Events;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;

namespace DomainDrivenDesign.CorePermission.Events
{
   public class UserEventHandles:IEventHandle<UserCreated>, IEventHandle<UserChangedPassword>
        ,IEventHandle<UserUpdated>,IEventHandle<UserLogedin>, IEventHandle<UserLogedout>
        ,IEventHandle<UserActived>,IEventHandle<UserDeactived>,IEventHandle<UserDeleted>,
        IEventHandle<UserRegistered>,IEventHandle<UserResetPasswordDone>
        ,IEventHandle<UserRegisteredFromFacebook>, IEventHandle<UserRegisteredFromGoogle>
   {
        public void Handle(UserCreated e)
        {
            using (var db = new CoreDbContext())
            {
                db.Users.Add(new User
                {
                    Id=e.Id,
                    CreatedDate=e.CreatedDate,
                    Deleted= false,
                    Phone=e.Phone,
                    Email=e.Email,
                    Username=e.Username,
                    TokenSession=string.Empty,
                    Password=e.Password,
                    Actived=false,
                    TokenSessionExpiredDate= EngineeCurrentContext.SystemMinDate
                });
                db.SaveChanges();
            }
        }

        public void Handle(UserChangedPassword e)
        {
            using (var db = new CoreDbContext())
            {
                var u = db.Users.SingleOrDefault(i => i.Id == e.Id);
                if (u != null)
                {
                    u.Password = e.Password;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(UserUpdated e)
        {
            using (var db = new CoreDbContext())
            {
                var u = db.Users.SingleOrDefault(i => i.Id == e.Id);
                if (u != null)
                {
                    u.Email = e.Email;
                    u.Phone = e.Phone;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(UserLogedin e)
        {
            using (var db = new CoreDbContext())
            {
                var u = db.Users.SingleOrDefault(i => i.Id == e.Id);
                if (u != null)
                {
                    u.TokenSession = e.TokenSession;
                    u.TokenSessionExpiredDate = e.TokenSessionExpiredDate;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(UserLogedout e)
        {
            using (var db = new CoreDbContext())
            {
                var u = db.Users.SingleOrDefault(i => i.Id == e.Id);
                if (u != null)
                {
                    u.TokenSession = e.TokenSession;
                    u.TokenSessionExpiredDate = e.TokenSessionExpiredDate;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(UserActived e)
        {
            using (var db = new CoreDbContext())
            {
                var u = db.Users.SingleOrDefault(i => i.Id == e.Id);
                if (u != null)
                {
                    u.Actived = true;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(UserDeactived e)
        {
            using (var db = new CoreDbContext())
            {
                var u = db.Users.SingleOrDefault(i => i.Id == e.Id);
                if (u != null)
                {
                    u.Actived = false;
                    db.SaveChanges();
                }
            }
        }

        public void Handle(UserDeleted e)
        {
            using (var db = new CoreDbContext())
            {
                var u = db.Users.SingleOrDefault(i => i.Id == e.Id);
                if (u != null)
                {
                    u.Deleted = true;
                    db.SaveChanges();
                }
            }
        }

       public void Handle(UserRegistered e)
       {
           using (var db = new CoreDbContext())
           {
               db.Users.Add(new User
               {
                   Id = e.Id,
                   CreatedDate = e.RegisteredDate,
                   Deleted = false,
                   Phone = e.Phone,
                   Email = e.Email,
                   Username = e.Username,
                   TokenSession = string.Empty,
                   Password = e.Password,
                   Actived = false,
                   TokenSessionExpiredDate = EngineeCurrentContext.SystemMinDate
               });
               db.SaveChanges();
           }
        }

       public void Handle(UserResetPasswordDone e)
       {
           
       }

       public void Handle(UserRegisteredFromFacebook e)
       {
       }

       public void Handle(UserRegisteredFromGoogle e)
       {
       }
   }
}
