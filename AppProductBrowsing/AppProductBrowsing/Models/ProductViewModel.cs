using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppProductBrowsing.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [MaxLength(200)]
        [Required]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string PriceFormatted => $"{Price:N2}";

        public List<SelectListItem> AvailableCategories { get; set; } = new();

        public int SelectedCategoryId { get; set; }

        public CategoryViewModel Category { get; set; } = new();

    }
}
