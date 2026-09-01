using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Application.Interfaces;
using Application.ServiceImpl;
using Domain.IRepository;
using Infrastructure.Data;
using ApartmentManagementSystem.Infrastructure.Repositories;

namespace ApartmentManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((context, services) =>
                    {
                        var configuration = context.Configuration;

                        // 1. DbContext
                        services.AddDbContext<AppDbContext>(options =>
                            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                        // 2. DI Repositories
                        services.AddScoped<IUserRepository, UserRepository>();
                        services.AddScoped<IApartmentRepository, ApartmentRepository>();
                        services.AddScoped<ITenantRepository, TenantRepository>();
                        services.AddScoped<IIssueRepository, IssueRepository>();
                        services.AddScoped<IParcelRepository, ParcelRepository>();
                        services.AddScoped<IPaymentRecordRepository, PaymentRecordRepository>();
                        services.AddScoped<IRoleRepository, RoleRepository>();

                        // 3. DI Services
                        services.AddScoped<IUserService, UserServiceImpl>();
                        services.AddScoped<IApartmentService, ApartmentServiceImpl>();
                        services.AddScoped<ITenantService, TenantServiceImpl>();
                        services.AddScoped<IIssueService, IssueServiceImpl>();
                        services.AddScoped<IParcelService, ParcelServiceImpl>();
                        services.AddScoped<IPaymentRecordService, PaymentRecordServiceImpl>();
                        services.AddScoped<IRoleService, RoleServiceImpl>();

                        // 4. AutoMapper
                        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

                        // 5. Controllers
                        services.AddControllers();

                        // 6. Swagger Config
                        services.AddSwaggerGen(c =>
                        {
                            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Apartment Management API", Version = "v1" });
                        });
                    });

                    webBuilder.Configure((context, app) =>
                    {
                        var env = context.HostingEnvironment;

                        // Configure pipeline
                        if (env.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }

                        // Swagger Middleware
                        app.UseSwagger();
                        app.UseSwaggerUI(c =>
                        {
                            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Apartment Management API v1");
                        });

                        app.UseHttpsRedirection();

                        // Required in .NET 5 for Routing and Endpoints
                        app.UseRouting();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                });
    }
}
