using System;
using System.Collections.Generic;

namespace SeniorProject.Models
{
    public partial class ProjectDownload
    {
        public uint Id { get; set; }
        public string FileName { get; set; } = null!;
        public string DownloadLink { get; set; } = null!;
        public DateTime ModifiedDate { get; set; }
        /// <summary>
        /// หน่วยเป็น byte (ส่วนมาก kB)
        /// </summary>
        public short FileSize { get; set; }
    }
}
