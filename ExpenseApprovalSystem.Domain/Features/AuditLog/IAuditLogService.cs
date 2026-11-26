using ExpenseApprovalSystem.Database.AppDbContextModels;

namespace ExpenseApprovalSystem.Domain.Features.AuditLog;

public interface IAuditLogService
{
    Task LogAsync(TblAuditLog log, CancellationToken cancellationToken = default);
}
