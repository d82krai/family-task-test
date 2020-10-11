using System;
using System.Collections.Generic;
using System.Text;
using Domain.DataModels;

namespace Domain.Commands
{
    public class CreateTaskCommand
    {
        public string Subject { get; set; }
        public Guid AssignedToId { get; set; }
        public bool IsComplete { get; set; }
        public Member AssignedTo { get; set; }
    }
}
