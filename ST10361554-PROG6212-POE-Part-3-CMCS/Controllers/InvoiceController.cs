using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10361554_PROG6212_POE_Part_3_CMCS.Data;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models;
using ST10361554_PROG6212_POE_Part_3_CMCS.Services;
using ST10361554_PROG6212_POE_Part_3_CMCS.ViewModels;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Controllers
{
    [Authorize(Roles = "HR")]
    public class InvoiceController : Controller
    {
        private readonly ILogger<InvoiceController> _logger;
        private readonly InvoiceGenerationService _invoiceGenerationService;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvoiceController(
            ILogger<InvoiceController> logger,
            InvoiceGenerationService generationService,
            ApplicationDbContext dbContext,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _invoiceGenerationService = generationService;
            _context = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? faculty)
        {
            try
            {
                var model = new GenerateInvoiceViewModel
                {
                    Claims = new List<Claim>(),
                    InvoiceItems = new List<InvoiceItemViewModel>()
                };

                // Get all the approved claims from the database
                var claims = await _context.Claim
                    .Where(c => c.Status == "Approved")
                    .Include(c => c.User)
                    .ToListAsync();

                // Check if the list of claims is empty or null
                if (claims == null || !claims.Any())
                {
                    _logger.LogWarning("No Approved claims found.");
                    TempData["ErrorMessage"] = "No Approved claims found.";
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                    return View(model);
                }

                _logger.LogInformation("Approved claims found");

                // extract a list of faculties from the claims
                var faculties = claims.Select(c => c.User.Faculty).Distinct().ToList();

                // Check if the list of faculties is empty or null
                if (faculties == null || !faculties.Any())
                {
                    _logger.LogWarning("No faculties found.");
                    TempData["ErrorMessage"] = "No faculties found.";

                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                    return View(model);
                }

                _logger.LogInformation("Faculties found");

                // pass the list of faculties to the view
                ViewBag.Faculties = faculties;

                // if the faculty to filter is selected, filter the claims by faculty
                if (!string.IsNullOrEmpty(faculty))
                {
                    // filter the claims by the selected faculty
                    claims = claims.Where(c => c.User.Faculty == faculty).ToList();

                    // Check if the list of claims is empty or null
                    if (claims == null || !claims.Any())
                    {
                        _logger.LogWarning("No claims found for the selected faculty.");
                        TempData["ErrorMessage"] = "No claims found for the selected faculty.";
                        ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                        return View(model);
                    }

                    _logger.LogInformation("Claims found for the selected faculty");

                    List<Claim> claimsToInvoice = new List<Claim>();

                    List<Claim> claimsInvoiced = new List<Claim>();

                    // check if the claims have already been invoiced
                    foreach (var claim in claims)
                    {
                        var invoice = await _context.Invoice.FirstOrDefaultAsync(i => i.ClaimId == claim.Id);
                        if (invoice == null)
                        {
                            claimsToInvoice.Add(claim);
                        }
                        else
                        {
                            claimsInvoiced.Add(claim);
                        }
                    }

                    // Check if the list of claims to invoice is empty or null
                    if (claimsToInvoice == null || !claimsToInvoice.Any())
                    {
                        _logger.LogWarning("No claims found to invoice for the selected faculty.");
                    }

                    model.Claims = claimsToInvoice;

                    // get all the invoices already generated

                    var allInvoices = await _context.Invoice.ToListAsync();

                    var invoiceItems = new List<InvoiceItemViewModel>();

                    // create a list of invoice items with the invoice and the associated claim
                    foreach (var invoice in allInvoices)
                    {
                        var claim = await _context.Claim.FirstOrDefaultAsync(c => c.Id == invoice.ClaimId);
                        if (claim != null)
                        {
                            var invoiceItem = new InvoiceItemViewModel
                            {
                                Invoice = invoice,
                                Claim = claim
                            };

                            invoiceItems.Add(invoiceItem);
                        }
                    }

                    model.InvoiceItems = invoiceItems;

                    // success message for filtering by faculty
                    TempData["SuccessMessage"] = $"Claims filtered for {faculty} faculty";

                    ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                    return View(model);
                }
                else
                {
                    List<Claim> claimsToInvoice = new List<Claim>();

                    // check if the claims have already been invoiced
                    foreach (var claim in claims)
                    {
                        var invoice = await _context.Invoice.FirstOrDefaultAsync(i => i.ClaimId == claim.Id);
                        if (invoice == null)
                        {
                            claimsToInvoice.Add(claim);
                        }
                    }

                    // Check if the list of claims to invoice is empty or null
                    if (claimsToInvoice == null || !claimsToInvoice.Any())
                    {
                        _logger.LogWarning("No claims found to invoice.");
                    }

                    model.Claims = claimsToInvoice;

                    // get all the invoices already generated

                    var allInvoices = await _context.Invoice.ToListAsync();

                    var invoiceItems = new List<InvoiceItemViewModel>();

                    // create a list of invoice items with the invoice and the associated claim
                    foreach (var invoice in allInvoices)
                    {
                        var claim = await _context.Claim.FirstOrDefaultAsync(c => c.Id == invoice.ClaimId);
                        if (claim != null)
                        {
                            var invoiceItem = new InvoiceItemViewModel
                            {
                                Invoice = invoice,
                                Claim = claim
                            };

                            invoiceItems.Add(invoiceItem);
                        }
                    }

                    model.InvoiceItems = invoiceItems;

                    ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                // log and handle unexpected errors
                _logger.LogError($"Error getting claims and invoice: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while getting claims and invoices. Please try again later";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerateInvoice(string claimId)
        {
            // check if claimId is null or empty
            if (string.IsNullOrEmpty(claimId))
            {
                _logger.LogError("Claim Id is null or empty");
                TempData["ErrorMessage"] = "Claim Id is null or empty";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // check if the invoice for the claim already exists
                var invoice = await _context.Invoice.FirstOrDefaultAsync(i => i.ClaimId == claimId);

                if (invoice != null)
                {
                    _logger.LogWarning($"Invoice for claim {claimId} already exists");
                    TempData["ErrorMessage"] = $"Invoice for claim {claimId} already exists";
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                    return RedirectToAction(nameof(Index));
                }

                // generate the invoice for the claim
                var generatedInvoice = await _invoiceGenerationService.GenerateInvoiceAsync(claimId);

                // check if the generated invoice is null
                if (generatedInvoice == null)
                {
                    _logger.LogError($"Error generating invoice for claim {claimId}");
                    TempData["ErrorMessage"] = $"Error generating invoice for claim {claimId}";
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                    return RedirectToAction(nameof(Index));
                }

                // save the invoice to the database
                var newInvoice = new Invoice
                {
                    Id = Guid.NewGuid().ToString(),
                    ClaimId = claimId,
                    Document = generatedInvoice,
                    DocumentName = $"Invoice_{claimId}.pdf",
                    DocumentType = "application/pdf"
                };

                _context.Invoice.Add(newInvoice);

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Invoice for claim {claimId} generated successfully");

                TempData["SuccessMessage"] = $"Invoice for claim {claimId} generated successfully";
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // log and handle database errors
                _logger.LogError($"Error saving invoice for claim {claimId}: {ex.Message}");
                TempData["ErrorMessage"] = $"An error occurred while saving invoice for claim {claimId}. Please try again later";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // log and handle unexpected errors
                _logger.LogError($"Error generating invoice for claim {claimId}: {ex.Message}");
                TempData["ErrorMessage"] = $"An error occurred while generating invoice for claim {claimId}. Please try again later";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewInvoice(string claimId)
        {
            // check if claimId is null or empty
            if (string.IsNullOrEmpty(claimId))
            {
                _logger.LogError("Claim Id is null or empty");
                TempData["ErrorMessage"] = "Claim Id is null or empty";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // get the invoice for the claim id
                var invoice = await _context.Invoice.FirstOrDefaultAsync(i => i.ClaimId == claimId);

                // check if the invoice is null
                if (invoice == null)
                {
                    _logger.LogWarning($"Invoice for claim {claimId} not found");
                    TempData["ErrorMessage"] = $"Invoice for claim {claimId} not found";
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation($"Invoice for claim {claimId} found");

                // Code Attribution:
                // How to open PDF file in a new tab or window instead of downloading it using C# and ASP.NET MVC?
                // marc_s
                //  Mar 5, 2019 at 5:47
                // https://stackoverflow.com/questions/54995753/how-to-open-pdf-file-in-a-new-tab-or-window-instead-of-downloading-it-using-c-sh

                // return the invoice as a file in a new tab

                HttpContext.Response.Headers.Append("Content-Disposition", $"inline; filename={invoice.DocumentName}");

                return File(invoice.Document, invoice.DocumentType);
            }
            catch (Exception ex)
            {
                // log and handle unexpected errors
                _logger.LogError($"Error viewing invoice for claim {claimId}: {ex.Message}");
                TempData["ErrorMessage"] = $"An error occurred while viewing invoice for claim {claimId}. Please try again later";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadInvoice(string claimId)
        {
            // check if claimId is null or empty
            if (string.IsNullOrEmpty(claimId))
            {
                _logger.LogError("Claim Id is null or empty");
                TempData["ErrorMessage"] = "Claim Id is null or empty";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                return RedirectToAction(nameof(Index));
            }
            try
            {
                // get the invoice for the claim id
                var invoice = await _context.Invoice.FirstOrDefaultAsync(i => i.ClaimId == claimId);

                // check if the invoice is null
                if (invoice == null)
                {
                    _logger.LogWarning($"Invoice for claim {claimId} not found");
                    TempData["ErrorMessage"] = $"Invoice for claim {claimId} not found";
                    ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogInformation($"Invoice for claim {claimId} found");

                TempData["SuccessMessage"] = $"Invoice for claim {claimId} downloaded successfully";
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];

                // Code Attribution:
                // Download file in C# .Net Core
                // Pratham Jain
                // 16 October 2024
                // https://learn.microsoft.com/en-us/answers/questions/1033258/download-file-in-c-net-core

                // return the invoice as a file to download
                return File(invoice.Document, invoice.DocumentType, invoice.DocumentName);
            }
            catch (Exception ex)
            {
                // log and handle unexpected errors
                _logger.LogError($"Error downloading invoice for claim {claimId}: {ex.Message}");
                TempData["ErrorMessage"] = $"An error occurred while downloading invoice for claim {claimId}. Please try again later";
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
