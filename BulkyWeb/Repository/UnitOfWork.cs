using BulkyWeb.Data;
using BulkyWeb.Repository.IRepository;

namespace BulkyWeb.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ILogger<UnitOfWork> _logger;
        private ApplicationDbContext _db;
        public ICategoryRepository CategoryRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public IOrderRepository OrderRepository { get; private set; }
        public IOrderDetailRepository OrderDetailRepository { get; private set; }
        public IShipperRepository ShipperRepository { get; private set; }
        public ISupplierRepository SupplierRepository { get; private set; }
        //public ICustomerRepository CustomerRepository { get; private set; }
        // public IEmployeeRepository EmployeeRepository { get; private set; }
        public IRegionRepository RegionRepository { get; private set; }
        // public ICustomerDemographicRepository CustomerDemographicRepository { get; private set; }
        public ITerritoryRepository TerritoryRepository { get; private set; }
        //public IRoleRepository RoleRepository { get; private set; }







        public ITaskRepository TaskRepository { get; private set; }
        public IProjectRepository ProjectRepository { get; private set; }

        public ICommentRepository CommentRepository { get; private set; }
        public IUserRepository UserRepository { get; private set; }

        public IPaymentGateway Payments { get; private set; }

        //  public ICustomerDemographicRepository CustomerDemographicRepository => throw new NotImplementedException();

        //  public IEmployeeRepository EmployeeRepository => throw new NotImplementedException();

        public UnitOfWork(ApplicationDbContext db, ILogger<UnitOfWork> logger)
        {
            _db = db;
            //CategoryRepository = new CategoryRepository(_db);
            //ProductRepository = new ProductRepository(_db);
            //CommentRepository = new CommentRepository(_db);
            //ProjectRepository = new ProjectRepository(_db);
            //UserRepository = new UserRepository(_db);
            //TaskRepository = new TaskRepository(_db);


            CategoryRepository = new CategoryRepository(_db);
            ProductRepository = new ProductRepository(_db);
            OrderRepository = new OrderRepository(_db);
            // CustomerRepository = new CustomerRepository(_db);
            OrderDetailRepository = new OrderDetailRepository(_db);





            _logger = logger;
        }


        public async Task SaveAsync(CancellationToken ct = default)
        {
            await _db.SaveChangesAsync(ct);
        }
        public void save()
        {
            //    _logger.LogInformation("UnitOfWork.Save started");

            //    foreach (var entry in _db.ChangeTracker.Entries())
            //    {
            //        _logger.LogInformation(
            //            "Entity: {Entity}, State: {State}",
            //            entry.Entity.GetType().Name,
            //            entry.State
            //        );
            //    }
            //    try
            //    {
            //        _db.SaveChanges();
            //        _logger.LogInformation("UnitOfWork.Save succeeded");
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, "DB ERROR | {Message} | Inner: {Inner}",
            //ex.Message,
            //ex.InnerException?.Message);
            //        throw;
            //    }
            //    //_db.SaveChanges();
        }
    }
}
