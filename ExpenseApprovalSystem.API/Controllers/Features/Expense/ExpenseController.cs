using ExpenseApprovalSystem.Domain.Features.Expense;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseApprovalSystem.API.Controllers.Features.Expense
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly ExpenseService _expenseService;

        public ExpenseController(ExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        public async Task<IActionResult> RequestExpense(RequestExpenseRequestModel request, CancellationToken cancellationToken)
        {
            if (request is null) return BadRequest();
            var result = await _expenseService.RequestAsync(request, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("approve")]
        public async Task<IActionResult> ApproveExpense(ApproveExpenseRequestModel request, CancellationToken cancellationToken)
        {
            if (request is null) return BadRequest();
            var result = await _expenseService.ApproveAsync(request, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectExpense(RejectExpenseRequestModel request, CancellationToken cancellationToken)
        {
            if (request is null) return BadRequest();
            var result = await _expenseService.RejectAsync(request, cancellationToken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }


    }
}
