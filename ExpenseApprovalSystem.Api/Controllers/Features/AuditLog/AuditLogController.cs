using ExpenseApprovalSystem.Domain.Features.AuditLog;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApprovalSystem.Api.Controllers.Features.AuditLog;

[Route("api/[controller]")]
[ApiController]
public sealed class AuditLogController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _auditLogService.GetAllAsync(cancellationToken);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}
