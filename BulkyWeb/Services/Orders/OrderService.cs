using AutoMapper;
using BulkyWeb.Data;
using BulkyWeb.Exceptions;
using BulkyWeb.Models;
using BulkyWeb.Models.DTO.Order;
using BulkyWeb.Models.Orders;
using BulkyWeb.Repository.IRepository;
using BulkyWeb.Services.Payments;
using BulkyWeb.Validators.Orders;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
namespace BulkyWeb.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _HttpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly PaymentService _paymentService;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderService> logger, ApplicationDbContext context
            , IHttpContextAccessor httpContextAccessor, PaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _context = context;
            _HttpContextAccessor = httpContextAccessor;
            _paymentService = paymentService;
        }

        public async Task<List<OrderGetAllDTO>> GetAllAsync(OrderFilterDTO filterDTO)
        {
            var query = _unitOfWork.OrderRepository.Query();

            if (filterDTO.OrderID.HasValue)
            {
                query = query.Where(x => x.OrderID == filterDTO.OrderID);
            }

            if (!string.IsNullOrWhiteSpace(filterDTO.CustomerId))
            {
                query = query.Where(x => x.CustomerID.Contains(filterDTO.CustomerId));
            }

            switch (filterDTO.SortBy)
            {
                case "id_desc":
                    query = query.OrderByDescending(x => x.OrderID);
                    break;

                default:
                    query = query.OrderBy(x => x.OrderID);
                    break;
            }

            int page = filterDTO.PageNumber is > 0 ? filterDTO.PageNumber.Value : 1;
            int pageSize = filterDTO.PageSize is > 0 ? filterDTO.PageSize.Value : 3;

            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return await _mapper
                .ProjectTo<OrderGetAllDTO>(query)
                .ToListAsync();
        }



        public async Task<OrderCreateResponseDTO> OrderCreate(OrderCreateDTO filter, CancellationToken ct = default)
        {
            var userId = _HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var pc = new PersianCalendar();
            var date = DateTime.Now;
            var shamsi = $"{pc.GetYear(date)}/{pc.GetMonth(date)}/{pc.GetDayOfMonth(date)}";


            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var validator = new OrderCreateValidator();
            var result = validator.Validate(filter);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }


            var productIds = filter.Items.Select(x => x.ProductID).Distinct();

            var products = await _unitOfWork.ProductRepository
                .Query(true)
                .Where(p => productIds.Contains(p.ProductID))
                .ToListAsync(ct);

            ct.ThrowIfCancellationRequested();
            decimal totalBefore = 0;
            decimal totalAfter = 0;



            try
            {

                var order = _mapper.Map<Order>(filter);
                order.UserId = userId;
                order.OrderDate = DateTime.UtcNow;
                order.RequiredDate = DateTime.UtcNow.AddDays(7);

                order.OrderDetails = new List<OrderDetail>();


                foreach (var item in filter.Items)
                {
                    if (item.Quantity <= 0)
                        throw new BusinessException($"Invalid quantity for product {item.ProductID}");

                    var product = products.First(p => p.ProductID == item.ProductID);

                    if (product == null)
                        throw new KeyNotFoundException($"Product {item.ProductID} not found");

                    if (product.UnitsInStock < item.Quantity)
                        throw new BusinessException($"Product {item.ProductID} does not have enough stock");


                    var rand = new Random();
                    decimal discount = rand.Next(0, 26) * 0.01m;
                    decimal unitPrice = product.UnitPrice;

                    decimal before = unitPrice * item.Quantity;

                    decimal after = before - (before * discount);

                    var detail = new OrderDetail
                    {
                        ProductID = item.ProductID,
                        UnitPrice = unitPrice,
                        Quantity = item.Quantity,
                        Discount = discount,
                        TotalBeforeDiscount = before,
                        TotalAfterDiscount = after
                    };
                    product.UnitsInStock -= (short)item.Quantity;

                    totalBefore += before;
                    totalAfter += after;

                    order.OrderDetails.Add(detail);
                }
                order.TotalBeforeDiscount = totalBefore;
                order.TotalAfterDiscount = totalAfter;

                await _unitOfWork.OrderRepository.AddAsync(order, ct);
                await _unitOfWork.SaveAsync(ct);
                var paymentUrl = await _paymentService.CreatePayment(order.OrderID, userId);
                return new OrderCreateResponseDTO
                {
                    OrderId = order.OrderID,
                    PaymentUrl = paymentUrl,
                    CreatedAt = order.OrderDate,
                    Message = "Order Created - complete payment"
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public async Task<OrderUpdateResponseDTO> UpdateByIDAsync(int id, OrderUpdateDTO body)
        {

            if (body == null)
                throw new ValidationException("Body is empty");

            var validator = new OrderUpdateValidator();
            var result = validator.Validate(body);

            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            var order = await _unitOfWork.OrderRepository
                .Query(true)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderID == id);




            if (order == null)
                throw new NotFoundException("Order not found");

            _mapper.Map(body, order);

            var oldDetails = order.OrderDetails.ToList();

            var productIds = body.Items.Select(x => x.ProductID)
                .Union(oldDetails.Select(x => x.ProductID))
                .Distinct();

            var products = await _unitOfWork.ProductRepository
                .Query(true)
                .Where(x => productIds.Contains(x.ProductID))
                .ToListAsync();

            // حذف آیتم‌های حذف شده
            var removed = oldDetails
                .Where(od => !body.Items.Any(i => i.ProductID == od.ProductID))
                .ToList();

            foreach (var r in removed)
            {
                var product = products.First(p => p.ProductID == r.ProductID);

                product.UnitsInStock += r.Quantity;

                order.OrderDetails.Remove(r);
            }

            decimal totalBefore = 0;
            decimal totalAfter = 0;

            foreach (var item in body.Items)
            {
                var product = products.FirstOrDefault(p => p.ProductID == item.ProductID);

                if (product == null)
                    throw new NotFoundException($"Product {item.ProductID} not found");

                var existing = oldDetails.FirstOrDefault(x => x.ProductID == item.ProductID);

                decimal unitPrice = product.UnitPrice;
                decimal discount = 0;

                decimal before = unitPrice * item.Quantity;
                decimal after = before - (before * discount);

                if (existing != null)
                {
                    int diff = item.Quantity - existing.Quantity;

                    if (diff > 0)
                    {
                        if (product.UnitsInStock < diff)
                            throw new ValidationException($"Stock not enough for product {product.ProductID}");

                        product.UnitsInStock -= diff;
                    }
                    else if (diff < 0)
                    {
                        product.UnitsInStock += (-diff);
                    }

                    existing.Quantity = item.Quantity;
                    existing.UnitPrice = unitPrice;
                    existing.Discount = discount;
                    existing.TotalBeforeDiscount = before;
                    existing.TotalAfterDiscount = after;
                }
                else
                {
                    if (product.UnitsInStock < item.Quantity)
                        throw new ValidationException($"Stock not enough for product {product.ProductID}");

                    product.UnitsInStock -= item.Quantity;

                    order.OrderDetails.Add(new OrderDetail
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        UnitPrice = unitPrice,
                        Discount = discount,
                        TotalBeforeDiscount = before,
                        TotalAfterDiscount = after
                    });
                }

                totalBefore += before;
                totalAfter += after;
            }

            order.TotalBeforeDiscount = totalBefore;
            order.TotalAfterDiscount = totalAfter;

            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }

            return new OrderUpdateResponseDTO
            {
                OrderId = order.OrderID,
                UpdatedAt = DateTime.UtcNow,
                Message = "Order Updated"
            };
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Invalid id");

            var order = await _unitOfWork.OrderRepository
                .Query(true)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
                throw new NotFoundException("Order not found");

            if (order.OrderDetails.Any())
                _unitOfWork.OrderDetailRepository.RemoveRange(order.OrderDetails);

            _unitOfWork.OrderRepository.Remove(order);

            await _unitOfWork.SaveAsync();

        }


    }

}
