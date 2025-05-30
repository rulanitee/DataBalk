﻿using DataBalk.Assessment.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataBalk.Assessment.Bootstrap
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataBalkAssessmentContext>(opt =>
                opt.UseSqlite(configuration.GetConnectionString("Default")));
        }

        public static void InitializeDatabase(this IApplicationBuilder app)
        {
            var scopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<DataBalkAssessmentContext>();

            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Any())
            {
                context.Database.Migrate();
            }
        }
    }
}
