using DomainDrivenDesign.Core.Commands;

namespace DomainDrivenDesign.CorePermission.Comands
{
    public class RoleCommandHandles : ICommandHandle<AddRightsToRole>
        , ICommandHandle<CreateRole>, ICommandHandle<DeleteRole>, ICommandHandle<UpdateRole>
    {
        public void Handle(AddRightsToRole c)
        {
            new DomainRole().UpdateRightsForRole(c.RoleId, c.RightIds);
        }

        public void Handle(CreateRole c)
        {
            new DomainRole().CreateRole(c.Id, c.KeyName, c.Title);
        }

        public void Handle(DeleteRole c)
        {
            new DomainRole().DeleteRole(c.Id);
        }

        public void Handle(UpdateRole c)
        {
            new DomainRole().UpdateRole(c.Id,c.KeyName,c.Title);
        }
    }
}