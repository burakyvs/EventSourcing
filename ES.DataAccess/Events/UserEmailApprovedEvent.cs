using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES.DataAccess.Events
{
    public class UserEmailApprovedEvent : IEvent
    {
        public int UserId { get; set; }
    }
}
