using System;
using System.Collections.Generic;

namespace ExpenseApprovalSystem.Database.AppDbContextModels;

public partial class TblUser
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? MobileNo { get; set; }

    public decimal Limit { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
