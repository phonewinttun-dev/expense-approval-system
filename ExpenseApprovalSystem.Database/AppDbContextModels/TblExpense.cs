using System;
using System.Collections.Generic;

namespace ExpenseApprovalSystem.Database.AppDbContextModels;

public partial class TblExpense
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = null!;

    public string RequestedBy { get; set; } = null!;

    public string? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
