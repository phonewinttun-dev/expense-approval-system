namespace ExpenseApprovalSystem.Domain.Features.Expense
{
    public sealed record ExpenseResponseModel
    {
        public int Id { get; init; }

        public string Title { get; init; } = null!;

        public string? Description { get; set; }

        public decimal Amount { get; init; }

        public string Status { get; init; } = null!;

        public string RequestedBy { get; init; } = null!;

        public string? ReviewedBy { get; init; }

        public DateTime? ReviewedAt { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime? UpdatedAt { get; init; }
    }

    public class RequestExpenseRequestModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public string RequestedBy { get; set; } = null!;
    }

    public class ApproveExpenseRequestModel
    {
        public int Id { get; set; }

        public string ReviewedBy { get; set; } = null!;

        public string? Reason { get; set; }
    }

    public class RejectExpenseRequestModel
    {
        public int Id { get; set; }

        public string ReviewedBy { get; set; } = null!;

        public string? Reason { get; set; }
    }
}
