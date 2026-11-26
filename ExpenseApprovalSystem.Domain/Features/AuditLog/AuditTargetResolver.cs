using ExpenseApprovalSystem.Database.AppDbContextModels;
using Microsoft.EntityFrameworkCore;

namespace ExpenseApprovalSystem.Domain.Features.AuditLog;

public sealed class AuditTargetResolver
{
    private readonly AppDbContext _db;

    public AuditTargetResolver(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(string? UserId, string? UserName, string? MobileNo)?> ResolveUserAsync(
        string? targetUserId,
        string targetId,
        CancellationToken cancellationToken = default)
    {
        var lookupId = targetUserId ?? targetId;
        var user = await _db.TblUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == lookupId, cancellationToken);

        return user is null ? null : (user.Id, user.Name, user.MobileNo);
    }
}
