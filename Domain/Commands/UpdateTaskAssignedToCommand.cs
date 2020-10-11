using System;
using System.Collections.Generic;
using System.Text;
using Domain.DataModels;

namespace Domain.Commands
{
    public class UpdateTaskAssignedToCommand
    {
        public Guid TaskId { get; set; }
        public Guid MemberId { get; set; }
    }
}
