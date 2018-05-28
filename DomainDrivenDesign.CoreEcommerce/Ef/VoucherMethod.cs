using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.CoreEcommerce.Ef
{
    [Table("VoucherMethod")]
    public class VoucherMethod
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(256)]
        public string Name { get; set; }
        [StringLength(256)]
        public string AssemblyType { get; set; }
        [StringLength(2048)]
        public string AssemblyFileDll { get; set; }
    }
}