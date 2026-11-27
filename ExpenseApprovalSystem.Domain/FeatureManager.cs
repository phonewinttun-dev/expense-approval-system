using ExpenseApprovalSystem.Database.AppDbContextModels;
using ExpenseApprovalSystem.Domain.Features.AuditLog;
using ExpenseApprovalSystem.Domain.Features.Expense;
using ExpenseApprovalSystem.Domain.Features.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseApprovalSystem.Domain
{
    public static class FeatureManager
    {
        public static void AddDomain(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(FeatureManager).Assembly));

            builder.Services.AddScoped<IAuditLogService, AuditLogService>();
            builder.Services.AddScoped<AuditTargetResolver>();

            builder.Services.AddScoped<UserValidationPolicy>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<ExpenseValidationPolicy>();
            builder.Services.AddScoped<IExpenseService, ExpenseService>();
        }
    }
}
