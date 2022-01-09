using System.ComponentModel.DataAnnotations.Schema;
using RegistryApp.Models.Authentication;

namespace RegistryApp.Models
{
    [Table("RegistryItem")]
    public class RegistryItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string RecipientName { get; set; }
        public string? BuyerName { get; set; }
        public virtual Product Product { get; set; }
        public virtual ApplicationUser Recipient { get; set; }
        public virtual ApplicationUser? Buyer { get; set; }
    }
}