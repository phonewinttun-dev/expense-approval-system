using ExpenseApprovalSystem.Database.AppDbContextModels;
using ExpenseApprovalSystem.Domain.Features.AuditLog;
using ExpenseApprovalSystem.Domain.Features.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseApprovalSystem.Domain
{
    public static class FeatureManager
    {
        public static IServiceCollection AddDomainServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(FeatureManager).Assembly));

            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<AuditTargetResolver>();

            services.AddScoped<UserValidationPolicy>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
