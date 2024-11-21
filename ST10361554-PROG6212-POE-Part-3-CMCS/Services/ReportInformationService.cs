using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using ST10361554_PROG6212_POE_Part_3_CMCS.Data;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Services
{
    // service for generating report information
    public class ReportInformationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReportInformationService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // constructor to initialize the service with dependencies
        public ReportInformationService(
            ApplicationDbContext dbContext,
            ILogger<ReportInformationService> logger,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = dbContext;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        // Code Attribution:
        // C# — Find the Median of a List<int>
        // Gautami Sanjiva
        // 21 October 2024
        // https://medium.com/@mankapure99/c-find-the-median-of-a-list-int-3e3fa6053d2a

        // method to calculate the median of a list of numbers
        public decimal Median(IEnumerable<decimal> source)
        {
            // create a copy of the input list
            var sortedList = source.OrderBy(i => i).ToList();

            // calculate the median
            if (sortedList.Count % 2 == 0)
            {
                return (sortedList[sortedList.Count / 2] + sortedList[sortedList.Count / 2 - 1]) / 2;
            }
            else
            {
                return sortedList[sortedList.Count / 2];
            }
        }

        // method to get report information for a given month, module
        public async Task<ReportModel?> GetReportInfoAsync(DateTime month, string module)
        {
            try
            {
                // input validation
                if (string.IsNullOrEmpty(module))
                {
                    _logger.LogError("Module is null or empty");
                    return null!;
                }

                // retrieve a list of all the claims for the given month and module
                var claims = await _context.Claim
                    .Include(c => c.User)
                    .Where(c => c.User.Module == module && c.ClaimDate.Month == month.Month && c.ClaimDate.Year == month.Year)
                    .ToListAsync();

                // check if claims is null
                if (claims == null)
                {
                    _logger.LogError($"No claims found for module {module} in {month.ToString("MMMM yyyy")}");
                    return null!;
                }

                // split the claims into approved and pending claims
                var approvedClaims = claims.Where(c => c.Status.Equals("Approved")).ToList();

                var pendingClaims = claims.Where(c => c.Status.Equals("Pending")).ToList();

                // check if approved claims is null
                if (approvedClaims.Count == 0)
                {
                    _logger.LogError($"No approved claims found for module {module} in {month.ToString("MMMM yyyy")}");
                }

                // check if pending claims is null
                if (pendingClaims.Count == 0)
                {
                    _logger.LogError($"No pending claims found for module {module} in {month.ToString("MMMM yyyy")}");
                }

                // calculate the aggregate data for the approved claims

                var averageTotalAmountApproved = Math.Round(approvedClaims.Average(c => c.FinalAmount), 2);
                var highestTotalAmountApproved = Math.Round(approvedClaims.Max(c => c.FinalAmount), 2);
                var lowestTotalAmountApproved = Math.Round(approvedClaims.Min(c => c.FinalAmount), 2);
                var medianTotalAmountApproved = Math.Round(Median(approvedClaims.Select(c => c.FinalAmount)), 2);

                var averageHoursApproved = Math.Round(approvedClaims.Average(c => c.HoursWorked), 2);
                var highestHoursApproved = Math.Round(approvedClaims.Max(c => c.HoursWorked), 2);
                var lowestHoursApproved = Math.Round(approvedClaims.Min(c => c.HoursWorked), 2);
                var medianHoursApproved = Math.Round(Median(approvedClaims.Select(c => c.HoursWorked)), 2);

                // calculate the summary data for the approved claims
                var totalHoursApproved = Math.Round(approvedClaims.Sum(c => c.HoursWorked), 2);
                var totalAmountApproved = Math.Round(approvedClaims.Sum(c => c.FinalAmount), 2);
                var totalClaimsApproved = approvedClaims.Count;


                // create an ReportNumber
                Random random = new Random();
                var reportNumber = random.Next(100000, 999999);

                // get the issue date
                var reportDate = DateTime.Now;

                // get the logo image
                var logoImage = GetImageAsByteArray();

                // check if logo image is null
                if (logoImage == null)
                {
                    _logger.LogError("Logo image not found");
                    return null!;
                }

                // create the report model
                var reportModel = new ReportModel
                {
                    ReportNumber = reportNumber,
                    ReportDate = reportDate,

                    Month = month,
                    Module = module,

                    ApprovedClaims = approvedClaims,
                    PendingClaims = pendingClaims,

                    AverageTotalAmount = averageTotalAmountApproved,
                    HighestTotalAmount = highestTotalAmountApproved,
                    LowestTotalAmount = lowestTotalAmountApproved,
                    MedianTotalAmount = medianTotalAmountApproved,

                    AverageHours = averageHoursApproved,
                    HighestHours = highestHoursApproved,
                    LowestHours = lowestHoursApproved,
                    MedianHours = medianHoursApproved,

                    SummedHours = totalHoursApproved,
                    SummedTotalAmount = totalAmountApproved,
                    TotalApprovedClaims = totalClaimsApproved,

                    LogoImage = logoImage
                };

                return reportModel;
            }
            catch (Exception ex)
            {
                // log the exception
                _logger.LogError($"Error getting report information: {ex.Message}");

                return null!;
            }
        }

        // Method to get the image as a byte array
        private Image GetImageAsByteArray()
        {
            try
            {
                string relativePath = "Images/ReportLogo.jpg";

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
