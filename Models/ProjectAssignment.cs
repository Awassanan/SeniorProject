using System;
using System.Collections.Generic;

namespace SeniorProject.Models
{
    public partial class ProjectAssignment
    {
        public ProjectAssignment()
        {
            ProjectUpload = new HashSet<ProjectUpload>();
        }

        public uint Id { get; set; }
        public uint SemesterId { get; set; }
        public string? AssignmentName { get; set; }
        public string? FileType { get; set; }
        public string SaveName { get; set; } = null!;
        public DateTime Deadline { get; set; }
        /// <summary>
        /// หน่วยเป็น byte (ส่วนมาก MB)
        /// </summary>
        public byte MaxSize { get; set; }

        public virtual Semester Semester { get; set; } = null!;
        public virtual ICollection<ProjectUpload> ProjectUpload { get; set; }
    }
}
