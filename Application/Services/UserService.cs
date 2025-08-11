// using LeaveRequestSystem.Domain.Repositories;
// using LeaveRequestSystem.Application.DTOs;
// using LeaveRequestSystem.Domain.Enums;
// using LeaveRequestSystem.Domain.Entities;

// namespace LeaveRequestSystem.Application.Services
// {
//     public class UserService
//     {
//         private readonly IUserRepository _userRepository;

//         public UserService(IUserRepository userRepository)
//         {
//             _userRepository = userRepository;
//         }

//         // Get all users in the system
//         public async Task<List<UserDto>> GetAllUsersAsync()
//         {
//             var users = await _userRepository.GetUsers();

//             return users.Select(u => new UserDto
//             {
//                 Id = u.Id,
//                 Username = u.Username,
//                 Name = u.Name,
//                 Email = u.Email ?? "",
//                 Department = u.Department,
//                 Role = u.Role,
//                 IsActive = u.IsActive
//             }).ToList();
//         }

//         // Get user by ID
//         public async Task<UserDto?> GetUserByIdAsync(int userId)
//         {
//             var user = await _userRepository.GetByIdAsync(userId);
//             if (user == null) return null;

//             return new UserDto
//             {
//                 Id = user.Id,
//                 Username = user.Username,
//                 Name = user.Name,
//                 Email = user.Email ?? "",
//                 Department = user.Department,
//                 Role = user.Role,
//                 IsActive = user.IsActive
//             };
//         }

//         // Get all users with detailed information including manager info
//         public async Task<List<UserDetailedDto>> GetAllUsersDetailedAsync()
//         {
//             var users = await _userRepository.GetUsers();
//             var userDetailsList = new List<UserDetailedDto>();

//             foreach (var user in users)
//             {
//                 var manager = user.ManagerId.HasValue
//                     ? users.FirstOrDefault(u => u.Id == user.ManagerId.Value)
//                     : null;

//                 var teamCount = user.Role == Role.MANAGER
//                     ? users.Count(u => u.ManagerId == user.Id)
//                     : 0;

//                 userDetailsList.Add(new UserDetailedDto
//                 {
//                     Id = user.Id,
//                     Username = user.Username,
//                     Name = user.Name,
//                     Email = user.Email ?? "",
//                     Department = user.Department,
//                     Role = user.Role.ToString(),
//                     IsActive = user.IsActive,
//                     CreatedAt = user.CreatedAt,
//                     ManagerId = user.ManagerId,
//                     ManagerName = manager?.Name,
//                     TeamMembersCount = teamCount
//                 });
//             }

//             return userDetailsList;
//         }

//         // Get users by role
//         public async Task<List<UserDto>> GetUsersByRoleAsync(Role role)
//         {
//             var users = await _userRepository.GetUsers();
//             var filteredUsers = users.Where(u => u.Role == role && u.IsActive).ToList();

//             return filteredUsers.Select(u => new UserDto
//             {
//                 Id = u.Id,
//                 Username = u.Username,
//                 Name = u.Name,
//                 Email = u.Email ?? "",
//                 Department = u.Department,
//                 Role = u.Role,
//                 IsActive = u.IsActive
//             }).ToList();
//         }

//         // Get users by department
//         public async Task<List<UserDto>> GetUsersByDepartmentAsync(string department)
//         {
//             var users = await _userRepository.GetUsers();
//             var filteredUsers = users
//                 .Where(u => u.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
//                 .ToList();

//             return filteredUsers.Select(u => new UserDto
//             {
//                 Id = u.Id,
//                 Username = u.Username,
//                 Name = u.Name,
//                 Email = u.Email ?? "",
//                 Department = u.Department,
//                 Role = u.Role,
//                 IsActive = u.IsActive
//             }).ToList();
//         }

//         // Get all managers
//         public async Task<List<ManagerDto>> GetAllManagersAsync()
//         {
//             var users = await _userRepository.GetUsers();
//             var managers = users.Where(u => u.Role == Role.MANAGER && u.IsActive).ToList();

//             var managerDtos = new List<ManagerDto>();

//             foreach (var manager in managers)
//             {
//                 var teamCount = users.Count(u => u.ManagerId == manager.Id);
//                 managerDtos.Add(new ManagerDto
//                 {
//                     Id = manager.Id,
//                     Username = manager.Username,
//                     Name = manager.Name,
//                     Email = manager.Email ?? "",
//                     Department = manager.Department,
//                     TeamMembersCount = teamCount
//                 });
//             }

//             return managerDtos;
//         }

