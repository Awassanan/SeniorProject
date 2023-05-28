using System;
using System.Collections.Generic;

namespace SeniorProject.DTOs
{
    public class Password
    {
        public string? oldPassword { get; set; }
        public string? newPassword { get; set; }
        public string? confirmNewPassword { get; set; }
        public string? oldHint { get; set; }
        public string? newHint { get; set; }
        public string? oldKeyword { get; set; }
        public string? newKeyword { get; set; }
    }
}