namespace Borda.UnitOfWork.SampleApplication
{
    [EventKey("personcreatedevent")]
    public class PersonCreatedEvent : EventBase
    {
        public string FullName { get; set; }
    }
}