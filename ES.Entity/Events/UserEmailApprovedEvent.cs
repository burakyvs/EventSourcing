namespace ES.Entity.Events
{
    public class UserEmailApprovedEvent : IEvent
    {
        public Guid UserId { get; set; }
    }
}
