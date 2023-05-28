using System;
using System.Collections.Generic;

namespace SeniorProject.Models
{
    public partial class Faq
    {
        public uint Id { get; set; }
        public string Question { get; set; } = null!;
        public string Answer { get; set; } = null!;
    }
}
