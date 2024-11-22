using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10361554_PROG6212_POE_Part_3_CMCS.Data;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models;
using ST10361554_PROG6212_POE_Part_3_CMCS.Services;
using ST10361554_PROG6212_POE_Part_3_CMCS.ViewModels;
using System.Globalization;
using System.Security.Claims;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Controllers
{
    [Authorize(Roles = "HR")]
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly ReportGenerationService _reportGenerationService;
        private readonly ApplicationDbContext _context;

        public ReportController(
            ILogger<ReportController> logger,
            ReportGenerationService reportGenerationService,
            ApplicationDbContext context)
        {
            _logger = logger;
            _reportGenerationService = reportGenerationService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                ReportIndexViewModel model = new ReportIndexViewModel();

                // get all the approved and pending claims from the database
                var claims = await _context.Claim
                    .Include(c => c.User)
                    .Where(c => c.Status.Equals("Approved") || c.Status.Equals("Pending"))
                    .ToListAsync();

                // check if there are any claims
                if (claims.Count == 0)
                {
                    _logger.LogInformation("No claims found");
                    TempData["ErrorMessage"] = "No Approved or Pending claims found.";
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                    return View(model);
                }

                // Populate the months with distinct values from the claim dates
                var months = claims
                    .Where(c => c.Status.Equals("Approved") || c.Status.Equals("Pending"))
                    .GroupBy(c => new { c.ClaimDate.Year, c.ClaimDate.Month }) // Group by year and month
                    .Select(g => new DateTime(g.Key.Year, g.Key.Month, 1)) // Select as DateTime objects
                    .OrderBy(date => date) // Sort by DateTime
                    .Select(date => date.ToString("MMMM yyyy")) // Convert back to string
                    .ToList();

                // Populate the modules with distinct values
                var modules = claims
                    .Select(c => c.User.Module) // Select the module
                    .Distinct()
                    .OrderBy(m => m) // Sort the modules alphabetically
                    .ToList();

                // get all the reports from the database
                var reports = await _context.Report
                    .ToListAsync();

                // Populate the model with the months and modules
                model.Months = months;

                model.Modules = modules!;

                model.Reports = reports;

                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return View(model);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting reports: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while getting the reports.";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                return View(new ReportIndexViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerateReport(string month, string module)
        {
            // input validation
            if (string.IsNullOrEmpty(month) || string.IsNullOrEmpty(module))
            {
                _logger.LogError("Month or Module is null or empty");
                TempData["ErrorMessage"] = "Month or Module is null or empty.";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Parse the month and attached year
                DateTime monthYear;

                if (!DateTime.TryParseExact(month, "MMMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out monthYear))
                {
                    _logger.LogError("Error parsing month");
                    TempData["ErrorMessage"] = "An error occurred while parsing the month.";
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                    return RedirectToAction(nameof(Index));
                }

                // Generate the report
                var report = await _reportGenerationService.GenerateReportAsync(monthYear, module);

                // Check if the report was generated
                if (report == null)
                {
                    _logger.LogError($"Error generating report for {module} in {month}");
                    TempData["ErrorMessage"] = "An error occurred while generating the report.";
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                    return RedirectToAction(nameof(Index));
                }

                string documentName = $"Report ({module} - {monthYear.ToString("MMM yyyy")}).pdf";

                // save the report to the database
                var newReport = new Report
                {
                    Id = Guid.NewGuid().ToString(),
                    DocumentName = documentName,
                    Document = report,
                    DocumentType = "application/pdf"
                };

                _context.Report.Add(newReport);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Report ({documentName}) generated successfully.");

                TempData["SuccessMessage"] = $"Report ({documentName}) generated successfully.";
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error saving report to the database: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while saving the report to the database.";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating report: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while generating the report.";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewReport(string id)
        {
            // check if the id is null or empty
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError("Id is null or empty");
                TempData["ErrorMessage"] = "Id is null or empty.";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToAction(nameof(Index));
            }

            try
            {
                // get the report for the id
                var report = await _context.Report
                    .FirstOrDefaultAsync(r => r.Id == id);

                // check if the report is null
                if (report == null)
                {
                    _logger.LogError($"Report with id {id} not found");
                    TempData["ErrorMessage"] = "Report not found.";
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation($"Report found for id {id}");

                // return the report as a file in a new tab

                // Code Attribution:
                // How to open PDF file in a new tab or window instead of downloading it using C# and ASP.NET MVC?
                // marc_s
                //  Mar 5, 2019 at 5:47
                // https://stackoverflow.com/questions/54995753/how-to-open-pdf-file-in-a-new-tab-or-window-instead-of-downloading-it-using-c-sh

                HttpContext.Response.Headers.Append("Content-Disposition", $"inline; filename={report.DocumentName}");

                return File(report.Document, report.DocumentType);
            }
            catch (Exception ex)
            {
                // log the exception
                _logger.LogError($"Error viewing report: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while viewing the report.";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadReport(string id)
        {
            // check if the id is null or empty
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogError("Id is null or empty");
                TempData["ErrorMessage"] = "Id is null or empty.";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToAction(nameof(Index));
            }

            try
            {
                // get the report for the id
                var report = await _context.Report
                    .FirstOrDefaultAsync(r => r.Id == id);

                // check if the report is null
                if (report == null)
                {
                    _logger.LogError($"Report with id {id} not found");
                    TempData["ErrorMessage"] = "Report not found.";
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation($"Report found for id {id}");

                TempData["SuccessMessage"] = $"Report for id {id} downloaded successfully";
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                // Code Attribution:
                // Download file in C# .Net Core
                // Pratham Jain
                // 16 October 2024
                // https://learn.microsoft.com/en-us/answers/questions/1033258/download-file-in-c-net-core

                return File(report.Document, report.DocumentType, report.DocumentName);
            }
            catch (Exception ex)
            {
                // log the exception
                _logger.LogError($"Error downloading report {id}: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while downloading the report.";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToAction(nameof(Index));
            }
        }
    }
}
