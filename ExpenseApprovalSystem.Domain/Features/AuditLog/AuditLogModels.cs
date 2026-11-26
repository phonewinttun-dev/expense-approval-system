namespace ExpenseApprovalSystem.Domain.Features.AuditLog;

public sealed record ChangeRecord(
    string FieldName,
    string? OldValue,
    string? NewValue);
