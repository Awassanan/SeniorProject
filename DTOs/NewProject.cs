using System;
using System.Collections.Generic;

namespace SeniorProject.DTOs
{
    public class NewProject
    {
        public string? Email { get; set; }
        // public uint Id { get; set; }
        public uint? No { get; set; }
        public string? Major { get; set; }
        public string? ProjectNameTh { get; set; }
        public string? ProjectNameEn { get; set; }
        public string? Semester { get; set; }
        public uint? AdvisorId1 { get; set; }
        public uint? AdvisorId2 { get; set; }
        public string? StudentId1 { get; set; }
        public string? StudentId2 { get; set; }
        public string? StudentId3 { get; set; }
    }
}