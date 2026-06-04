using AutoMapper;
using BulkyWeb.Models;
using BulkyWeb.Models.DTO.Category;
using BulkyWeb.Models.DTO.Order;
using BulkyWeb.Models.DTO.Product;
using BulkyWeb.Models.Orders;
using BulkyWeb.Models.RoleModels;
using Microsoft.AspNetCore.Identity;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

        //Role
        CreateMap<IdentityRole, RoleGetDTO>();
        CreateMap<RoleCreateDTO, IdentityRole>();


        //Category
        CreateMap<Category, CategoryGetAllDTO>();
        CreateMap<Category, CategoryShortDTO>();
        CreateMap<Category, CategoryShortInAllDTO>();
        CreateMap<Category, CategoryGetByIdDTO>()
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
        CreateMap<CategoryCreateDTO, Category>();
        CreateMap<CategoryUpdateDTO, Category>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));




        //Product
        // Product Read
        CreateMap<Product, ProductGetAllDTO>()
                 .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.CategoryName : null));

        CreateMap<Product, ProductGetByIdDTO>()
                 .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.CategoryName : null))
                 .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category));

        // Product for Category details
        CreateMap<Product, CategoryProductDTO>();

        // Product Write
        CreateMap<ProductCreateDTO, Product>();
        CreateMap<ProductCreateDTO, Product>();
        CreateMap<ProductUpdateDTO, Product>()
                  .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // Supplier
        CreateMap<Supplier, SupplierShortDTO>();



        //Order
        CreateMap<Order, OrderGetAllDTO>();
        CreateMap<Shipper, OrderShipperShortDTO>();
        CreateMap<Order, OrderGetByIdDTO>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderDetails));
        CreateMap<OrderDetail, OrderDetailDTO>();
        // Order Create
        CreateMap<OrderCreateDTO, Order>()
            .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.Items))
            .ForMember(x => x.OrderDate, opt => opt.Ignore())
            .ForMember(x => x.RequiredDate, opt => opt.Ignore());
        CreateMap<OrderDetailDTO, OrderDetail>();

        // Order Update
        CreateMap<OrderUpdateDTO, Order>()
            .ForMember(x => x.RequiredDate, opt => opt.Ignore())
            .ForMember(x => x.OrderDate, opt => opt.Ignore())
            .ForMember(x => x.OrderDetails, opt => opt.Ignore());
        // .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));









        //---------------------------------------------------------------------------------------------------------------
        //old

        //    CreateMap<Category, CategoryGetAllDTO>();
        //    //CreateMap<CategoryCreateDTO, Category>();

        //    CreateMap<CategoryUpdateDTO, Category>().ForAllMembers(opt =>
        //opt.Condition((src, dest, srcMember) => srcMember != null));


        //    CreateMap<CategoryCreateDTO, Category>();

        //    CreateMap<Product, ProductGetAllDTO>().ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category))
        //        ;
        //    CreateMap<Category, CategoryShortInAllDTO>();

        //    CreateMap<Product, ProductGetByIdDTO>()
        //        .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category))
        //        ;

        //    CreateMap<Supplier, SupplierShortDTO>();
        //    CreateMap<Category, CategoryShortDTO>();
        //    CreateMap<Category, CategoryGetByIdDTO>().ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
        //    CreateMap<Product, CategoryProductDTO>();
        //    CreateMap<ProductCreateDTO, Product>();

        //    //CategoryGetByIdDTO
        //    //CreateMap<ProductCreateDTO, Product>();

        //    // اگر برای Update هم AutoMapper می‌خوای:
        //    CreateMap<ProductUpdateDTO, Product>().ForAllMembers(opt =>
        //        opt.Condition((src, dest, srcMember) => srcMember != null));



        //    CreateMap<Order, OrderGetAllDTO>();
        //    CreateMap<Shipper, OrderShipperShortDTO>();
        //    CreateMap<Order, OrderGetByIdDTO>()
        //.ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderDetails));

        //    CreateMap<OrderDetail, OrderDetailDTO>();
        //    CreateMap<OrderCreateDTO, Order>()
        //        .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.Items));


        //    CreateMap<OrderDetailDTO, OrderDetail>();
        //    CreateMap<OrderCreateDTO, Order>()
        //        .ForMember(x => x.OrderDate, opt => opt.Ignore())
        //        .ForMember(x => x.RequiredDate, opt => opt.Ignore());

        //    CreateMap<OrderUpdateDTO, Order>()
        //         .ForMember(x => x.RequiredDate, opt => opt.Ignore())
        //         .ForMember(x => x.OrderDate, opt => opt.Ignore())
        //         .ForMember(x => x.OrderDetails, opt => opt.Ignore())

        //         //.ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null))
        //         ;



    }




}
