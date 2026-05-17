using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace All_Baby_Essentials.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public int OrderId {  get; set; }
        public Order Order { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;


        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [NotMapped]
        public decimal SubTotalBeforeTax => TotalAmount - TaxAmount;
    }
}
