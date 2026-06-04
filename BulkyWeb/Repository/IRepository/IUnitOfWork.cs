namespace BulkyWeb.Repository.IRepository
{
    public interface IUnitOfWork
    {



        ICategoryRepository CategoryRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderDetailRepository OrderDetailRepository { get; }

        IProductRepository ProductRepository { get; }
        //  ICustomerRepository CustomerRepository { get; }
        // IEmployeeRepository EmployeeRepository { get; }
        //ICategoryRepository Categories { get; }
        ISupplierRepository SupplierRepository { get; }
        IShipperRepository ShipperRepository { get; }
        IRegionRepository RegionRepository { get; }
        // ICustomerDemographicRepository CustomerDemographicRepository { get; }
        ITerritoryRepository TerritoryRepository { get; }
        //IRoleRepository RoleRepository { get; }





        Task SaveAsync(CancellationToken ct = default);











        IUserRepository UserRepository { get; }
        ITaskRepository TaskRepository { get; }
        IProjectRepository ProjectRepository { get; }
        ICommentRepository CommentRepository { get; }
        //void save();

        IPaymentGateway Payments { get; }

    }
}
