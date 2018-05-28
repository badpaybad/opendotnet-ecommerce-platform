using DomainDrivenDesign.Core.Commands;

namespace DomainDrivenDesign.CorePermission.Comands
{
    public class RightCommandHandles:ICommandHandle<UpdateRight>
    {
        public void Handle(UpdateRight c)
        {
            new DomainRight().UpdateDescription(c.Id,c.Description);
        }
    }
}