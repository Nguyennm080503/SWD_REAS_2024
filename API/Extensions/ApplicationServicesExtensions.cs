﻿using API.Data;
using API.Errors;
using API.Helper;
using API.Interfaces;
using API.Repository;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace API.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection ApplicationServices(this IServiceCollection services
            , IConfiguration config)
        {
            services.AddScoped<IRealEstateRepository, RealEstateRepository>();
            services.AddScoped<IRealEstateDetailRepository, RealEstateDetailRepository>();
            services.AddScoped<IRealEstatePhotoRepository, RealEstatePhotoRepository>();
            services.AddScoped<IRuleRepository, RuleRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAuctionRepository, AuctionRepository>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<IMoneyTransactionDetailRepository, MoneyTransactionDetailRepository>();
            services.AddScoped<IMoneyTransactionRepository, MoneyTransactionRepository>();
            services.AddScoped<IDepositAmountRepository, DepositAmountRepository>();
            services.AddScoped<ITypeReasRepository, TypeReasRepository>();

            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<ITokenService, TokenService>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); //the current position of the mapping profile
            services.Configure<CloudinarySetting>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage).ToArray();

                    var errorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(errorResponse);
                };
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
