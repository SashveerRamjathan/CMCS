using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using ST10361554_PROG6212_POE_Part_3_CMCS.Data;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Services
{
    // Service for retrieving invoice information
    public class InvoiceInformationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InvoiceInformationService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Constructor to initialize the service with dependencies
        public InvoiceInformationService(
            ApplicationDbContext dbContext, 
            ILogger<InvoiceInformationService> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = dbContext;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        // Method to get invoice information for a given lecturer and claim

        public async Task<InvoiceModel?> GetInvoiceInfoAsync(string claimId)
        {
            try
            {
                // Input validation
                if (string.IsNullOrEmpty(claimId))
                {
                    _logger.LogError("Claim Id is null or empty");
                    return null!;
                }

                // Retrieve the claim with the user details from the database
                var claim = await _context.Claim
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == claimId);


                // Check if claim is null
                if (claim == null)
                {
                    _logger.LogError($"Claim {claimId} not found");
                    return null!;
                }

                // check if user in claim is null
                if (claim.User == null)
                {
                    _logger.LogError($"User for claim {claimId} not found");
                    return null!;
                }

                _logger.LogInformation($"HR requested invoice for claim {claim.ClaimName} (ID: {claim.Id}) at {DateTime.Now.ToString()}");

                // Create the Lecturers address details
                Address LecturerAddress = new Address
                {
                    Name = (claim.User.FirstName + " " + claim.User.Surname),
                    Street = claim.User.StreetAddress!,
                    Area = claim.User.AreaAddress!,
                    City = claim.User.City!,
                    Province = claim.User.Province!,
                    PhoneNumber = claim.User.PhoneNumber!,
                    Email = claim.User.Email!
                };

                // Create Company address details
                Address CompanyAddress = new Address
                {
                    Name = "Contract Monthly Claims System",
                    Street = "164 Evergreen Lane",
                    Area = "Greenfield Heights",
                    City = "Johannesburg",
                    Province = "Gauteng",
                    PhoneNumber = "011 682 7901",
                    Email = "claims@cmcs.edu.za"
                };

                // Create an invoice number
                Random random = new Random();
                int invoiceNumber = random.Next(10000000, 99999999);

                // Get the issue date
                DateTime issueDate = DateTime.Now;

                // Get the invoice comments
                string comments = "Thank you for your valuable contribution to our institution. " +
                    $"\nThis claim invoice (Claim ID: {claim.Id}) has been issued for your payment. " +
                    "\nIf you have any questions or require further clarification, " +
                    "please contact us at 011 682 7901 or via email at support@cmcs.edu.za" +
                    "\n\nWe appreciate your dedication to academic excellence.";

                // Get the image as a byte array
                var image = GetImageAsByteArray();

                // Check if image array is null
                if (image == null)
                {
                    _logger.LogError("Image array is null");
                    return null!;
                }

                // Prepare the invoice model
                var model = new InvoiceModel
                {
                    InvoiceNumber = invoiceNumber,
                    IssueDate = issueDate,
                    LecturerAddress = LecturerAddress,
                    CompanyAddress = CompanyAddress,
                    LecturerClaim = claim,
                    Comments = comments,
                    LogoImage = image
                };

                return model;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError($"Error getting invoice information: {ex.Message}");

                return null!;
            }
        }

        // Method to get the image as a byte array
        private Image GetImageAsByteArray()
        {
            try
            {
                string relativePath = "Images/claimInvoiceLogo.jpg";

                var stream = _webHostEnvironment.WebRootFileProvider.GetFileInfo(relativePath).CreateReadStream();

                if (stream == null)
                {
                    throw new FileNotFoundException("The file was not found.", relativePath);
                }

                Image image = Image.FromStream(stream);

                return image;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting file: {ex.Message}");

                return null!;
            }
        }
    }
}
