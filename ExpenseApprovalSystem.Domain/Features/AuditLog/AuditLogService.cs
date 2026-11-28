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

    public async Task<Result<List<AuditLogResponseModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var logs = await _db.TblAuditLogs
            .AsNoTracking()
            .OrderByDescending(l => l.ActionDateTime)
            .Select(l => new AuditLogResponseModel(
                l.Id,
                l.ActorUserId,
                l.ActorRole,
                l.Action,
                l.ResourceType,
                l.TargetId,
                l.TargetLookupKey,
                l.TargetUserId,
                l.TargetUserName,
                l.TargetUserMobileNo,
                l.Changes,
                l.Reason,
                l.ActionDateTime))
            .ToListAsync(cancellationToken);

        return Result<List<AuditLogResponseModel>>.Success(logs);
    }
}
