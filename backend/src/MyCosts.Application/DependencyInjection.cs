using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyCosts.Application.Features.Auth.LoginUser;
using MyCosts.Application.Features.Auth.RegisterUser;
using MyCosts.Application.Features.ProductCategories.CreateProductCategory;
using MyCosts.Application.Features.ProductCategories.DeleteProductCategory;
using MyCosts.Application.Features.ProductCategories.GetProductCategories;
using MyCosts.Application.Features.ProductCategories.UpdateProductCategory;
using MyCosts.Application.Features.Products.CreateProduct;
using MyCosts.Application.Features.Products.DeleteProduct;
using MyCosts.Application.Features.Products.GetProducts;
using MyCosts.Application.Features.Products.UpdateProduct;
using MyCosts.Domain.Users;

namespace MyCosts.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddScoped<RegisterUserHandler>();
            services.AddScoped<LoginUserHandler>();
            services.AddScoped<CreateProductCategoryHandler>();
            services.AddScoped<UpdateProductCategoryHandler>();
            services.AddScoped<DeleteProductCategoryHandler>();
            services.AddScoped<GetProductCategoriesHandler>();
            services.AddScoped<CreateProductHandler>();
            services.AddScoped<UpdateProductHandler>();
            services.AddScoped<DeleteProductHandler>();
            services.AddScoped<GetProductsHandler>();

            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

            return services;
        }
    }
}
