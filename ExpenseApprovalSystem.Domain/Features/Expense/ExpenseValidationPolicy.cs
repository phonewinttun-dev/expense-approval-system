namespace ExpenseApprovalSystem.Domain.Features.Expense;

public sealed class ExpenseValidationPolicy
{
    public Result ValidateRequest(RequestExpenseRequestModel request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return Result.Failure("Title is required.");

        if (string.IsNullOrWhiteSpace(request.Description))
            return Result.Failure("Description is required.");

        if (request.Amount < 0)
            return Result.Failure("Amount cannot be negative.");

        if (request.Amount > 1000000)
            return Result.Failure("Amount cannot be more than 1000000");

        if (string.IsNullOrWhiteSpace(request.RequestedBy))
            return Result.Failure("RequestedBy is required.");

        return Result.Success();
    }

    public Result ValidateApprove(ApproveExpenseRequestModel request)
    {
        if (request.Id <= 0)
            return Result.Failure("Valid expense Id is required.");

        if (string.IsNullOrWhiteSpace(request.ReviewedBy))
            return Result.Failure("ReviewedBy is required.");

        return Result.Success();
    }

    public Result ValidateReject(RejectExpenseRequestModel request)
    {
        if (request.Id <= 0)
            return Result.Failure("Valid expense Id is required.");

        if (string.IsNullOrWhiteSpace(request.ReviewedBy))
            return Result.Failure("ReviewedBy is required.");

        if (string.IsNullOrWhiteSpace(request.Reason))
            return Result.Failure("Reason is required.");

        return Result.Success();
    }

    public Result ValidateCanApprove(TblExpense expense, TblUser reviewer, TblUser? requester)
    {
        if (expense.Status != "Pending")
            return Result.Failure("Only pending expenses can be approved.");

        if (string.Equals(expense.RequestedBy, reviewer.Id, StringComparison.OrdinalIgnoreCase))
            return Result.Failure("Requester cannot approve their own expense.");

        if (expense.Amount > reviewer.Limit)
            return Result.Failure($"Expense amount exceeds reviewer limit ({reviewer.Limit}).");

        if (requester is not null && expense.Amount > requester.Limit)
            return Result.Failure($"Expense amount exceeds requester limit ({requester.Limit}).");

        return Result.Success();
    }

    public Result ValidateCanReject(TblExpense expense, string reviewedBy)
    {
        if (expense.Status != "Pending")
            return Result.Failure("Only pending expenses can be rejected.");

        if (string.Equals(expense.RequestedBy, reviewedBy, StringComparison.OrdinalIgnoreCase))
            return Result.Failure("Requester cannot reject their own expense.");

        return Result.Success();
    }
}
