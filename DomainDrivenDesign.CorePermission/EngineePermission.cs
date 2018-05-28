using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DomainDrivenDesign.Core;
using DomainDrivenDesign.Core.Commands;
using DomainDrivenDesign.Core.Implements;
using DomainDrivenDesign.Core.Implements.Models;
using DomainDrivenDesign.CorePermission.Comands;
using DomainDrivenDesign.CorePermission.Reflections;
using System.Web.Http;
using System.Web.Routing;

namespace DomainDrivenDesign.CorePermission
{
   public  class EngineePermission
    {
        static EngineePermission()
        {
            RoleInit();
            RightInit();
        }
        public static void Init()
        {
          
        }

        public static void RefreshRights()
        {
            RightInit();
        }

        static void RoleInit()
        {
            var adminRoleId = EngineeCurrentContext.AdminRoleId;

            new DomainRole().CreateRole(adminRoleId, "Admin", "Admin");
            var adminId = Guid.NewGuid();

            User admin;
            using (var db = new CoreDbContext())
            {
                admin = db.Users
                        .SingleOrDefault(i => i.Username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    ;
            }

            if (admin == null)
            {
                MemoryMessageBuss.PushCommand(new CreateUser(adminId, "admin", "JulSau@2018", "", "","", DateTime.Now,
                    adminId));
                MemoryMessageBuss.PushCommand(new ActiveUser(adminId, adminId, DateTime.Now));
      
            }
            else
            {
                adminId = admin.Id;
                if (admin.Actived == false)
                {
                    MemoryMessageBuss.PushCommand(new ActiveUser(adminId, adminId, DateTime.Now));
                }
            }
            RelationShip roleRelation;
            using (var db = new CoreDbContext())
            {
                roleRelation = db.RelationShips.FirstOrDefault(i => i.FromId == adminId && i.ToId== adminRoleId);
            }
            if (roleRelation == null)
            {
                MemoryMessageBuss.PushCommand(new AddUserToRole(adminId, adminRoleId, adminId, DateTime.Now));
            }

        }

        static void RightInit()
        {
            var allAss = AppDomain.CurrentDomain.GetAssemblies();
            RegisterCommandAsRight(allAss);

            RegisterUrlRoutingAsRight(allAss);
            RegisterApiUrlRoutingAsRight(allAss);
        }

        static void RegisterUrlRoutingAsRight(Assembly[] allAss)
        {
            foreach (var assembly in allAss)
            {
                var controllers = assembly.GetTypes()
                    .Where(type => typeof(Controller).IsAssignableFrom(type)).ToList();
                if (controllers.Count == 0)
                {
                    continue;
                }
                var listMethods =
                    controllers.SelectMany(
                            type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                        .Where(
                            m =>
                                !m.IsDefined(typeof(System.Web.Http.NonActionAttribute))
                                //&&!m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),true).Any()
                                )
                        .Select(x => new
                        {
                            Controller = x.DeclaringType.Name.Replace("Controller", string.Empty),
                            Action = x.Name,
                            RightDescription = x.GetCustomAttributes().Where(i => i.GetType() == typeof(RightDescriptionAttribute))
                                .Select(rd => rd as RightDescriptionAttribute).Where(rd => rd != null).Select(rd => rd.Description).FirstOrDefault()
                            ,
                            Area = x.DeclaringType.Namespace.Split('.').Reverse().Skip(1).First(),
                            ReturnType = x.ReturnType.FullName
                        })
                        .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();

                using (var db = new CoreDbContext())
                {
                    foreach (var c in listMethods)
                    {
                        var rightName = $"/{c.Controller}/{c.Action}/";
                        var existed =
                            db.Rights.FirstOrDefault(
                                i => i.KeyName.Equals(rightName, StringComparison.OrdinalIgnoreCase));
                        if (existed == null)
                        {
                            var right = new Right()
                            {
                                Id = Guid.NewGuid(),
                                KeyName = rightName,
                                Title = rightName,
                                Type = 0,
                                ReturnType = c.ReturnType,
                                GroupName = c.Controller,
                                Description = c.RightDescription
                            };
                            db.Rights.Add(right);

                            var rs = db.RelationShips.FirstOrDefault(
                                i => i.FromId == EngineeCurrentContext.AdminRoleId && i.ToId == right.Id);
                            if (rs == null)
                            {
                                db.RelationShips.Add(new RelationShip()
                                {
                                    FromId = EngineeCurrentContext.AdminRoleId,
                                    ToId = right.Id,
                                    DisplayOrder = 0,
                                    FromTableName = "Role",
                                    ToTableName = "Right"
                                });
                            }
                        }
                        else
                        {
                            if(string.IsNullOrEmpty(existed.Description))
                            { existed.Description = c.RightDescription;}
                            existed.GroupName = c.Controller;
                        }

                    }
                    db.SaveChanges();
                }

            }
        }

        private static void RegisterCommandAsRight(Assembly[] allAss)
        {
            List<string> listCommand = new List<string>();
            foreach (var assembly in allAss)
            {
                var allTypes = assembly.GetTypes();
                var temp = allTypes
                    .Where(t => typeof(ICommand).IsAssignableFrom(t)
                                && t.IsClass && !t.IsAbstract)
                    .Select(i => i.FullName)
                    .ToList();
                listCommand.AddRange(temp);
            }

            if (listCommand != null && listCommand.Count > 0)
            {
                using (var db = new CoreDbContext())
                {
                    foreach (var cmd in listCommand)
                    {
                        var temp = db.Rights.SingleOrDefault(
                            i => i.KeyName.Equals(cmd, StringComparison.OrdinalIgnoreCase));
                        if (temp == null)
                        {
                            var right = new Right()
                            {
                                Id = Guid.NewGuid(),
                                KeyName = cmd,
                                Title = cmd,
                                Type = 1
                            };
                            db.Rights.Add(right);

                            var rs = db.RelationShips.SingleOrDefault(
                                i => i.FromId == EngineeCurrentContext.AdminRoleId && i.ToId == right.Id);
                            if (rs == null)
                            {
                                db.RelationShips.Add(new RelationShip()
                                {
                                    FromId = EngineeCurrentContext.AdminRoleId,
                                    ToId = right.Id,
                                    DisplayOrder = 0,
                                    FromTableName = "Role",
                                    ToTableName = "Right"
                                });
                            }

                        }
                    }
                    db.SaveChanges();
                }
            }
        }

        static void RegisterApiUrlRoutingAsRight(Assembly[] allAss)
        {
            foreach (var assembly in allAss)
            {
                var controllers = assembly.GetTypes()
                    .Where(type => typeof(ApiController).IsAssignableFrom(type)).ToList();

                if (controllers.Count == 0)
                {
                    continue;
                }
               
                var listMethods =
                    controllers.SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly| BindingFlags.Public))
                        .Where(
                            m =>
                                !m.IsDefined(typeof(System.Web.Http.NonActionAttribute))
                            //&&!m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute),true).Any()
                        )
                        .Select(x => new
                        {
                            Controller = x.DeclaringType.Name.Replace("Controller", string.Empty),
                            Action = x.Name,
                            ApiRoute = x.GetCustomAttributes<System.Web.Http.RouteAttribute>(true).Where(rd => rd != null).Select(rd => rd.Template).FirstOrDefault(),
                            RightDescription = x.GetCustomAttributes<RightDescriptionAttribute>(true).Where(rd=>rd!=null).Select(rd=>rd.Description).FirstOrDefault()
                            ,
                            Area = x.DeclaringType.Namespace.Split('.').Reverse().Skip(1).First(),
                            ReturnType = x.ReturnType.FullName
                        })
                        .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();
              
                using (var db = new CoreDbContext())
                {
                    foreach (var c in listMethods)
                    {
                        var rightName = $"/api/{c.Controller}/{c.Action}/";
                        if (!string.IsNullOrEmpty(c.ApiRoute))
                        {
                            rightName = c.ApiRoute;
                        }
                        var existed =
                            db.Rights.FirstOrDefault(
                                i => i.KeyName.Equals(rightName, StringComparison.OrdinalIgnoreCase));
                        if (existed == null)
                        {
                            var right = new Right()
                            {
                                Id = Guid.NewGuid(),
                                KeyName = rightName,
                                Title = rightName,
                                Type = 0,
                                ReturnType = c.ReturnType,
                                GroupName = c.Controller,
                                Description = c.RightDescription
                            };
                            db.Rights.Add(right);

                            var rs = db.RelationShips.FirstOrDefault(
                                i => i.FromId == EngineeCurrentContext.AdminRoleId && i.ToId == right.Id);
                            if (rs == null)
                            {
                                db.RelationShips.Add(new RelationShip()
                                {
                                    FromId = EngineeCurrentContext.AdminRoleId,
                                    ToId = right.Id,
                                    DisplayOrder = 0,
                                    FromTableName = "Role",
                                    ToTableName = "Right"
                                });
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(existed.Description))
                            { existed.Description = c.RightDescription; }
                            existed.GroupName = c.Controller;
                        }

                    }
                    db.SaveChanges();
                }

            }
        }

    }
}
