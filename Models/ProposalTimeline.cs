using System;
using System.Collections.Generic;

namespace SeniorProject.Models
{
    public partial class ProposalTimeline
    {
        public uint Id { get; set; }
        public uint SemesterId { get; set; }
        public DateOnly Deadline { get; set; }
        public string ToDo { get; set; } = null!;

        public virtual Semester Semester { get; set; } = null!;
    }
}
