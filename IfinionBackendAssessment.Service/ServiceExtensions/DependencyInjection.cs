using IfinionBackendAssessment.DataAccess.CategoryRepository;
using IfinionBackendAssessment.DataAccess.ProductRepository;
using IfinionBackendAssessment.DataAccess.Repository;
using IfinionBackendAssessment.DataAccess.UnitOfWork;
using IfinionBackendAssessment.DataAccess.UserRepository;
using IfinionBackendAssessment.Service.ImageService;
using IfinionBackendAssessment.Service.JWT;
using IfinionBackendAssessment.Service.MailService;
using IfinionBackendAssessment.Service.ProductServices;
using IfinionBackendAssessment.Service.UserService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            ;            
        }
    }
}
