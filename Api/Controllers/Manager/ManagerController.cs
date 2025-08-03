using Microsoft.AspNetCore.Mvc;
using LeaveRequestSystem.Application.Services;
using LeaveRequestSystem.Application.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;

namespace LeaveRequestSystem.Api.Controllers.Manager
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManagerController : ControllerBase
    {
        private readonly ByManagerService managerService;

        public ManagerController(ByManagerService _managerService)
        {
            managerService = _managerService;
        }

        // موافقة المدير المباشر
        [HttpPost("{leaveId}/approve")]
        [Authorize(Roles = "MANAGER")] // فقط المدراء يقدرون ينفذون
        public async Task<IActionResult> ApproveByManager(int leaveId)
        {
            
            // جلب managerId من الـ Claims
            var managerIdString = User.Claims.FirstOrDefault(c => c.Type == "ManagerId")?.Value;
            if (string.IsNullOrEmpty(managerIdString))
                return Unauthorized("لم يتم العثور على معرف المدير!");
            var managerId = int.Parse(managerIdString);

            var response = await managerService.ApproveByManagerAsync(leaveId, managerId);
            return Ok(response);
        }

        // رفض المدير المباشر
        [HttpPost("{leaveId}/reject")]
        [Authorize(Roles = "MANAGER")]
        public async Task<IActionResult> RejectByManager(int leaveId)
        {
            var managerIdString = User.Claims.FirstOrDefault(c => c.Type == "ManagerId")?.Value;
            if (string.IsNullOrEmpty(managerIdString))
                return Unauthorized("لم يتم العثور على معرف المدير!");
            var managerId = int.Parse(managerIdString);

            var response = await managerService.RejectByManagerAsync(leaveId, managerId);
            return Ok(response);
        }


    }
}
