namespace ES.Entity.Events
{
    public class UserCreatedEvent : IEvent
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailApprove { get; set; }
    }
}
