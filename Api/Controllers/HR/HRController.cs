using Microsoft.AspNetCore.Mvc;
using LeaveRequestSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LeaveRequestSystem.Api.Controllers
{
    // Controller for HR-related operations
    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "HR")]
    public class HRController : ControllerBase
    {
        private readonly HRService _hrService;

        public HRController(HRService hrService)
        {
            _hrService = hrService;
        }

        // Approve leave request by HR
        [HttpPost("{leaveId}/approve")]
        public async Task<IActionResult> ApproveByHR(int leaveId)
        {
            try
            {
                var hrIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(hrIdString))
                    return Unauthorized("HR user not found!");

                var hrId = int.Parse(hrIdString);
                var response = await _hrService.ApproveByHRAsync(leaveId, hrId);

                return Ok(new
                {
                    success = true,
                    message = "Leave request approved by HR successfully",
                    data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // Reject leave request by HR
        [HttpPost("{leaveId}/reject")]
        public async Task<IActionResult> RejectByHR(int leaveId, [FromBody] RejectRequestDto request)
        {
            try
            {
                var hrIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(hrIdString))
                    return Unauthorized("HR user not found!");

                var hrId = int.Parse(hrIdString);
                var response = await _hrService.RejectByHRAsync(leaveId, hrId, request.Reason);

                return Ok(new
                {
                    success = true,
                    message = "Leave request rejected by HR",
                    data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // Get pending HR approvals
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingHRApprovals()
        {
            try
            {
                var result = await _hrService.GetPendingHRApprovalsAsync();
                return Ok(new
                {
                    success = true,
                    count = result.Count(),
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // Get all leave requests
        [HttpGet("all")]
        public async Task<IActionResult> GetAllLeaves()
        {
            try
            {
                var result = await _hrService.GetAllLeavesForHRAsync();
                return Ok(new
                {
                    success = true,
                    count = result.Count(),
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // Get leave statistics
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var stats = await _hrService.GetLeaveStatisticsAsync();
                return Ok(new
                {
                    success = true,
                    data = stats
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
