using ExpenseApprovalSystem.Database.AppDbContextModels;

namespace ExpenseApprovalSystem.Domain.Features.AuditLog;

public sealed class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _db;

    public AuditLogService(AppDbContext db)
    {
        _db = db;
    }

    public async Task LogAsync(TblAuditLog log, CancellationToken cancellationToken = default)
    {
        _db.TblAuditLogs.Add(log);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
