using ExpenseApprovalSystem.Shared;

namespace ExpenseApprovalSystem.Domain.Features.Expense
{
    public interface IExpenseService
    {
        Task<Result<ExpenseResponseModel>> RequestAsync(RequestExpenseRequestModel request, CancellationToken cancellationToken = default);
        Task<Result<ExpenseResponseModel>> ApproveAsync(ApproveExpenseRequestModel request, CancellationToken cancellationToken = default);
        Task<Result<ExpenseResponseModel>> RejectAsync(RejectExpenseRequestModel request, CancellationToken cancellationToken = default);
    }
}
