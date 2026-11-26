using ExpenseApprovalSystem.Database.AppDbContextModels;
using ExpenseApprovalSystem.Shared;
using Microsoft.EntityFrameworkCore;

namespace ExpenseApprovalSystem.Domain.Features.Users;

public sealed class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly UserValidationPolicy _validationPolicy;

    public UserService(AppDbContext db, UserValidationPolicy validationPolicy)
    {
        _db = db;
        _validationPolicy = validationPolicy;
    }

    public async Task<Result<List<UserResponseModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _db.TblUsers
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => MapToResponse(u))
            .ToListAsync(cancellationToken);

        return Result<List<UserResponseModel>>.Success(users);
    }

    public async Task<Result<UserResponseModel>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _db.TblUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
            return Result<UserResponseModel>.Failure("User not found.");

        return Result<UserResponseModel>.Success(MapToResponse(user));
    }

    public async Task<Result<UserResponseModel>> CreateAsync(CreateUserRequestModel request, CancellationToken cancellationToken = default)
    {
        var validation = _validationPolicy.ValidateCreate(request);
        if (validation.IsFailure)
            return Result<UserResponseModel>.Failure(validation.Message);

        var emailExists = await _db.TblUsers
            .AnyAsync(u => u.Email == request.Email.Trim(), cancellationToken);
        if (emailExists)
            return Result<UserResponseModel>.Failure("Email already exists.");

        var user = new TblUser
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            MobileNo = request.MobileNo?.Trim(),
            Limit = request.Limit,
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };

        _db.TblUsers.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return Result<UserResponseModel>.Success(MapToResponse(user), "User created.");
    }

    public async Task<Result<UserResponseModel>> UpdateAsync(string id, UpdateUserRequestModel request, CancellationToken cancellationToken = default)
    {
        var validation = _validationPolicy.ValidateUpdate(request);
        if (validation.IsFailure)
            return Result<UserResponseModel>.Failure(validation.Message);

        var user = await _db.TblUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
            return Result<UserResponseModel>.Failure("User not found.");

        user.Name = request.Name.Trim();
        user.Email = request.Email.Trim();
        user.MobileNo = request.MobileNo?.Trim();
        user.Limit = request.Limit;
        user.Status = request.Status.Trim();

        await _db.SaveChangesAsync(cancellationToken);

        return Result<UserResponseModel>.Success(MapToResponse(user), "User updated.");
    }

    public async Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _db.TblUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
            return Result.Failure("User not found.");

        _db.TblUsers.Remove(user);
        await _db.SaveChangesAsync(cancellationToken);

        return Result.Success("User deleted.");
    }

    private static UserResponseModel MapToResponse(TblUser user) =>
        new(user.Id, user.Name, user.Email, user.MobileNo, user.Limit, user.Status, user.CreatedAt);
}
