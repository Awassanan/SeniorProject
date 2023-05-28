using System;
using System.Collections.Generic;

namespace SeniorProject.Models
{
    public partial class Announcement
    {
        public uint Id { get; set; }
        public string Event { get; set; } = null!;
        public DateOnly AnnouncedDate { get; set; }
    }
}
