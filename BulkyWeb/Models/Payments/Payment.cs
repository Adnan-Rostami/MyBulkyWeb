using BulkyWeb.Models.Orders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models.Payments
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string Authority { get; set; }

        public long? RefId { get; set; }
        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public string Status { get; set; } // Created, Success, Failed
        //public enum PaymentStatus { Pending, Success, Failed }
        public string Gateway { get; set; } = "ZarinPal";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? VerifiedAt { get; set; }
    }

    //public enum PaymentStatus
    //{
    //    Pending,
    //    PaymentPending,
    //    success,
    //    Cancelled
    //}
}
