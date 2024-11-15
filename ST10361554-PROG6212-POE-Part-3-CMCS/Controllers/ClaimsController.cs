using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models;
using ST10361554_PROG6212_POE_Part_3_CMCS.ViewModels;
using ST10361554_PROG6212_POE_Part_3_CMCS.Data;
using System.Security.Claims;
using ST10361554_PROG6212_POE_Part_3_CMCS.FluentValidators;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClaimsController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ClaimsController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            ILogger<ClaimsController> logger,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = dbContext;
            _logger = logger;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Lecturer")]
        [HttpGet]
        public async Task<IActionResult> SubmitClaim()
        {
            try
            {
                // get the current users id
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // check if the user id is null or empty
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("User ID is null. Cannot proceed with submitting the claim.");
                    TempData["ErrorMessage"] = "Your session has expired. Please log in again.";
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                // get the current user based on the user id
                var user = await _userManager.FindByIdAsync(userId);

                // check if user is null
                if (user == null)
                {
                    _logger.LogError("No user found for ID {UserId}.", userId);
                    return RedirectToAction("GetLecturerDashboard", "Dashboards");
                }

                // check the hourly rate of the user
                if (user.HourlyRate == null)
                {
                    _logger.LogError("Hourly rate not set for user {UserId}.", userId);
                    TempData["ErrorMessage"] = "Hourly rate not set. Please contact your administrator.";
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                    return RedirectToAction("GetLecturerDashboard", "Dashboards");
                }

                // pass the hourly rate to the view
                SubmitClaimViewModel model = new SubmitClaimViewModel
                {
                    HourlyRate = (decimal)user.HourlyRate
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // log the error and return an error message
                _logger.LogError(ex.Message, $"An error occurred while fetching claim form for user {User.FindFirstValue(ClaimTypes.NameIdentifier)}.");
                TempData["ErrorMessage"] = "An error occurred while fetching the claim form. Please try again later.";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                return RedirectToAction("GetLecturerDashboard", "Dashboards");
            }
        }

        [Authorize(Roles = "Lecturer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(SubmitClaimViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid claim model state for user {User.FindFirstValue(ClaimTypes.NameIdentifier)}.");
                TempData["ErrorMessage"] = "Please correct the errors in the form.";

                // Collect validation errors and pass them to the view
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                ViewData["ValidationErrors"] = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return View(model);
            }

            // get the current users id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // check if the user id is null or empty
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogError("User ID is null. Cannot proceed with submitting the claim.");
                TempData["ErrorMessage"] = "Your session has expired. Please log in again.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // get the current user based on the user id
            var user = await _userManager.FindByIdAsync(userId);

            // check if user is null
            if (user == null)
            {
                _logger.LogError("No user found for ID {UserId}.", userId);
                TempData["ErrorMessage"] = "User not found. Please log in again.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            try
            {
                // fill in general claim properties
                _logger.LogInformation($"Creating new claim for user {userId}.");

                Models.Claim claim = new Models.Claim()
                {
                    Id = model.Id,
                    ClaimName = model.ClaimName,
                    ClaimDate = model.ClaimDate,
                    ClaimDescription = model.ClaimDescription,
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    FinalAmount = model.FinalAmount,
                    Status = model.Status,
                    User = user,
                    DocumentName = model.Document.FileName,
                    DocumentType = model.Document.ContentType
                };

                _logger.LogInformation("Adding document to claim.");

                if (model.Document == null || model.Document.Length == 0)
                {
                    _logger.LogError($"No document uploaded for claim {model.ClaimName}.");
                    TempData["ErrorMessage"] = "Document upload failed. Please ensure a valid file is attached.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View(model);
                }

                // Code Attribution:
                // Reading and Writing Files in ASP.NET Core from memory streams to byte arrays
                // DA_VDCT
                // 15 October 2024
                // https://stackoverflow.com/questions/60001470/c-sharp-problem-reading-file-memorystream-in-my-mvc-project

                // Handle the document
                using (var memoryStream = new MemoryStream())
                {
                    await model.Document.CopyToAsync(memoryStream);
                    claim.Document = memoryStream.ToArray();
                }

                // Ensure user claims are initialized
                if (user.Claims == null)
                {
                    user.Claims = new List<Models.Claim>();
                }

                // Code Attribution:
                // Creating your first validator
                // Fluentvalidation.net
                // 14 November 2024
                // https://docs.fluentvalidation.net/en/latest/start.html

                // validate the claim model
                var claimValidator = new ClaimValidator();
                var validationResult = claimValidator.Validate(claim);

                // check if the claim is valid
                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError("", error.ErrorMessage);
                    }

                    TempData["ErrorMessage"] = "Please correct the errors in the form.";
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    ViewData["ValidationErrors"] = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return View(model);
                }

                // add the claim to the user
                user.Claims.Add(claim);

                // add the claim to the database
                _context.Add(claim);

                // save the changes to the database
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Claim {claim.ClaimName} saved to the database for user {userId}.");

                TempData["SuccessMessage"] = $"Claim '{claim.ClaimName}' submitted successfully.";

                ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                return RedirectToAction(nameof(TrackClaimStatus));
            }
            // Code Attribution:
            // DbUpdateException Class
            // 15 October 2024
            // https://learn.microsoft.com/en-us/dotnet/api/system.data.entity.infrastructure.dbupdateexception?view=entity-framework-6.2.0
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.Message, $"Database update failed for claim {model.ClaimName} by user {userId}.");
                TempData["ErrorMessage"] = "An error occurred while saving your claim. Please try again.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"An unexpected error occurred while submitting claim {model.ClaimName} by user {userId}.");
                TempData["ErrorMessage"] = "An unexpected error occurred. Please contact support.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View(model);
            }
        }

        [Authorize(Roles = "Lecturer")]
        [HttpGet]
        public async Task<IActionResult> TrackClaimStatus()
        {
            try
            {
                // get the current users id
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("User ID is null or empty.");
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                // get the current user based on the user id
                var user = await _userManager.FindByIdAsync(userId);

                // check if user is null
                if (user == null)
                {
                    _logger.LogError($"User not found for ID: {userId}.");
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                // Fetch the user from the context, including the Claims navigation property
                var userWithClaims = await _context.Users
               .Include(u => u.Claims)
               .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (userWithClaims == null || userWithClaims.Claims == null || !userWithClaims.Claims.Any())
                {
                    _logger.LogInformation($"No claims found for user ID: {userId}.");
                    TempData["ErrorMessage"] = "No claims found. You haven't submitted any claims.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View(new List<Models.Claim>());
                }


                // Convert claims to a list
                List<Models.Claim> claims = userWithClaims.Claims.ToList();

                // check if the user has claims
                if (!claims.Any())
                {
                    _logger.LogInformation($"User {userId} has no claims.");
                    return View(new List<Models.Claim>());
                }

                _logger.LogInformation($"User {userId} has {claims.Count} claims.");

                return View(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"An error occurred while tracking claim status for user ID: {User.FindFirstValue(ClaimTypes.NameIdentifier)}.");
                TempData["ErrorMessage"] = "An error occurred while fetching your claims. Please try again later.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View(new List<Models.Claim>());
            }
        }

        [Authorize(Roles = "Lecturer, Academic Manager, HR")]
        [HttpGet]
        public async Task<IActionResult> DownloadSupportingDocument(string id)
        {
            try
            {
                // Check if the id is null or empty
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Download attempt with invalid claim ID.");
                    TempData["ErrorMessage"] = "Claim ID is invalid.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return RedirectToAction(nameof(TrackClaimStatus));
                }

                // Attempt to retrieve the claim from the database
                var claim = await _context.Claim.FindAsync(id);

                // Check if claim exists
                if (claim == null)
                {
                    _logger.LogWarning($"Claim with ID {id} not found.");
                    TempData["ErrorMessage"] = $"Claim with ID {id} was not found.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return RedirectToAction(nameof(TrackClaimStatus));
                }

                // Check if the document is available
                if (claim.Document == null || claim.Document.Length == 0)
                {
                    _logger.LogWarning($"Claim with ID {id} has no document.");
                    TempData["ErrorMessage"] = $"No supporting document found for Claim ID {id}.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return RedirectToAction(nameof(TrackClaimStatus));
                }

                _logger.LogInformation($"Claim with ID {id} found. Returning document.");

                // Code Attribution:
                // Download file in C# .Net Core
                // Pratham Jain
                // 16 October 2024
                // https://learn.microsoft.com/en-us/answers/questions/1033258/download-file-in-c-net-core

                // Return the document as a file
                return File(claim.Document, claim.DocumentType, claim.DocumentName);
            }
            catch (Exception ex)
            {
                // Log the error and return an error message
                _logger.LogError(ex.Message, $"Error downloading document for Claim ID {id}.");
                TempData["ErrorMessage"] = "An error occurred while downloading the document. Please try again later.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToAction(nameof(TrackClaimStatus));
            }
        }

        [Authorize(Roles = "Academic Manager, HR")]
        [HttpGet]
        public async Task<IActionResult> ViewAllPendingClaims(string? faculty)
        {
            try
            {
                // Retrieve Lecturer role
                var lecturerRole = await _roleManager.FindByNameAsync("Lecturer");

                // If Lecturer role doesn't exist
                if (lecturerRole == null)
                {
                    _logger.LogError("Lecturer role not found.");
                    TempData["ErrorMessage"] = "Lecturer role not found.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View(new List<Models.Claim>());
                }

                // get all users in the lecturer role
                var lecturerUsers = await _userManager.GetUsersInRoleAsync("Lecturer");

                // Check if there are any lecturers
                if (lecturerUsers == null || !lecturerUsers.Any())
                {
                    _logger.LogWarning("No lecturers found.");
                    TempData["ErrorMessage"] = "No lecturers found.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View(new List<Models.Claim>());
                }

                // Extract distinct faculties from lecturers
                var faculties = lecturerUsers.Select(l => l.Faculty).Distinct();

                // Check if the list of faculties is empty or null
                if (faculties == null || !faculties.Any())
                {
                    _logger.LogWarning("No faculties found.");
                    TempData["ErrorMessage"] = "No faculties found.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View(new List<Models.Claim>());
                }

                _logger.LogInformation("Faculties found");
                // pass the list of faculties to the view
                ViewBag.Faculties = faculties;

                // If faculty is selected, filter claims by faculty
                if (!string.IsNullOrEmpty(faculty))
                {
                    // Ensure faculty exists within the lecturer faculties
                    if (!faculties.Contains(faculty))
                    {
                        _logger.LogError($"Invalid faculty: {faculty}.");
                        TempData["ErrorMessage"] = $"Invalid faculty: {faculty}.";

                        ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                        return RedirectToAction(nameof(ViewAllPendingClaims));
                    }

                    // Get all pending claims for the selected faculty
                    var claims = await _context.Claim
                        .Include(c => c.User)
                        .Where(c => c.Status == "Pending" && c.User.Faculty == faculty)
                        .ToListAsync();

                    if (claims.Count == 0)
                    {
                        _logger.LogInformation($"No pending claims for faculty {faculty}.");
                        TempData["ErrorMessage"] = $"No pending claims for faculty {faculty}.";

                        ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                        return View(new List<Models.Claim>());
                    }

                    _logger.LogInformation($"Showing pending claims for faculty {faculty}.");
                    TempData["SuccessMessage"] = $"Showing pending claims for faculty {faculty}.";

                    // Display success message
                    ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                    return View(claims);
                }
                else
                {
                    // Get all pending claims across all faculties
                    var allClaims = await _context.Claim
                        .Where(c => c.Status == "Pending")
                        .ToListAsync();

                    // check if there are any pending claims
                    if (allClaims.Count == 0)
                    {
                        _logger.LogInformation("No pending claims across all faculties.");
                        TempData["ErrorMessage"] = "No pending claims found.";

                        ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                        return View(new List<Models.Claim>());
                    }

                    _logger.LogInformation("Showing pending claims across all faculties.");
                    TempData["SuccessMessage"] = "Showing pending claims across all faculties.";

                    // Pass success messages to the view
                    ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                    return View(allClaims);
                }
            }
            catch (Exception ex)
            {
                // Log and handle unexpected errors
                _logger.LogError(ex.Message, "An error occurred while retrieving pending claims.");
                TempData["ErrorMessage"] = "An error occurred while retrieving pending claims. Please try again later.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToAction(nameof(ViewAllPendingClaims));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Academic Manager, HR")]
        public async Task<IActionResult> ApproveClaim(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError("ApproveClaim: Claim ID is null or empty.");

                TempData["ErrorMessage"] = "Invalid claim ID provided.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToAction(nameof(ViewAllPendingClaims));
            }

            try
            {
                // get the claim based on the id
                var claim = await _context.Claim.FindAsync(id);

                // check if the claim is null
                if (claim == null)
                {
                    _logger.LogWarning($"ApproveClaim: Claim with ID {id} not found.");

                    TempData["ErrorMessage"] = "Claim not found.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return RedirectToAction(nameof(ViewAllPendingClaims));
                }

                // Check if the claim is already approved
                if (claim.Status == "Approved")
                {
                    _logger.LogInformation($"ApproveClaim: Claim {claim.ClaimName} is already approved.");

                    TempData["ErrorMessage"] = $"Claim {claim.ClaimName} is already approved.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return RedirectToAction(nameof(ViewAllPendingClaims));
                }

                // set the status of the claim to approved
                claim.Status = "Approved";

                // Attempt to update the claim in the database
                _context.Update(claim);
                await _context.SaveChangesAsync();


                _logger.LogInformation($"ApproveClaim: Claim {claim.ClaimName} approved successfully.");

                TempData["SuccessMessage"] = $"Claim {claim.ClaimName} has been approved.";

                // Pass success messages to the view
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                return RedirectToAction(nameof(ViewAllPendingClaims));
            }
            // Code Attribution:
            // DbUpdateConcurrencyException Class
            // 16 October 2024
            // https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbupdateconcurrencyexception?view=efcore-8.0
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency issues if multiple users try to update the same claim
                _logger.LogError(ex.Message, $"ApproveClaim: Concurrency error while approving claim with ID {id}.");

                TempData["ErrorMessage"] = "There was an issue approving the claim due to a concurrency conflict. Please try again.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToAction(nameof(ViewAllPendingClaims));
            }
            catch (Exception ex)
            {
                // General error handling for other unexpected exceptions
                _logger.LogError(ex.Message, $"ApproveClaim: Error occurred while approving claim with ID {id}.");

                TempData["ErrorMessage"] = "An error occurred while approving the claim. Please try again later.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToAction(nameof(ViewAllPendingClaims));
            }
        }

        [HttpPost]
        [Authorize(Roles = "Academic Manager, HR")]
        public async Task<IActionResult> RejectClaim(string id)
        {
            // check if the id is null
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError("RejectClaim: Claim ID is null or empty.");

                TempData["ErrorMessage"] = "Invalid claim ID provided.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToAction(nameof(ViewAllPendingClaims));
            }

            try
            {
                // get the claim based on the id
                var claim = await _context.Claim.FindAsync(id);

                // check if the claim is null
                if (claim == null)
                {
                    _logger.LogWarning($"RejectClaim: Claim with ID {id} not found.");

                    TempData["ErrorMessage"] = "Claim not found.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return RedirectToAction(nameof(ViewAllPendingClaims));
                }

                // Check if the claim is already rejected or approved
                if (claim.Status == "Rejected")
                {
                    _logger.LogInformation($"RejectClaim: Claim {claim.ClaimName} is already rejected.");

                    TempData["ErrorMessage"] = $"Claim {claim.ClaimName} is already rejected.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return RedirectToAction(nameof(ViewAllPendingClaims));
                }
                else if (claim.Status == "Approved")
                {
                    _logger.LogWarning($"RejectClaim: Claim {claim.ClaimName} has already been approved and cannot be rejected.");

                    TempData["ErrorMessage"] = $"Claim {claim.ClaimName} has already been approved and cannot be rejected.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return RedirectToAction(nameof(ViewAllPendingClaims));
                }

                // set the status of the claim to rejected
                claim.Status = "Rejected";

                // Attempt to update the claim in the database
                _context.Update(claim);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"RejectClaim: Claim {claim.ClaimName} rejected successfully.");

                TempData["SuccessMessage"] = $"Claim {claim.ClaimName} has been rejected.";

                // Pass success messages to the view
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                return RedirectToAction(nameof(ViewAllPendingClaims));
            }
            // Code Attribution:
            // DbUpdateConcurrencyException Class
            // 16 October 2024
            // https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbupdateconcurrencyexception?view=efcore-8.0
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency issues if multiple users try to update the same claim
                _logger.LogError(ex.Message, $"RejectClaim: Concurrency error while rejecting claim with ID {id}.");

                TempData["ErrorMessage"] = "There was an issue rejecting the claim due to a concurrency conflict. Please try again.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToAction(nameof(ViewAllPendingClaims));
            }
            catch (Exception ex)
            {
                // General error handling for other unexpected exceptions
                _logger.LogError(ex.Message, $"RejectClaim: Error occurred while rejecting claim with ID {id}.");

                TempData["ErrorMessage"] = "An error occurred while rejecting the claim. Please try again later.";

                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToAction(nameof(ViewAllPendingClaims));
            }
        }

        [Authorize(Roles = "HR")]
        [HttpGet]
        public async Task<IActionResult> ViewAllApprovedClaims()
        {
            try
            {
                // Retrieve all approved claims, including the associated User for faculty access
                var claims = await _context.Claim
                    .Include(c => c.User)
                    .Where(c => c.Status == "Approved")
                    .ToListAsync();

                // Check if there are any approved claims
                if (claims == null || !claims.Any())
                {
                    _logger.LogInformation("ViewAllApprovedClaims: No approved claims found.");

                    return View(new List<Models.Claim>()); // Return an empty list to the view
                }

                _logger.LogInformation($"ViewAllApprovedClaims: {claims.Count} approved claims found.");

                return View(claims);
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                _logger.LogError(ex.Message, "ViewAllApprovedClaims: An error occurred while retrieving approved claims.");


                return RedirectToAction("GetHRDashboard", "Dashboards");
            }
        }

        [Authorize(Roles = "HR")]
        [HttpGet]
        public async Task<IActionResult> ViewAllRejectedClaims()
        {
            try
            {
                // Retrieve all rejected claims, including the associated User for faculty access
                var claims = await _context.Claim
                    .Include(c => c.User)
                    .Where(c => c.Status == "Rejected")
                    .ToListAsync();

                // Check if there are any rejected claims
                if (claims == null || !claims.Any())
                {
                    _logger.LogInformation("ViewAllRejectedClaims: No rejected claims found.");

                    return View(new List<Models.Claim>()); // Return an empty list to the view
                }

                _logger.LogInformation($"ViewAllRejectedClaims: {claims.Count} rejected claims found.");

                return View(claims);
            }
            catch (Exception ex)
            {
                // Log the error with detailed exception information
                _logger.LogError(ex.Message, "ViewAllRejectedClaims: An error occurred while retrieving rejected claims.");

                return RedirectToAction("GetHRDashboard", "Dashboards");
            }
        }
    }
}
