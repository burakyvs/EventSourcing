using ES.Core.Exceptions;
using ES.Entity;
using ES.Entity.Events;

namespace ES.Business.EventAggregate
{
    public class UserAggregate : Aggregate
    {
        //Kullanıcı oluşturulduğunda
        public void Created(User model)
        {
            if (CheckStreamName())
                throw new StreamNotFoundException();

            UserCreatedEvent userCreated = new()
            {
                UserId = model.Id,
                Email = model.Email,
                EmailApprove = model.EmailApprove,
                Name = model.Name,
                UserName = model.UserName
            };
            events.Add(userCreated);
        }
        //Kullanıcı adı değiştirildiğinde
        public void NameChanged(string newName, Guid userId)
        {
            if (CheckStreamName())
                throw new StreamNotFoundException();

            UserNameChangedEvent userNameChanged = new()
            {
                NewName = newName,
                UserId = userId
            };
            events.Add(userNameChanged);
        }
        //Kullanıcı email onaylandığında
        public void EmailApproved(Guid userId)
        {
            if (CheckStreamName())
                throw new StreamNotFoundException();

            UserEmailApprovedEvent userEmailApproved = new()
            {
                UserId = userId
            };
            events.Add(userEmailApproved);
        }
    }
}
