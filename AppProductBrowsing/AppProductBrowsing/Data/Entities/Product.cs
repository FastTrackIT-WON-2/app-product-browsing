using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppProductBrowsing.Data.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1000000)]
        public decimal Price { get; set; } = 0M;

        [Required]
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}
