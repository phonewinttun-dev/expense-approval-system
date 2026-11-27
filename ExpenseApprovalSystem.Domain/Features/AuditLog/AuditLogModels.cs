namespace ExpenseApprovalSystem.Domain.Features.AuditLog;

public sealed record ChangeRecord(
    string FieldName,
    string? OldValue,
    string? NewValue);

public sealed record AuditLogResponseModel(
    string Id,
    string ActorUserId,
    string? ActorRole,
    string Action,
    string ResourceType,
    string TargetId,
    string? TargetLookupKey,
    string? TargetUserId,
    string? TargetUserName,
    string? TargetUserMobileNo,
    string? Changes,
    string? Reason,
    DateTime ActionDateTime);
