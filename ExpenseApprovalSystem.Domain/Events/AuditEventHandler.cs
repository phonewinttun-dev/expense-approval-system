namespace ExpenseApprovalSystem.Domain.Events;

public sealed class AuditEventHandler : INotificationHandler<AuditEvent>
{
    private readonly IAuditLogService _auditLogService;
    private readonly AuditTargetResolver _targetResolver;

    public AuditEventHandler(
        IAuditLogService auditLogService,
        AuditTargetResolver targetResolver)
    {
        _auditLogService = auditLogService;
        _targetResolver = targetResolver;
    }

    public async Task Handle(AuditEvent notification, CancellationToken cancellationToken)
    {
        var changesText = notification.Changes?.Count > 0
            ? string.Join(" | ", notification.Changes.Select(c =>
                $"{c.FieldName}: {c.OldValue} -> {c.NewValue}"))
            : null;

        if (!string.IsNullOrEmpty(notification.OldStatus) || !string.IsNullOrEmpty(notification.NewStatus))
        {
            var statusChange = $"Status: {notification.OldStatus} -> {notification.NewStatus}";
            changesText = string.IsNullOrEmpty(changesText)
                ? statusChange
                : $"{changesText} | {statusChange}";
        }

        var target = await _targetResolver.ResolveUserAsync(
            notification.TargetUserId,
            notification.TargetId,
            cancellationToken);

        var log = new TblAuditLog
        {
            Id = Guid.NewGuid().ToString(),
            ActorUserId = notification.ActorUserId,
            ActorRole = notification.ActorRole,
            Action = notification.Action.ToUpperInvariant(),
            ResourceType = notification.ResourceType,
            TargetId = notification.TargetId,
            TargetLookupKey = notification.TargetLookupKey,
            TargetUserId = target?.UserId ?? notification.TargetUserId,
            TargetUserName = target?.UserName,
            TargetUserMobileNo = target?.MobileNo,
            Changes = changesText,
            Reason = notification.Reason,
            ActionDateTime = DateTime.UtcNow
        };

        await _auditLogService.LogAsync(log, cancellationToken);
    }
}
