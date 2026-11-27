namespace ExpenseApprovalSystem.Domain.Features.Users;

public sealed record UserResponseModel
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? MobileNo { get; init; } = string.Empty;
    public decimal Limit { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public class CreateUserRequestModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? MobileNo { get; set; }
    public decimal Limit { get; set; }
}

public class UpdateUserRequestModel
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? MobileNo { get; set; }
    public decimal? Limit { get; set; }
    public string? Status { get; set; }
}
