using IfinionBackendAssessment.DataAccess;
using IfinionBackendAssessment.DataAccess.CartRepository;
using IfinionBackendAssessment.DataAccess.CategoryRepository;
using IfinionBackendAssessment.DataAccess.ProductRepository;
using IfinionBackendAssessment.DataAccess.Repository;
using IfinionBackendAssessment.DataAccess.UserRepository;
using IfinionBackendAssessment.DataAccess.WishListRepository;
using IfinionBackendAssessment.Service.CategoryServices;
using IfinionBackendAssessment.Service.Common;
using IfinionBackendAssessment.Service.ImageService;
using IfinionBackendAssessment.Service.JWT;
using IfinionBackendAssessment.Service.MailService;
using IfinionBackendAssessment.Service.OrderServices;
using IfinionBackendAssessment.Service.ProductServices;
using IfinionBackendAssessment.Service.TransactionServices;
using IfinionBackendAssessment.Service.UserService;
using IfinionBackendAssessment.Service.WishListServices;
using Microsoft.Extensions.DependencyInjection;

namespace IfinionBackendAssessment.Service.ServiceExtensions
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IServiceCollection Services)
        {
            Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            Services.AddScoped<IUserRepository, UserRepository>();
            Services.AddScoped<IUserService, UserService.UserService>();
            Services.AddScoped<IEMailService, EMailService>();
            Services.AddScoped<IJWTService, JWTService>();
            Services.AddScoped<IphotoService, PhotoService>();
            Services.AddScoped<IProductService, ProductService>();
            Services.AddScoped<IProductRepo, ProductRepo>();
            Services.AddScoped<ICategoryRepo, CategoryRepo>();
            Services.AddScoped<ICartRepo, CartRepo>();
            Services.AddScoped<HelperMethods>();
            Services.AddScoped<ICategoryService, CategoryService>();
            Services.AddScoped<IOrderService, OrderService>();
            Services.AddScoped<IWishListRepo, WishListRepo>();
            Services.AddScoped<IWishListService, WishListService>();
            Services.AddScoped<ITransactionService, TransactionService>();
        }
    }
}
