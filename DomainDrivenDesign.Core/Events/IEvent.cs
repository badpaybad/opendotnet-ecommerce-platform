namespace DomainDrivenDesign.Core.Events
{
    public interface IEvent
    {
        long Version { get; set; }      
    }
}