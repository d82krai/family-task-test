using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.DataModels
{
    public class Task
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public Guid AssignedToId { get; set; }
        public bool IsComplete { get; set; }

        [ForeignKey("AssignedToId")]
        public Member AssignedTo { get; set; }
    }
}
