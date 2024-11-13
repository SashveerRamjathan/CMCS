﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models;
using ST10361554_PROG6212_POE_Part_3_CMCS.ViewModels;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Controllers
{
    [Authorize]
    public class StaffController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<StaffController> _logger;

        public StaffController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<StaffController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Academic Manager")]
        public async Task<IActionResult> ViewAllLecturers(string? faculty)
        {

            List<LecturerUserViewModel> lecturers = new List<LecturerUserViewModel>();

            try
            {
                // check if the lecturer role exists
                var lecturerRole = await _roleManager.FindByNameAsync("Lecturer");

                if (lecturerRole == null)
                {
                    _logger.LogError("Lecturer role not found");
                    TempData["ErrorMessage"] = "Lecturer role not found.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View(lecturers);
                }

                // get all users in the lecturer role
                var lecturerUsers = await _userManager.GetUsersInRoleAsync("Lecturer");

                if (lecturerUsers == null || !lecturerUsers.Any())
                {
                    _logger.LogError("No lecturers found");
                    TempData["ErrorMessage"] = "No lecturers found.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View(lecturers);
                }

                // Filter the lecturer users to only include approved lecturers
                var approvedLecturerUsers = lecturerUsers.Where(u => u.IsLecturerApproved == true).ToList();

                foreach (var user in approvedLecturerUsers)
                {
                    var lecturerUser = new LecturerUserViewModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName!,
                        Surname = user.Surname!,
                        Email = user.Email!,
                        PhoneNumber = user.PhoneNumber!,
                        Faculty = user.Faculty!,
                        Module = user.Module!,
                        AccountNumber = user.AccountNumber!,
                        BankName = user.BankName!,
                        BranchCode = user.BranchCode!,
                        Address = $"{user.StreetAddress}, {user.AreaAddress}, {user.City}, {user.Province}"
                    };

                    lecturers.Add(lecturerUser);
                }

                // Get a list of all the faculties
                var faculties = lecturers.Select(l => l.Faculty).Distinct();

                if (!faculties.Any())
                {
                    _logger.LogError("No faculties found");
                    TempData["ErrorMessage"] = "No faculties found.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View(lecturers);
                }

                _logger.LogInformation("Faculties found");

                // Pass the list of faculties to the view
                ViewBag.Faculties = faculties;

                // Filter by faculty if provided
                if (string.IsNullOrEmpty(faculty))
                {
                    _logger.LogInformation("Showing lecturers for all faculties");
                    TempData["SuccessMessage"] = "Showing lecturers for all faculties.";

                    // Display success message
                    ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                    return View(lecturers);
                }

                lecturers = lecturers.Where(l => l.Faculty == faculty).ToList();

                if (!lecturers.Any())
                {
                    _logger.LogWarning("No lecturers found for faculty {Faculty}", faculty);
                    TempData["ErrorMessage"] = $"No lecturers found for faculty {faculty}.";
                }
                else
                {
                    _logger.LogInformation($"Showing lecturers for faculty {faculty}");
                    TempData["SuccessMessage"] = $"Showing lecturers for faculty {faculty}.";
                }

                // Display success or error message
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View(lecturers);
            }
            catch (Exception ex)
            {
                // Log the exception with detailed information
                _logger.LogError(ex.Message, $"An error occurred while retrieving lecturers for faculty {faculty}");
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View(lecturers);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Academic Manager, HR")]
        public async Task<IActionResult> UpdateLecturerDetailsByManager(string id)
        {
            // Validate the ID
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError("Lecturer ID is null or empty");
                TempData["ErrorMessage"] = "Invalid lecturer ID provided.";

                return RedirectToAction(nameof(ViewAllLecturers));
            }

            try
            {
                // get the lecturer user by id
                var lecturerUser = await _userManager.FindByIdAsync(id);

                // Check if the lecturer user is null
                if (lecturerUser == null)
                {
                    _logger.LogError($"Lecturer not found with ID: {id}");
                    TempData["ErrorMessage"] = "Lecturer not found.";

                    return RedirectToAction(nameof(ViewAllLecturers));
                }

                var updateLecturerUser = new UpdateLecturerUserViewModel
                {
                    Id = lecturerUser.Id,
                    FirstName = lecturerUser.FirstName!,
                    Surname = lecturerUser.Surname!,
                    Email = lecturerUser.Email!,
                    PhoneNumber = lecturerUser.PhoneNumber!,
                    Faculty = lecturerUser.Faculty!,
                    Module = lecturerUser.Module!,
                    AccountNumber = lecturerUser.AccountNumber!,
                    BankName = lecturerUser.BankName!,
                    BranchCode = lecturerUser.BranchCode!,
                    StreetAddress = lecturerUser.StreetAddress!,
                    AreaAddress = lecturerUser.AreaAddress!,
                    City = lecturerUser.City!,
                    Province = lecturerUser.Province!
                };

                _logger.LogInformation($"Lecturer found with ID: {id}");

                return View("UpdateLecturerDetails", updateLecturerUser);
            }
            catch (Exception ex)
            {
                // Log the exception and inform the user
                _logger.LogError(ex.Message, $"An error occurred while updating lecturer details for ID: {id}");
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";

                return RedirectToAction(nameof(ViewAllLecturers));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Academic Manager, HR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLecturerDetailsByManager(UpdateLecturerUserViewModel model)
        {
            try
            {
                // Check if the model state is valid
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    _logger.LogError($"Invalid lecturer details: {string.Join(", ", errors.Select(e => e.ErrorMessage))}");
                    TempData["ErrorMessage"] = "Invalid lecturer details. Please correct the errors and try again.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View("UpdateLecturerDetails", model);
                }

                // get the lecturer user by id
                var lecturerUser = await _userManager.FindByIdAsync(model.Id!);

                // Check if the lecturer user is null
                if (lecturerUser == null)
                {
                    _logger.LogError("Lecturer not found with ID: {LecturerId}", model.Id);
                    TempData["ErrorMessage"] = "Lecturer not found.";

                    return RedirectToAction(nameof(ViewAllLecturers));
                }

                // update the lecturer user properties
                lecturerUser.FirstName = model.FirstName;
                lecturerUser.Surname = model.Surname;
                lecturerUser.UserName = model.Email;
                lecturerUser.Email = model.Email;
                lecturerUser.PhoneNumber = model.PhoneNumber;
                lecturerUser.Faculty = model.Faculty;
                lecturerUser.Module = model.Module;
                lecturerUser.AccountNumber = model.AccountNumber;
                lecturerUser.BankName = model.BankName;
                lecturerUser.BranchCode = model.BranchCode;
                lecturerUser.StreetAddress = model.StreetAddress;
                lecturerUser.AreaAddress = model.AreaAddress;
                lecturerUser.City = model.City;
                lecturerUser.Province = model.Province;

                // update the lecturer user
                var result = await _userManager.UpdateAsync(lecturerUser);

                // Check if the update was successful
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Lecturer details updated successfully for lecturer: {model.Id}");
                    TempData["SuccessMessage"] = "Lecturer details updated successfully.";

                    return User.IsInRole("HR") ? RedirectToAction(nameof(ViewAllStaff)) : RedirectToAction(nameof(ViewAllLecturers));
                }

                // Log each error that occurred during update
                foreach (var error in result.Errors)
                {
                    _logger.LogError($"Error updating lecturer details: {error.Description}");
                }

                TempData["ErrorMessage"] = "Failed to update lecturer details. Please try again later.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View("UpdateLecturerDetails", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"An unexpected error occurred while updating lecturer details for ID: {model.Id}");
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View("UpdateLecturerDetails", model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Lecturer")]
        public async Task<IActionResult> UpdateLecturerPersonalDetails(string id)
        {
            try
            {
                // get the lecturer user by id
                var lecturerUser = await _userManager.FindByIdAsync(id);

                // Check if the lecturer user is null
                if (lecturerUser == null)
                {
                    _logger.LogError($"Lecturer not found with ID: {id}");

                    return RedirectToAction("GetLecturerDashboard", "Dashboards");
                }

                var updateLecturerUser = new UpdateLecturerUserViewModel
                {
                    Id = lecturerUser.Id,
                    FirstName = lecturerUser.FirstName!,
                    Surname = lecturerUser.Surname!,
                    Email = lecturerUser.Email!,
                    PhoneNumber = lecturerUser.PhoneNumber!,
                    Faculty = lecturerUser.Faculty!,
                    Module = lecturerUser.Module!,
                };

                _logger.LogInformation($"Lecturer found with ID: {id}");

                return View(updateLecturerUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"An error occurred while trying to get lecturer details for ID: {id}");

                return RedirectToAction("GetLecturerDashboard", "Dashboards");
            }
        }

        // Helper method to validate the lecturer model
        private List<string> ValidateLecturerModel(UpdateLecturerUserViewModel model)
        {
            List<string> errors = new List<string>();

            // check if only the editable properties are valid
            if (string.IsNullOrEmpty(model.FirstName))
            {
                errors.Add("First name is required");
            }

            if (string.IsNullOrEmpty(model.Surname))
            {
                errors.Add("Surname is required");
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                errors.Add("Email is required");
            }

            if (string.IsNullOrEmpty(model.PhoneNumber))
            {
                errors.Add("Phone number is required");
            }

            if (string.IsNullOrEmpty(model.Faculty))
            {
                errors.Add("Faculty is required");
            }

            if (string.IsNullOrEmpty(model.Module))
            {
                errors.Add("Module is required");
            }

            return errors;
        }

        [HttpPost]
        [Authorize(Roles = "Lecturer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLecturerPersonalDetails(UpdateLecturerUserViewModel model)
        {
            // Validate the model
            var errors = ValidateLecturerModel(model);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    TempData["ErrorMessage"] += error + "\n";
                }

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View(model);
            }

            try
            {
                // get the lecturer user by id
                var lecturerUser = await _userManager.FindByIdAsync(model.Id!);

                // Check if the lecturer user is null
                if (lecturerUser == null)
                {
                    _logger.LogError($"Lecturer not found with ID: {model.Id}");

                    TempData["ErrorMessage"] = "Lecturer not found.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View(model);
                }

                // update the lecturer user properties
                lecturerUser.FirstName = model.FirstName;
                lecturerUser.Surname = model.Surname;
                lecturerUser.UserName = model.Email;
                lecturerUser.Email = model.Email;
                lecturerUser.PhoneNumber = model.PhoneNumber;
                lecturerUser.Faculty = model.Faculty;
                lecturerUser.Module = model.Module;

                // update the lecturer user
                var result = await _userManager.UpdateAsync(lecturerUser);

                // check if the update was successful
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Lecturer details updated successfully for lecturer: {model.Id}");
                    TempData["SuccessMessage"] = $"Lecturer details updated successfully";

                    // Display success message
                    ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                    return RedirectToAction("UpdateLecturerPersonalDetails", "Staff", new { id = model.Id });
                }

                // Log any errors returned by the result
                foreach (var error in result.Errors)
                {
                    _logger.LogError($"Error updating lecturer: {error.Description}");
                }

                TempData["ErrorMessage"] = $"Failed to update lecturer details. Please try again later.";

                // Display success message
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"An unexpected error occurred while updating lecturer details for ID: {model.Id}");
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Academic Manager, HR")]
        public async Task<IActionResult> UpdateAcademicManagerDetails(string id)
        {
            try
            {
                // get the Academic Manager user by id
                var managerUser = await _userManager.FindByIdAsync(id);

                // check if the Academic Manager user is null
                if (managerUser == null)
                {
                    _logger.LogError($"Academic Manager not found with ID: {id}");
                    TempData["ErrorMessage"] = $"Academic Manager not found";

                    return User.IsInRole("Academic Manager") ? RedirectToAction("GetAcademicManagerDashboard", "Dashboards")
                        : RedirectToAction("ViewAllStaff", "Staff");
                }

                _logger.LogInformation($"Academic Manager found with ID: {managerUser.Id}");

                UpdateAcademicManagerViewModel viewModel = new UpdateAcademicManagerViewModel
                {
                    Id = managerUser.Id,
                    FirstName = managerUser.FirstName!,
                    Surname = managerUser.Surname!,
                    Email = managerUser.Email!,
                    PhoneNumber = managerUser.PhoneNumber!,
                    StreetAddress = managerUser.StreetAddress!,
                    AreaAddress = managerUser.AreaAddress!,
                    City = managerUser.City!,
                    Province = managerUser.Province!
                };

                // Display success or error message
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex.Message, $"An error occurred while retrieving the Academic Manager with ID: {id}");

                // Display a generic error message to the user
                TempData["ErrorMessage"] = "An error occurred while retrieving the Academic Manager details. Please try again later.";

                // Redirect to appropriate action based on user role
                return User.IsInRole("Academic Manager") ? RedirectToAction("GetAcademicManagerDashboard", "Dashboards")
                        : RedirectToAction("ViewAllStaff", "Staff");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Academic Manager, HR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAcademicManagerDetails(UpdateAcademicManagerViewModel model)
        {
            try
            {
                // check if the model state is valid
                if (!ModelState.IsValid)
                {
                    _logger.LogError($"Invalid model state for Academic Manager with ID: {model.Id}");
                    TempData["ErrorMessage"] = "Invalid Academic Manager details. Please correct the errors.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View("UpdateAcademicManagerDetails", model);
                }

                // get the lecturer user by id
                var managerUser = await _userManager.FindByIdAsync(model.Id!);

                // check if the lecturer user is null
                if (managerUser == null)
                {
                    _logger.LogError($"Academic Manager not found with ID: {model.Id}");
                    TempData["ErrorMessage"] = $"Academic Manager not found";

                    return User.IsInRole("Academic Manager") ? RedirectToAction("GetAcademicManagerDashboard", "Dashboards")
                        : RedirectToAction("ViewAllStaff", "Staff");
                }

                // update the lecturer user properties
                managerUser.FirstName = model.FirstName;
                managerUser.Surname = model.Surname;
                managerUser.UserName = model.Email;
                managerUser.Email = model.Email;
                managerUser.PhoneNumber = model.PhoneNumber;
                managerUser.StreetAddress = model.StreetAddress;
                managerUser.AreaAddress = model.AreaAddress;
                managerUser.City = model.City;
                managerUser.Province = model.Province;

                // update the lecturer user
                var result = await _userManager.UpdateAsync(managerUser);

                // check if the update was successful
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Academic Manager details updated successfully for Academic Manager: {model.Id}");
                    TempData["SuccessMessage"] = $"Academic Manager details updated successfully";

                    return User.IsInRole("Academic Manager") ? RedirectToAction(nameof(UpdateAcademicManagerDetails), new { id = model.Id })
                        : RedirectToAction("ViewAllStaff", "Staff");
                }
                else
                {
                    // Log specific errors from the IdentityResult
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError($"Error updating Academic Manager: {error.Description}");
                    }
                }

                TempData["ErrorMessage"] = $"Failed to update Academic Manager details. Please try again later.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View("UpdateAcademicManagerDetails", model);
            }
            catch (Exception ex)
            {
                // Catch any unhandled exceptions
                _logger.LogError(ex.Message, $"An unexpected error occurred while updating Academic Manager with ID: {model.Id}");
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View("UpdateAcademicManagerDetails", model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> ViewAllStaff()
        {
            try
            {
                // Check if both Lecturer and Academic Manager roles exist
                var lecturerRoleExists = await _roleManager.RoleExistsAsync("Lecturer");
                var managerRoleExists = await _roleManager.RoleExistsAsync("Academic Manager");

                if (!lecturerRoleExists || !managerRoleExists)
                {
                    string missingRoles = !lecturerRoleExists ? "Lecturer" : "Academic Manager";

                    _logger.LogError($"Role '{missingRoles}' does not exist");
                    TempData["ErrorMessage"] = $"Role '{missingRoles}' not found.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View(new List<ApplicationUser>());  // Return an empty list if roles are missing
                }

                // Get users in both roles
                var lecturerUsers = await _userManager.GetUsersInRoleAsync("Lecturer");
                var managerUsers = await _userManager.GetUsersInRoleAsync("Academic Manager");

                // Check if no users found in either role
                if (!lecturerUsers.Any() && !managerUsers.Any())
                {
                    TempData["ErrorMessage"] = "No staff found in either 'Lecturer' or 'Academic Manager' roles.";

                    // Display error message
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View(new List<ApplicationUser>());
                }

                // Combine lecturer and manager users into a single list
                List<ApplicationUser> allStaff = lecturerUsers.Concat(managerUsers).ToList();

                // only show approved staff
                allStaff = allStaff.Where(u => u.IsLecturerApproved == true || u.IsManagerApproved == true).ToList();

                // If no approved staff
                if (!allStaff.Any())
                {
                    _logger.LogInformation("No approved staff found.");
                    TempData["ErrorMessage"] = "No approved staff found in either role.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View(new List<ApplicationUser>());
                }

                // Display success or error message
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                return View(allStaff); // Pass the combined and filtered list to the view
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "An error occurred while retrieving staff.");
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving staff.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View(new List<ApplicationUser>());
            }
        }
    }
}
