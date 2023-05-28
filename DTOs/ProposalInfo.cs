using System;
using System.Collections.Generic;

namespace SeniorProject.DTOs
{
    public class ProposalInfo {
        public uint Id { get; set; }
        public uint ? No { get; set; }
        public string ? Role { get; set; }
        public string ? Major { get; set; }
        public string ? ProjectNameTh { get; set; }
        public string ? ProjectNameEn { get; set; }
        public string ? Semester { get; set; }
        public string ? Advisor1 { get; set; }
        public string ? Advisor2 { get; set; }
        public string ? Committee1 { get; set; }
        public string ? Committee2 { get; set; }
        public string ? Student1 { get; set; }
        public string ? Student2 { get; set; }
        public string ? Student3 { get; set; }
        public string ? Advisor1UploadFile { get; set; }
        public string ? Advisor2UploadFile { get; set; }
        public string ? Committee1UploadFile { get; set; }
        public string ? Committee2UploadFile { get; set; }
        // public string ? GradeStudent1 { get; set; }
        // public string ? GradeStudent2 { get; set; }
        // public string ? GradeStudent3 { get; set; }
    }
}