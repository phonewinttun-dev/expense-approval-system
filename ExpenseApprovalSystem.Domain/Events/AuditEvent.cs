namespace ExpenseApprovalSystem.Domain.Events;

public sealed record AuditEvent(
    string Action,
    string ResourceType,
    string TargetId,
    string? TargetUserId,
    string ActorUserId,
    string? ActorRole = null,
    string? Reason = null,
    List<ChangeRecord>? Changes = null,
    string? OldStatus = null,
    string? NewStatus = null,
    string? TargetLookupKey = null
) : INotification;
