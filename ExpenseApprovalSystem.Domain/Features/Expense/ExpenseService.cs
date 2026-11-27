using ExpenseApprovalSystem.Database.AppDbContextModels;
using ExpenseApprovalSystem.Domain.Features.AuditLog;
using ExpenseApprovalSystem.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ExpenseApprovalSystem.Domain.Features.Expense
{
    public class ExpenseService : IExpenseService
    {
        private readonly AppDbContext _db;
        private readonly ExpenseValidationPolicy _validationPolicy;
        private readonly IPublisher _publisher;

        public ExpenseService(AppDbContext db, ExpenseValidationPolicy validationPolicy, IPublisher publisher)
        {
            _db = db;
            _validationPolicy = validationPolicy;
            _publisher = publisher;
        }

        #region Request Expense
        public async Task<Result<ExpenseResponseModel>> RequestAsync(RequestExpenseRequestModel request, CancellationToken cancellationToken = default)
        {
            var validation = _validationPolicy.ValidateRequest(request);
            if (validation.IsFailure)
                return Result<ExpenseResponseModel>.Failure(validation.Message);

            var expense = new TblExpense
            {
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                Amount = request.Amount,
                Status = "Pending",
                RequestedBy = request.RequestedBy.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            _db.TblExpenses.Add(expense);
            await _db.SaveChangesAsync(cancellationToken);

            await _publisher.Publish(new AuditEvent(
                Action: "REQUEST",
                ResourceType: "Expense",
                TargetId: expense.Id.ToString(),
                TargetUserId: expense.RequestedBy,
                ActorUserId: expense.RequestedBy,
                Reason: "Expense created"
            ), cancellationToken);

            var response = new ExpenseResponseModel
            {
                Id = expense.Id,
                Title = expense.Title,
                Description = expense.Description,
                Amount = expense.Amount,
                Status = expense.Status,
                RequestedBy = expense.RequestedBy,
                ReviewedBy = expense.ReviewedBy,
                ReviewedAt = expense.ReviewedAt,
                CreatedAt = expense.CreatedAt,
                UpdatedAt = expense.UpdatedAt
            };

            return Result<ExpenseResponseModel>.Success(response, "Expense created.");
        }
        #endregion

        #region Approve
        public async Task<Result<ExpenseResponseModel>> ApproveAsync(ApproveExpenseRequestModel request, CancellationToken cancellationToken = default)
        {
            var validation = _validationPolicy.ValidateApprove(request);
            if (validation.IsFailure)
                return Result<ExpenseResponseModel>.Failure(validation.Message);

            var expense = await _db.TblExpenses.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
            if (expense is null)
                return Result<ExpenseResponseModel>.Failure("Expense not found.");

            var reviewedBy = request.ReviewedBy.Trim();

            var reviewer = await _db.TblUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == reviewedBy, cancellationToken);
            if (reviewer is null)
                return Result<ExpenseResponseModel>.Failure("Reviewer not found.");

            var requester = await _db.TblUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == expense.RequestedBy, cancellationToken);

            var policyCheck = _validationPolicy.ValidateCanApprove(expense, reviewer, requester);
            if (policyCheck.IsFailure)
                return Result<ExpenseResponseModel>.Failure(policyCheck.Message);

            var oldStatus = expense.Status;
            var now = DateTime.UtcNow;

            expense.Status = "Approved";
            expense.ReviewedBy = reviewedBy;
            expense.ReviewedAt = now;
            expense.UpdatedAt = now;

            await _db.SaveChangesAsync(cancellationToken);

            await _publisher.Publish(new AuditEvent(
                Action: "APPROVE",
                ResourceType: "Expense",
                TargetId: expense.Id.ToString(),
                TargetUserId: expense.RequestedBy,
                ActorUserId: reviewedBy,
                Reason: string.IsNullOrWhiteSpace(request.Reason) ? "Expense approved" : request.Reason.Trim(),
                OldStatus: oldStatus,
                NewStatus: "Approved"
            ), cancellationToken);

            var response = new ExpenseResponseModel
            {
                Id = expense.Id,
                Title = expense.Title,
                Description = expense.Description,
                Amount = expense.Amount,
                Status = expense.Status,
                RequestedBy = expense.RequestedBy,
                ReviewedBy = expense.ReviewedBy,
                ReviewedAt = expense.ReviewedAt,
                CreatedAt = expense.CreatedAt,
                UpdatedAt = expense.UpdatedAt
            };

            return Result<ExpenseResponseModel>.Success(response, "Expense approved.");
        }
        #endregion

        #region Reject 
        public async Task<Result<ExpenseResponseModel>> RejectAsync(RejectExpenseRequestModel request, CancellationToken cancellationToken = default)
        {
            var validation = _validationPolicy.ValidateReject(request);
            if (validation.IsFailure)
                return Result<ExpenseResponseModel>.Failure(validation.Message);

            var expense = await _db.TblExpenses
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
            if (expense is null)
                return Result<ExpenseResponseModel>.Failure("Expense not found.");

            var reviewedBy = request.ReviewedBy.Trim();

            var policyCheck = _validationPolicy.ValidateCanReject(expense, reviewedBy);
            if (policyCheck.IsFailure)
                return Result<ExpenseResponseModel>.Failure(policyCheck.Message);

            var reviewerExists = await _db.TblUsers
                .AsNoTracking()
                .AnyAsync(u => u.Id == reviewedBy, cancellationToken);
            if (!reviewerExists)
                return Result<ExpenseResponseModel>.Failure("Reviewer not found.");

            var oldStatus = expense.Status;
            var now = DateTime.UtcNow;

            expense.Status = "Rejected";
            expense.ReviewedBy = reviewedBy;
            expense.ReviewedAt = now;
            expense.UpdatedAt = now;

            await _db.SaveChangesAsync(cancellationToken);

            await _publisher.Publish(new AuditEvent(
                Action: "REJECT",
                ResourceType: "Expense",
                TargetId: expense.Id.ToString(),
                TargetUserId: expense.RequestedBy,
                ActorUserId: reviewedBy,
                Reason: string.IsNullOrWhiteSpace(request.Reason) ? "Expense rejected" : request.Reason.Trim(),
                OldStatus: oldStatus,
                NewStatus: "Rejected"
            ), cancellationToken);

            var response = new ExpenseResponseModel
            {
                Id = expense.Id,
                Title = expense.Title,
                Description = expense.Description,
                Amount = expense.Amount,
                Status = expense.Status,
                RequestedBy = expense.RequestedBy,
                ReviewedBy = expense.ReviewedBy,
                ReviewedAt = expense.ReviewedAt,
                CreatedAt = expense.CreatedAt,
                UpdatedAt = expense.UpdatedAt
            };

            return Result<ExpenseResponseModel>.Success(response, "Expense rejected.");
        }
        #endregion

        private static List<ChangeRecord> CollectChanges(TblExpense existing, string newStatus)
        {
            var changes = new List<ChangeRecord>();

            if (existing.Status != newStatus)
                changes.Add(new("Status", existing.Status, newStatus));

            return changes;
        }
    }
}
