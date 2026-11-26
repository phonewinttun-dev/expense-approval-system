using ExpenseApprovalSystem.Shared;

namespace ExpenseApprovalSystem.Domain.Features.Users;

public interface IUserService
{
    Task<Result<List<UserResponseModel>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<UserResponseModel>> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Result<UserResponseModel>> CreateAsync(CreateUserRequestModel request, CancellationToken cancellationToken = default);
    Task<Result<UserResponseModel>> UpdateAsync(string id, UpdateUserRequestModel request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
