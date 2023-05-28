using System;
using System.Collections.Generic;

namespace SeniorProject.Models
{
    public partial class Semester
    {
        public Semester()
        {
            Project = new HashSet<Project>();
            ProjectAssignment = new HashSet<ProjectAssignment>();
            ProjectTimeline = new HashSet<ProjectTimeline>();
            Proposal = new HashSet<Proposal>();
            ProposalAssignment = new HashSet<ProposalAssignment>();
            ProposalTimeline = new HashSet<ProposalTimeline>();
        }

        public uint Id { get; set; }
        public short AcademicYear { get; set; }
        public sbyte Term { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public virtual ICollection<Project> Project { get; set; }
        public virtual ICollection<ProjectAssignment> ProjectAssignment { get; set; }
        public virtual ICollection<ProjectTimeline> ProjectTimeline { get; set; }
        public virtual ICollection<Proposal> Proposal { get; set; }
        public virtual ICollection<ProposalAssignment> ProposalAssignment { get; set; }
        public virtual ICollection<ProposalTimeline> ProposalTimeline { get; set; }
    }
}
