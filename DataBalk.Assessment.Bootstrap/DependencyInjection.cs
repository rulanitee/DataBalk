using AutoMapper;
using DataBalk.Assessment.Data;
using DataBalk.Assessment.Data.Abstractions;
using DataBalk.Assessment.Services.Abstractions;
using DataBalk.Assessment.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DataBalk.Assessment.Bootstrap
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            IMapper mapper = MapProfiles();

            services.AddSingleton(mapper);
            services.AddDataServices(configuration);
            services.AddTransient<IRepository, Repository>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            return services;
        }

        private static IMapper MapProfiles()
        {            
            var profileTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t) && !t.IsAbstract);            
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                foreach (var profileType in profileTypes)
                {
                    cfg.AddProfile(Activator.CreateInstance(profileType) as Profile);
                }
            });            
            var mapper = mapperConfig.CreateMapper();
            return mapper;
        }
    }



}
