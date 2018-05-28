using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainDrivenDesign.Core.Implements.Models
{
    [Table("Language")]
    public class Language
    {
        [Key]
        [Index]
        public Guid Id { get; set; }
        [StringLength(128)]
        public string Code { get; set; }
        [StringLength(128)]
        public string Title { get; set; }
        [StringLength(128)]
        public string CurrencyCode { get; set; }
        public double CurrencyExchangeRate { get; set; }
    }
}