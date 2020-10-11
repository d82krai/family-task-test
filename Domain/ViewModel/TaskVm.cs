using Domain.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.DataModels;

namespace Domain.ViewModel
{
    public class TaskVm
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public Guid AssignedToId { get; set; }
        public bool IsComplete { get; set; }
        public Member AssignedTo { get; set; }
    }
}
