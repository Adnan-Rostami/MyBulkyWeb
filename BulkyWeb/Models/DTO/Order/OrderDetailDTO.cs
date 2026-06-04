namespace BulkyWeb.Models.DTO.Order
{
    public class OrderDetailDTO
    {

        public int ProductID { get; set; }

        public short Quantity { get; set; }
        //public decimal UnitPrice { get; set; }

        //[Range(0, 1)]
        //public decimal Discount { get; set; }
        //public decimal TotalBeforeDiscount { get; set; }
        //public decimal TotalAfterDiscount { get; set; }
    }
}
