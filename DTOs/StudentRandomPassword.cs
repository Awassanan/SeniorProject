using System;
using System.Collections.Generic;

namespace SeniorProject.DTOs
{
    public class StudentRandomPassword
    {
        public string? studentId { get; set; }
        public string? fullname { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
    }
}