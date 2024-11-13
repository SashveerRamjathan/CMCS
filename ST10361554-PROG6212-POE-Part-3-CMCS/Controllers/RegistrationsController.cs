using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models;
using ST10361554_PROG6212_POE_Part_3_CMCS.Data;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Controllers
{
    [Authorize(Roles = "Academic Manager, HR")]
    public class RegistrationsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegistrationsController> _logger;
        private readonly ApplicationDbContext _context;

        public RegistrationsController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<RegistrationsController> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _context = context;
        }

        [Authorize(Roles = "Academic Manager, HR")]
        [HttpGet]
        public async Task<IActionResult> GetAllPendingLecturers()
        {
            try
            {
                // Get all users in the 'Lecturer' role
                var lecturerUsers = await _userManager.GetUsersInRoleAsync("Lecturer");

                // Check if there are any users in the 'Lecturer' role
                if (lecturerUsers.Count == 0)
                {
                    _logger.LogInformation("No lecturers found in the system.");

                    // Display success or error message
                    ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View("ViewLecturerRegistrations", new List<ApplicationUser>());
                }

                // Filter users by those who are not approved
                List<ApplicationUser> pendingLecturers = lecturerUsers
                    .Where(u => !u.IsLecturerApproved)
                    .ToList();

                // Check if there are any pending lecturers
                if (pendingLecturers.Count == 0)
                {
                    _logger.LogInformation("No pending lecturers found in the system.");

                    // Display success or error message
                    ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View("ViewLecturerRegistrations", new List<ApplicationUser>());
                }

                _logger.LogInformation($"Found {pendingLecturers.Count} pending lecturers in the system.");

                // Display success or error message
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View("ViewLecturerRegistrations", pendingLecturers);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex.Message, "An error occurred while retrieving pending lecturers.");

                // Set an error message for the user
                TempData["ErrorMessage"] = "An error occurred while loading pending lecturers. Please try again later.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                // Return the view with an empty list
                return View("ViewLecturerRegistrations", new List<ApplicationUser>());
            }
        }

        [Authorize(Roles = "Academic Manager, HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveLecturerRegistration(string id)
        {
            try
            {
                // Check if the ID is null or empty
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogError("Lecturer ID was null or empty");
                    TempData["ErrorMessage"] = "Lecturer ID cannot be null or empty.";
                    return RedirectToAction(nameof(GetAllPendingLecturers));
                }

                // get the lecturer user by id
                var lecturerUser = await _userManager.FindByIdAsync(id);

                // Check if the lecturer user is null
                if (lecturerUser == null)
                {
                    _logger.LogError($"Lecturer user with ID {id} was not found");
                    TempData["ErrorMessage"] = "Lecturer user not found.";
                    return RedirectToAction(nameof(GetAllPendingLecturers));
                }

                // Check if the user is in the 'Lecturer' role
                if (!await _userManager.IsInRoleAsync(lecturerUser, "Lecturer"))
                {
                    _logger.LogError($"User with ID {id} is not in the 'Lecturer' role");
                    TempData["ErrorMessage"] = "User is not in the 'Lecturer' role.";
                    return RedirectToAction(nameof(GetAllPendingLecturers));
                }

                // Check if the user is already approved
                if (lecturerUser.IsLecturerApproved)
                {
                    _logger.LogError($"Lecturer user with ID {id} is already approved");
                    TempData["ErrorMessage"] = "Lecturer user is already approved.";
                    return RedirectToAction(nameof(GetAllPendingLecturers));
                }

                // approve the lecturer user
                lecturerUser.IsLecturerApproved = true;

                // update the user
                var result = await _userManager.UpdateAsync(lecturerUser);

                // check if the update was successful
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to approve lecturer user with ID {Id}. Errors: {Errors}", id, string.Join(", ", result.Errors.Select(e => e.Description)));
                    TempData["ErrorMessage"] = "Failed to approve lecturer user. Please try again.";
                    return RedirectToAction(nameof(GetAllPendingLecturers));
                }

                _logger.LogInformation($"Successfully approved lecturer {lecturerUser.FirstName} {lecturerUser.Surname}");

                TempData["SuccessMessage"] = $"Successfully approved lecturer {lecturerUser.FirstName} {lecturerUser.Surname}";

                return RedirectToAction(nameof(GetAllPendingLecturers));
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex.Message, $"An error occurred while approving lecturer registration for ID {id}");
                TempData["ErrorMessage"] = "An unexpected error occurred while processing your request. Please try again.";

                return RedirectToAction(nameof(GetAllPendingLecturers));
            }
        }

        [Authorize(Roles = "Academic Manager, HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectLecturerRegistration(string id)
        {
            try
            {
                // Check if the ID is null or empty
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogError("Lecturer ID was null or empty.");
                    TempData["ErrorMessage"] = "Lecturer ID cannot be null or empty.";
                    return RedirectToAction(nameof(GetAllPendingLecturers));
                }

                // get the lecturer user by id
                var lecturerUser = await _userManager.FindByIdAsync(id);

                // Check if the lecturer user is null
                if (lecturerUser == null)
                {
                    _logger.LogError($"Lecturer user with ID {id} was not found.");
                    TempData["ErrorMessage"] = "Lecturer user not found.";

                    return RedirectToAction(nameof(GetAllPendingLecturers));
                }

                // Check if the user is in the 'Lecturer' role
                if (!await _userManager.IsInRoleAsync(lecturerUser, "Lecturer"))
                {
                    _logger.LogError($"User with ID {id} is not in the 'Lecturer' role.");
                    TempData["ErrorMessage"] = "User is not in the 'Lecturer' role.";

                    return RedirectToAction(nameof(GetAllPendingLecturers));
                }

                // Check if the user is already approved
                if (lecturerUser.IsLecturerApproved)
                {
                    _logger.LogError($"Lecturer user with ID {id} is already approved.");
                    TempData["ErrorMessage"] = "Lecturer user is already approved.";

                    return RedirectToAction(nameof(GetAllPendingLecturers));
                }

                // delete the lecturer user
                var result = await _userManager.DeleteAsync(lecturerUser);

                // Check if the delete was successful
                if (!result.Succeeded)
                {
                    var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));

                    _logger.LogError($"Failed to reject lecturer user with ID {id}. Errors: {errorMessages}");

                    TempData["ErrorMessage"] = "Failed to reject lecturer user. Please try again.";

                    return RedirectToAction(nameof(GetAllPendingLecturers));
                }

                _logger.LogInformation($"Rejected lecturer registration for {lecturerUser.FirstName} {lecturerUser.Surname}");

                TempData["SuccessMessage"] = $"Rejected lecturer registration for {lecturerUser.FirstName} {lecturerUser.Surname}";

                return RedirectToAction(nameof(GetAllPendingLecturers));
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex.Message, $"An unexpected error occurred while rejecting lecturer registration for ID {id}.");
                TempData["ErrorMessage"] = "An unexpected error occurred while processing your request. Please try again.";

                return RedirectToAction(nameof(GetAllPendingLecturers));
            }
        }

        [Authorize(Roles = "HR")]
        [HttpGet]
        public async Task<IActionResult> GetAllPendingAcademicManagers()
        {
            try
            {
                // Get all users in the 'Academic Manager' role
                var academicManagerUsers = await _userManager.GetUsersInRoleAsync("Academic Manager");

                // check if there are any users in the 'Lecturer' role
                if (academicManagerUsers == null || academicManagerUsers.Count == 0)
                {
                    _logger.LogInformation("No academic managers found in the system.");
                    TempData["SuccessMessage"] = "No academic managers found.";

                    // Display success message
                    ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                    return View("ViewAcademicManagerRegistrations", new List<ApplicationUser>());
                }

                // Filter users by those who are not approved
                List<ApplicationUser> pendingManagers = academicManagerUsers.Where(u => !u.IsManagerApproved).ToList();

                // Check if there are any pending academic managers
                if (!pendingManagers.Any())
                {
                    _logger.LogInformation("No pending academic managers found in the system.");
                    TempData["SuccessMessage"] = "No pending academic managers found.";

                    ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                    return View("ViewAcademicManagerRegistrations", new List<ApplicationUser>());
                }

                _logger.LogInformation($"Found {pendingManagers.Count} pending academic managers in the system.");

                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View("ViewAcademicManagerRegistrations", pendingManagers);
            }
            catch (Exception ex)
            {
                // Log the exception with detailed information
                _logger.LogError(ex.Message, "An error occurred while retrieving pending academic managers.");
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View("ViewAcademicManagerRegistrations", new List<ApplicationUser>());
            }
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveAcademicManagerRegistration(string id)
        {
            try
            {
                // Check if the id is null or empty
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogError("Academic Manager ID was null or empty");
                    TempData["ErrorMessage"] = "Academic Manager ID was null or empty.";

                    return RedirectToAction(nameof(GetAllPendingAcademicManagers));
                }

                // Get the academic manager user by id
                var academicManagerUser = await _userManager.FindByIdAsync(id);

                // Check if the academic manager user is null
                if (academicManagerUser == null)
                {
                    _logger.LogError($"Academic manager user with ID {id} was not found");
                    TempData["ErrorMessage"] = "Academic Manager user was not found.";

                    return RedirectToAction(nameof(GetAllPendingAcademicManagers));
                }

                // Check if the user is in the 'Academic Manager' role
                if (!await _userManager.IsInRoleAsync(academicManagerUser, "Academic Manager"))
                {
                    _logger.LogError($"User with ID {id} is not in the 'Academic Manager' role");
                    TempData["ErrorMessage"] = "User is not in the 'Academic Manager' role.";

                    return RedirectToAction(nameof(GetAllPendingAcademicManagers));
                }

                // Check if the user is already approved
                if (academicManagerUser.IsManagerApproved)
                {
                    _logger.LogError($"Academic Manager user {academicManagerUser.FirstName} {academicManagerUser.Surname} is already approved");
                    TempData["ErrorMessage"] = "Academic Manager user is already approved.";

                    return RedirectToAction(nameof(GetAllPendingAcademicManagers));
                }

                // approve the lecturer user
                academicManagerUser.IsManagerApproved = true;

                // update the user
                var result = await _userManager.UpdateAsync(academicManagerUser);

                // Check if the update was successful
                if (!result.Succeeded)
                {
                    _logger.LogError($"Failed to approve academic manager user {academicManagerUser.FirstName} {academicManagerUser.Surname}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    TempData["ErrorMessage"] = "Failed to approve academic manager user.";

                    return RedirectToAction(nameof(GetAllPendingAcademicManagers));
                }

                _logger.LogInformation($"Successfully approved academic manager {academicManagerUser.FirstName} {academicManagerUser.Surname}");

                TempData["SuccessMessage"] = $"Successfully approved academic manager {academicManagerUser.FirstName} {academicManagerUser.Surname}";

                return RedirectToAction(nameof(GetAllPendingAcademicManagers));
            }
            catch (Exception ex)
            {
                // Log the exception with detailed information
                _logger.LogError(ex.Message, $"An error occurred while approving academic manager registration for ID {id}");
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";

                return RedirectToAction(nameof(GetAllPendingAcademicManagers));
            }
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectAcademicManagerRegistration(string id)
        {
            try
            {
                // Check if the id is null or empty
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogError("Academic Manager ID was null or empty");
                    TempData["ErrorMessage"] = "Academic Manager ID was null or empty.";

                    return RedirectToAction(nameof(GetAllPendingAcademicManagers));
                }

                // Get the academic manager user by id
                var academicManagerUser = await _userManager.FindByIdAsync(id);

                // Check if the academic manager user is null
                if (academicManagerUser == null)
                {
                    _logger.LogError($"Academic Manager user with ID {id} was not found");
                    TempData["ErrorMessage"] = "Academic Manager user was not found.";

                    return RedirectToAction(nameof(GetAllPendingAcademicManagers));
                }

                // Check if the user is in the 'Academic Manager' role
                if (!await _userManager.IsInRoleAsync(academicManagerUser, "Academic Manager"))
                {
                    _logger.LogError($"User with ID {id} is not in the 'Academic Manager' role");
                    TempData["ErrorMessage"] = "User is not in the 'Academic Manager' role.";

                    return RedirectToAction(nameof(GetAllPendingAcademicManagers));
                }

                // Check if the user is already approved
                if (academicManagerUser.IsManagerApproved)
                {
                    _logger.LogError($"Academic Manager user {academicManagerUser.FirstName} {academicManagerUser.Surname} is already approved");
                    TempData["ErrorMessage"] = "Academic Manager user is already approved.";

                    return RedirectToAction(nameof(GetAllPendingAcademicManagers));
                }

                // delete the lecturer user
                var result = await _userManager.DeleteAsync(academicManagerUser);

                // check if the delete was successful
                if (!result.Succeeded)
                {
                    _logger.LogError($"Failed to reject Academic Manager user {academicManagerUser.FirstName} {academicManagerUser.Surname}. " +
                        $"Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                    TempData["ErrorMessage"] = "Failed to reject Academic Manager user.";

                    return RedirectToAction(nameof(GetAllPendingAcademicManagers));
                }

                _logger.LogInformation($"Rejected Academic Manager registration for {academicManagerUser.FirstName} {academicManagerUser.Surname}");

                TempData["SuccessMessage"] = $"Rejected Academic Manager registration for {academicManagerUser.FirstName} {academicManagerUser.Surname}";

                return RedirectToAction(nameof(GetAllPendingAcademicManagers));
            }
            catch (Exception ex)
            {
                // Log the exception with detailed information
                _logger.LogError(ex.Message, $"An error occurred while rejecting academic manager registration for ID {id}");
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";

                return RedirectToAction(nameof(GetAllPendingAcademicManagers));
            }
        }
    }
}
