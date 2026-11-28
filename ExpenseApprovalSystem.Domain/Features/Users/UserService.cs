namespace ExpenseApprovalSystem.Domain.Features.Users;

public sealed class UserService : IUserService
{
    private readonly AppDbContext _db;
    private readonly UserValidationPolicy _validationPolicy;
    private readonly IPublisher _publisher;

    public UserService(
        AppDbContext db,
        UserValidationPolicy validationPolicy,
        IPublisher publisher)
    {
        _db = db;
        _validationPolicy = validationPolicy;
        _publisher = publisher;
    }

    #region Get All Users
    public async Task<Result<List<UserResponseModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _db.TblUsers
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserResponseModel
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                MobileNo = u.MobileNo,
                Limit = u.Limit,
                Status = u.Status,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Result<List<UserResponseModel>>.Success(users);
    }
    #endregion

    #region Get User By Id
    public async Task<Result<UserResponseModel>> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _db.TblUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
            return Result<UserResponseModel>.Failure("User not found.");

        var response = new UserResponseModel
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            MobileNo = user.MobileNo,
            Limit = user.Limit,
            Status = user.Status,
            CreatedAt = user.CreatedAt
        };
        return Result<UserResponseModel>.Success(response);
    }
    #endregion

    #region Create User
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

        await _publisher.Publish(new AuditEvent(
            Action: "CREATE",
            ResourceType: "User",
            TargetId: user.Id,
            TargetUserId: user.Id,
            ActorUserId: "system",
            Reason: "User registered"
        ), cancellationToken);

        var response = new UserResponseModel
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            MobileNo = user.MobileNo,
            Limit = user.Limit,
            Status = user.Status,
            CreatedAt = user.CreatedAt
        };
        return Result<UserResponseModel>.Success(response, "User created successfully.");
    }
    #endregion

    #region Update User
    public async Task<Result<UserResponseModel>> UpdateAsync(string id, UpdateUserRequestModel request, CancellationToken cancellationToken = default)
    {
        var validation = _validationPolicy.ValidateUpdate(request);
        if (validation.IsFailure)
            return Result<UserResponseModel>.Failure(validation.Message);

        var user = await _db.TblUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
            return Result<UserResponseModel>.Failure("User not found.");

        var changes = CollectChanges(user, request);
        var oldStatus = user.Status;
        var newStatus = request.Status is not null ? request.Status.Trim() : user.Status;

        if (request.Name is not null)
            user.Name = request.Name.Trim();

        if (request.Email is not null)
            user.Email = request.Email.Trim();

        if (request.MobileNo is not null)
            user.MobileNo = request.MobileNo.Trim();

        if (request.Limit.HasValue)
            user.Limit = request.Limit.Value;

        if (request.Status is not null)
            user.Status = newStatus;

        await _db.SaveChangesAsync(cancellationToken);

        if (changes.Count > 0 || oldStatus != newStatus)
        {
            await _publisher.Publish(new AuditEvent(
                Action: "UPDATE",
                ResourceType: "User",
                TargetId: user.Id,
                TargetUserId: user.Id,
                ActorUserId: "system",
                Changes: changes,
                OldStatus: oldStatus != newStatus ? oldStatus : null,
                NewStatus: oldStatus != newStatus ? newStatus : null
            ), cancellationToken);
        }

        var response = new UserResponseModel
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            MobileNo = user.MobileNo,
            Limit = user.Limit,
            Status = user.Status,
            CreatedAt = user.CreatedAt
        };
        return Result<UserResponseModel>.Success(response, "User updated successfully.");
    }
    #endregion

    #region Delete User
    public async Task<Result> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _db.TblUsers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
            return Result.Failure("User not found.");

        _db.TblUsers.Remove(user);
        await _db.SaveChangesAsync(cancellationToken);

        await _publisher.Publish(new AuditEvent(
            Action: "DELETE",
            ResourceType: "User",
            TargetId: user.Id,
            TargetUserId: user.Id,
            ActorUserId: "system",
            Reason: $"Deleted user {user.Name} ({user.Email})"
        ), cancellationToken);

        return Result.Success("User deleted.");
    }
    #endregion

    private static List<ChangeRecord> CollectChanges(TblUser existing, UpdateUserRequestModel updated)
    {
        var changes = new List<ChangeRecord>();

        if (updated.Name is not null && existing.Name != updated.Name.Trim())
            changes.Add(new("Name", existing.Name, updated.Name.Trim()));

        if (updated.Email is not null && existing.Email != updated.Email.Trim())
            changes.Add(new("Email", existing.Email, updated.Email.Trim()));

        if (updated.MobileNo is not null && existing.MobileNo != updated.MobileNo.Trim())
            changes.Add(new("MobileNo", existing.MobileNo, updated.MobileNo.Trim()));

        if (updated.Limit.HasValue && existing.Limit != updated.Limit.Value)
            changes.Add(new("Limit", existing.Limit.ToString(), updated.Limit.Value.ToString()));

        return changes;
    }
}
