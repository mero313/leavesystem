using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeaveRequestSystem.Application.DTOs;
using LeaveRequestSystem.Application.Services;
using System.Security.Claims;
using LeaveRequestSystem.Domain.Entities;

using LeaveRequestSystem.Domain.Enums;

namespace LeaveRequestSystem.Api.Controllers
{
    [ApiController]
    [Route("api/hr")]
    [Authorize(Roles = "HR")]
    public class HRController : ControllerBase
    {
        private readonly HRService _hrService;
        private readonly UserService _userService;
        private readonly DepartmentService _departmentService;

        public HRController(HRService hrService, UserService userService, DepartmentService departmentService)
        {
            _hrService = hrService;
            _userService = userService;
            _departmentService = departmentService;
        }


        // Approve or reject leave requests by HR
        /// <summary>
        /// Approve a leave request by HR.
        /// </summary>
        /// <param name="leaveId"> id of leave request </param>
        /// <param name="dto"> reason of Approve    </param>
        /// <returns></returns>

        [HttpPost("approve/{leaveId:int}")]
        public async Task<IActionResult> Approve(int leaveId, [FromBody] DecisionDto dto)
        {
            var hrIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(hrIdStr)) return Unauthorized();
            var hrId = int.Parse(hrIdStr);

            var res = await _hrService.ApproveByHRAsync(leaveId, hrId, dto?.Reason);
            return Ok(res);
        }


        /// <summary>
        /// Reject a leave request by HR.
        /// </summary>
        /// <param name="leaveId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("reject/{leaveId:int}")]
        public async Task<IActionResult> Reject(int leaveId, [FromBody] DecisionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Reason))
                return BadRequest(new { message = "Reason is required to reject" });

            var hrIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(hrIdStr)) return Unauthorized();
            var hrId = int.Parse(hrIdStr);

            var res = await _hrService.RejectByHRAsync(leaveId, hrId, dto.Reason!);
            return Ok(res);
        }
        [HttpPost("{departmentId:int}/assign-manager")]
        public async Task<IActionResult> AssignManager(int departmentId, int manageruserid, CancellationToken ct)
        {
            var result = await _departmentService.AssignManagerAsync(departmentId, manageruserid, ct);
            return Ok(result);
        }


        /// <summary>
        /// Promote a user to manager.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("to-manager")]
        public async Task<IActionResult> Manager([FromBody] PromotionManagerDto dto, CancellationToken ct)
        {

            await _userService.Promotion2Manager(dto.UserId, ct);
            return Ok(new { message = "Updated" });
        }


        /// <summary>
        /// Demote a manager to employee.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="removeFromDepartment"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("to-employee")]
        public async Task<IActionResult> Employee([FromBody] PromotionManagerDto dto, bool removeFromDepartment, CancellationToken ct)
        {

            await _userService.Demote2Employee(dto.UserId, removeFromDepartment, ct);
            return Ok(new { message = "Updated" });
        }


        /// <summary>
        /// Toggle a user's active status.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("toggle-active")]
        public async Task<IActionResult> ToggleActive([FromBody] ToggleActiveDto dto)
        {
            await _userService.ToggleActiveAsync(dto.UserId, dto.IsActive);
            return Ok(new
            {
                message = "the user status has been changed - " + (dto.IsActive ? "Activated" : "Deactivated"),
            });
        }


        /// <summary>
        /// Get a paginated list of users with filtering and sorting options.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="search"></param>
        /// <param name="departmentId"></param>
        /// <param name="managerId"></param>
        /// <param name="isActive"></param>
        /// <param name="sortBy"></param>
        /// <param name="desc"></param>
        /// <param name="role"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("All-users")]
        public async Task<IActionResult> UserManagement([FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] int? departmentId = null,
            [FromQuery] int? managerId = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? sortBy = "id",
            [FromQuery] bool desc = true,
            [FromQuery] Role? role = null,
            CancellationToken ct = default)
        {

            var q = new UserQuery
            {
                Page = page,
                PageSize = pageSize,
                Search = search,
                DepartmentId = departmentId,
                ManagerId = managerId,
                IsActive = isActive,
                SortBy = sortBy,
                Desc = desc,
                Role = role
            };
            var result = await _userService.SearchUsersAsync(q, ct);
            return Ok(new
            {
                success = true,
                total = result.TotalCount,
                page = result.Page,
                pageSize = result.PageSize,
                items = result.Items

            });

        }
        /// <summary>
        /// Create a new department.
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("departments")]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentRequestDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var result = await _departmentService.CreateAsync(dto, ct);
            return Ok(result);
        }

        /// <summary>
        /// Get department details by ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("departments/{id:int}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var dep = await _departmentService.GetDepByIdAsync(id);
            return Ok(dep);
        }


        /// <summary>
        /// Get department statistics, optionally including user details.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="users"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("department-stats")]
        public async Task<IActionResult> GetDepartmentStats(int id, bool users, CancellationToken ct)
        {
            var stats = await _departmentService.GetDepartmentWithStatsAsync(id, users, ct);
            return Ok(stats);
        }


        /// <summary>
        /// Get all departments with statistics.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("department-list")]
        public async Task<List<DepartmentWithStatsDto>> ListDepartmentsWithCount(CancellationToken ct)
        {
            var departments = await _departmentService.ListDepartmentsWithCountsAsync(ct);
            return departments;
        }


        /// <summary>
        /// Add an employee to a department.
        /// </summary>
        /// <param name="moveIfInAnotherDept"></param>
        /// <param name="departmentId"></param>
        /// <param name="dto"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("{departmentId:int}/employees")]
        public async Task<IActionResult> AddEmployee(bool moveIfInAnotherDept,
            int departmentId,
            [FromBody] AddEmployeeToDepartmentDto dto,
            CancellationToken ct)
        {
            await _departmentService.AddEmployeeAsync(departmentId, dto.UserId, moveIfInAnotherDept, ct);
            return Ok(new { success = true });
        }


        /// <summary>
        /// Add multiple employees to a department.
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="dto"></param>
        /// <param name="ct"></param>
        /// <returns></returns>

        // إضافة مجموعة موظفين
        [HttpPost("{departmentId:int}/employees/bulk")]
        public async Task<IActionResult> AddEmployeesBulk(
            int departmentId,
            [FromBody] AddEmployeesToDepartmentDto dto,
            CancellationToken ct)
        {
            await _departmentService.AddEmployeesAsync(departmentId, dto, ct);
            return Ok(new { success = true, added = dto.UserIds.Distinct().Count() });
        }

        /// <summary>
        ///     Remove an employee from a department.
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="userId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpDelete("{departmentId:int}/employees/{userId:int}")]
        public async Task<IActionResult> RemoveEmployee(int departmentId, int userId, CancellationToken ct)
        {
            await _departmentService.RemoveEmployeeAsync(departmentId, userId, ct);
            return Ok(new { success = true });
        }


        /// <summary>
        /// Get all pending leave requests for HR approval.
        /// </summary>
        /// <returns></returns>
        [HttpGet("pending-requests")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var requests = await _hrService.HrPendingRequest();
            return Ok(requests);
        }

        /// <summary>
        /// Get count of all users in the system.
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-stats-count")]
        public async Task<IActionResult> GetStatsCount()
        {
            var count = await _userService.GetallUsersAsync();
            return Ok(new
            {
                count = count.Count,
            });
        }


        [HttpGet("hr-statistics")]
        public async Task<IActionResult> GetHRStatistics()
        {
            var stats = await _hrService.GetHRStatisticsAsync();
            return Ok(stats);
        }
    }
}
