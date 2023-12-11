using Pronia.Utilities.Enums;

namespace Pronia.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string Address { get; set; } 
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public List<BasketItem> BasketItems { get; set; }
        public DateTime PurchasedAt { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }



    }
}
