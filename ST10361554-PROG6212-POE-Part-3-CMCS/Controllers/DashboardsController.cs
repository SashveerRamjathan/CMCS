using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models;
using ST10361554_PROG6212_POE_Part_3_CMCS.ViewModels;
using ST10361554_PROG6212_POE_Part_3_CMCS.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Controllers
{
    [Authorize]
    public class DashboardsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DashboardsController> _logger;

        public DashboardsController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            ILogger<DashboardsController> logger)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> GetLecturerDashboard()
        {
            try
            {
                // Get the current users ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Check if the user ID is null or empty
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("GetLecturerDashboard: User ID is null or empty.");

                    return View("LecturerDashboard", new LecturerUserViewModel());  // Return empty model
                }

                // Set the user ID in ViewData for the view
                ViewData["UserId"] = userId;

                // Get the current user
                var user = await _userManager.FindByIdAsync(userId);

                // If user is not found, log the error and return empty model
                if (user == null)
                {
                    _logger.LogError($"GetLecturerDashboard: User with ID {userId} not found.");

                    return View("LecturerDashboard", new LecturerUserViewModel());  // Return empty model
                }

                // Create the LecturerUserViewModel
                var lecturerUser = new LecturerUserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName!,
                    Surname = user.Surname!,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber!,
                    Faculty = user.Faculty!,
                    Module = user.Module!
                };

                _logger.LogInformation($"GetLecturerDashboard: Successfully retrieved dashboard for user {userId}.");

                return View("LecturerDashboard", lecturerUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "GetLecturerDashboard: An error occurred while retrieving lecturer dashboard.");

                return View("LecturerDashboard", new LecturerUserViewModel());  // Return empty model in case of an error
            }
        }


        [HttpGet]
        [Authorize(Roles = "Academic Manager")]
        public async Task<IActionResult> GetAcademicManagerDashboard()
        {
            try
            {
                // Get the current users ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Check if the user ID is null or empty
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("GetAcademicManagerDashboard: User ID is null or empty.");

                    return View("LecturerDashboard", new UpdateAcademicManagerViewModel());  // Return empty model
                }

                // set the view data to hold the user id
                ViewData["UserId"] = userId;

                // Get the current user
                var user = await _userManager.FindByIdAsync(userId);

                // Check if the user is null
                if (user == null)
                {
                    _logger.LogWarning($"GetAcademicManagerDashboard: User with ID {userId} not found.");

                    // Return a default model to avoid null reference in the view
                    return View("AcademicManagerDashboard", new UpdateAcademicManagerViewModel());
                }

                // Create the UpdateAcademicManagerViewModel
                var academicManagerUser = new UpdateAcademicManagerViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName ?? "N/A", // Default value if null
                    Surname = user.Surname ?? "N/A",     // Default value if null
                    Email = user.Email ?? "N/A",         // Default value if null
                    PhoneNumber = user.PhoneNumber ?? "N/A", // Default value if null
                    StreetAddress = user.StreetAddress ?? "N/A", // Default value if null
                    AreaAddress = user.AreaAddress ?? "N/A",     // Default value if null
                    City = user.City ?? "N/A",                 // Default value if null
                    Province = user.Province ?? "N/A"           // Default value if null
                };

                // Get the number of total claims
                var totalClaims = await _context.Claim.CountAsync();
                ViewData["TotalClaims"] = totalClaims;

                // Get the number of approved lecturers
                var totalLecturers = await _context.Users.CountAsync(u => u.IsLecturerApproved);
                ViewData["TotalLecturers"] = totalLecturers;

                // Get the number of pending lecturers
                var lecturerUsers = await _userManager.GetUsersInRoleAsync("Lecturer");
                var pendingLecturers = lecturerUsers.Count(u => !u.IsLecturerApproved);
                ViewData["PendingLecturers"] = pendingLecturers;

                return View("AcademicManagerDashboard", academicManagerUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "GetAcademicManagerDashboard: An error occurred while retrieving the academic manager dashboard.");

                // Return a default model to avoid null reference in the view
                return View("AcademicManagerDashboard", new UpdateAcademicManagerViewModel());
            }
        }

        [HttpGet]
        [Authorize(Roles = "HR")]
        public IActionResult GetHRDashboard()
        {
            try
            {
                // get the number of total claims
                var totalClaims = _context.Claim.Count();
                ViewData["TotalClaims"] = totalClaims;

                // get the number of approved claims
                var approvedClaims = _context.Claim.Where(c => c.Status == "Approved").Count();
                ViewData["ApprovedClaims"] = approvedClaims;

                // get the number of rejected claims
                var rejectedClaims = _context.Claim.Where(c => c.Status == "Rejected").Count();
                ViewData["RejectedClaims"] = rejectedClaims;

                // Log successful retrieval of dashboard data
                _logger.LogInformation("HR dashboard data retrieved successfully.");

                return View("HRDashboard");
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex.Message, "An error occurred while retrieving the HR dashboard.");

                return View("HRDashboard");
            }
        }
    }
}
