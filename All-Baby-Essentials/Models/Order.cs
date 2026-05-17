using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace All_Baby_Essentials.Models
{
    public enum OrderStatus
    {
        Processing,Completed, Cancelled
    }
    public enum OrderPaymentStatus
    {
        Pending,Approved,Failed,Refunded,Cancelled     
    }
    public enum OrderPaymentMethod
    {
        Visa,PayPal,CashOnDelivery
    }

    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(250)]
        public string ShippingAddress { get; set; } = string.Empty;

        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [StringLength(100)]
        public string Country { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public OrderPaymentStatus PaymentStatus { get; set; }

        public OrderPaymentMethod PaymentMethod { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
