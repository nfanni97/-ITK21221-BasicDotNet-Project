using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace RegistryApp.Models
{
    [Table("CategoryProduct")]
    public class CategoryProduct : IEquatable<CategoryProduct>
    {
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public bool Equals(CategoryProduct? other)
        {
            if(other == null) {
                return false;
            }
            return CategoryId == other.CategoryId && ProductId == other.ProductId;
        }
    }
}