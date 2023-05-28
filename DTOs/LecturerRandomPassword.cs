using System;
using System.Collections.Generic;

namespace SeniorProject.DTOs
{
    public class LecturerRandomPassword
    {
        public uint? lecturerId { get; set; }
        public string? fullname { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
    }
}