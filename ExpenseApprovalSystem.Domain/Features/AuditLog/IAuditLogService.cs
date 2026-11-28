namespace ExpenseApprovalSystem.Domain.Features.AuditLog;

public interface IAuditLogService
{
    Task LogAsync(TblAuditLog log, CancellationToken cancellationToken = default);
    Task<Result<List<AuditLogResponseModel>>> GetAllAsync(CancellationToken cancellationToken = default);
}
