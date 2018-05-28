using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.Core.Implements.Models
{
    [Table("FileInfo")]
    public class FileInfo
    {
        [Key]
        [Index]
        public Guid Id { get; set; }
        public string Domain { get; set; }
        public string RelativePath { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string Qquuid { get; set; }
        public string PhysicalPath { get; set; }
    }
}