//         // Get employees without manager
//         public async Task<List<EmployeeDto>> GetEmployeesWithoutManagerAsync()
//         {
//             var users = await _userRepository.GetUsers();
//             var employeesWithoutManager = users
//                 .Where(u => u.Role == Role.EMPLOYEE && u.ManagerId == null && u.IsActive)
//                 .ToList();

//             return employeesWithoutManager.Select(e => new EmployeeDto
//             {
//                 Id = e.Id,
//                 Username = e.Username,
//                 Name = e.Name,
//                 Email = e.Email ?? "",
//                 Department = e.Department,
//                 ManagerId = null,
//                 ManagerName = null
//             }).ToList();
//         }

//         // Get all employees with their managers
//         public async Task<List<EmployeeDto>> GetAllEmployeesWithManagersAsync()
//         {
//             var users = await _userRepository.GetUsers();
//             var employees = users.Where(u => u.Role == Role.EMPLOYEE && u.IsActive).ToList();

//             var employeeDtos = new List<EmployeeDto>();

//             foreach (var employee in employees)
//             {
//                 var manager = employee.ManagerId.HasValue
//                     ? users.FirstOrDefault(u => u.Id == employee.ManagerId.Value)
//                     : null;

//                 employeeDtos.Add(new EmployeeDto
//                 {
//                     Id = employee.Id,
//                     Username = employee.Username,
//                     Name = employee.Name,
//                     Email = employee.Email ?? "",
//                     Department = employee.Department,
//                     ManagerId = employee.ManagerId,
//                     ManagerName = manager?.Name
//                 });
//             }

//             return employeeDtos;
//         }

//         // Get team members for a manager
//         public async Task<List<EmployeeDto>> GetTeamMembersAsync(int managerId)
//         {
//             var users = await _userRepository.GetUsers();
//             var manager = users.FirstOrDefault(u => u.Id == managerId && u.Role == Role.MANAGER);

//             if (manager == null)
//                 throw new Exception("Manager not found!");

//             var teamMembers = users.Where(u => u.ManagerId == managerId && u.IsActive).ToList();

//             return teamMembers.Select(m => new EmployeeDto
//             {
//                 Id = m.Id,
//                 Username = m.Username,
//                 Name = m.Name,
//                 Email = m.Email ?? "",
//                 Department = m.Department,
//                 ManagerId = managerId,
//                 ManagerName = manager.Name
//             }).ToList();
//         }

//         // Assign manager to employee
//         public async Task<bool> AssignManagerToEmployeeAsync(int employeeId, int managerId)
//         {
//             var employee = await _userRepository.GetByIdAsync(employeeId);
//             var manager = await _userRepository.GetByIdAsync(managerId);

//             if (employee == null)
//                 throw new Exception($"Employee with ID {employeeId} not found!");

//             if (manager == null)
//                 throw new Exception($"Manager with ID {managerId} not found!");

//             if (manager.Role != Role.MANAGER)
//                 throw new Exception($"User {manager.Name} is not a manager!");

//             if (employee.Role != Role.EMPLOYEE)
//                 throw new Exception($"User {employee.Name} is not an employee!");

//             employee.ManagerId = managerId;
//             await _userRepository.UpdateAsync(employee);

//             return true;
//         }

//         // Remove manager from employee
//         public async Task<bool> RemoveManagerFromEmployeeAsync(int employeeId)
//         {
//             var employee = await _userRepository.GetByIdAsync(employeeId);

//             if (employee == null)
//                 throw new Exception($"Employee with ID {employeeId} not found!");

//             employee.ManagerId = null;
//             await _userRepository.UpdateAsync(employee);

//             return true;
//         }

//         // Activate/Deactivate user
//         public async Task<bool> SetUserStatusAsync(int userId, bool isActive)
//         {
//             var user = await _userRepository.GetByIdAsync(userId);
//             if (user == null)
//                 throw new Exception($"User with ID {userId} not found!");

//             user.IsActive = isActive;
//             await _userRepository.UpdateAsync(user);

//             return true;
//         }

//         // Get user statistics
//         public async Task<UserStatisticsDto> GetUserStatisticsAsync()
//         {
//             var users = await _userRepository.GetUsers();

//             return new UserStatisticsDto
//             {
//                 TotalUsers = users.Count,
//                 ActiveUsers = users.Count(u => u.IsActive),
//                 InactiveUsers = users.Count(u => !u.IsActive),
//                 TotalEmployees = users.Count(u => u.Role == Role.EMPLOYEE),
//                 TotalManagers = users.Count(u => u.Role == Role.MANAGER),
//                 TotalHR = users.Count(u => u.Role == Role.HR),
//                 EmployeesWithoutManager = users.Count(u => u.Role == Role.EMPLOYEE && u.ManagerId == null)
//             };
//         }
//     }
// }