using ExpenseApprovalSystem.Shared;

namespace ExpenseApprovalSystem.Domain.Features.Users;

public sealed class UserValidationPolicy
{
    public Result ValidateCreate(CreateUserRequestModel request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result.Failure("Name is required.");

        if (string.IsNullOrWhiteSpace(request.Email))
            return Result.Failure("Email is required.");

        if (request.Limit < 0)
            return Result.Failure("Limit cannot be negative.");

        return Result.Success();
    }

    public Result ValidateUpdate(UpdateUserRequestModel request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result.Failure("Name is required.");

        if (string.IsNullOrWhiteSpace(request.Email))
            return Result.Failure("Email is required.");

        if (request.Limit < 0)
            return Result.Failure("Limit cannot be negative.");

        if (string.IsNullOrWhiteSpace(request.Status))
            return Result.Failure("Status is required.");

        return Result.Success();
    }
}
