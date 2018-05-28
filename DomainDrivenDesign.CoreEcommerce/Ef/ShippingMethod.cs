using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.CoreEcommerce.Ef
{
    [Table("ShippingMethod")]
    public class ShippingMethod
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(256)]
        public string Name { get; set; }
        [StringLength(256)]
        public string AssemblyType { get; set; }
        [StringLength(2048)]
        public string AssemblyFileDll { get; set; }
        public long UnitCost { get; set; }
    }
}