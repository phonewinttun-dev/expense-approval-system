namespace ExpenseApprovalSystem.Domain.Features.Users;

public sealed record UserResponseModel(
    string Id,
    string Name,
    string Email,
    string? MobileNo,
    decimal Limit,
    string Status,
    DateTime CreatedAt);

public sealed record CreateUserRequestModel(
    string Name,
    string Email,
    string? MobileNo,
    decimal Limit);

public sealed record UpdateUserRequestModel(
    string Name,
    string Email,
    string? MobileNo,
    decimal Limit,
    string Status);
