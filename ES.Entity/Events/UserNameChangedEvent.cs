namespace ES.Entity.Events
{
    public class UserNameChangedEvent : IEvent
    {
        public Guid UserId { get; set; }
        public string NewName { get; set; }
    }
}
