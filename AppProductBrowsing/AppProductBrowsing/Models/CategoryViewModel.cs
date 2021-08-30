using System.ComponentModel.DataAnnotations;

namespace AppProductBrowsing.Models
{
    public class CategoryViewModel
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }
    }
}
