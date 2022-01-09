using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistryApp.Models
{
    [Table("Product")]
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(0,Int32.MaxValue)]
        public int PriceHuf { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        public virtual List<CategoryProduct> CategoryProducts { get; set; }
    }
}