namespace BulkyWeb.Models.DTO.Order
{
    public class OrderCreateDTO
    {
        //[Required(ErrorMessage = "آیدی مشتری اجباری است")]
        //[MinLength(5, ErrorMessage = "آیدی مشتری باید ۵ کاراکتر باشد")]
        // public string CustomerID { get; set; } = null!;
        // public string userId { get; set; } = null!;
        // public int ProductID { get; set; }

        //public DateTime? OrderDate { get; set; }
        //public DateTime? RequiredDate { get; set; }
        public int? ShipVia { get; set; }
        public decimal? Freight { get; set; }

        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }


        public DateTime? ShippedDate { get; set; }
        //   [Required]
        //  [MinLength(1, ErrorMessage = "سفارش باید حداقل یک آیتم داشته باشد")]
        public List<OrderDetailDTO> Items { get; set; } = new();
    }
}
