using System;
using System.Collections.Generic;

namespace SeniorProject.DTOs
{
    public class ProposalUpload
    {

        public uint? Id { get; set; }
        public uint? ProjectId { get; set; }
        public uint? AssignmentId { get; set; }
        public string? AssignmentName { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public DateTime? SubmitDate { get; set; }
        public string? StudentId { get; set; }
        public string? Sender { get; set; }
    }
}