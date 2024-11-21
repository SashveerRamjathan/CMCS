using QuestPDF.Fluent;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models.QuestPDF;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Services
{
    // Service class for generating reports
    public class ReportGenerationService
    {
        private readonly ReportInformationService _informationService;
        private readonly ILogger<ReportGenerationService> _logger;

        // Constructor to initialize the service with dependencies
        public ReportGenerationService(
            ReportInformationService informationService,
            ILogger<ReportGenerationService> logger)
        {
            _informationService = informationService;
            _logger = logger;
        }

        public async Task<byte[]> GenerateReportAsync(DateTime month, string module)
        {
            try
            {
                // input validation
                if (string.IsNullOrEmpty(module))
                {
                    _logger.LogError("Module is null or empty");
                    return null!;
                }

                // Retrieve report information from the information service
                var reportInfo = await _informationService.GetReportInfoAsync(month, module);

                if (reportInfo != null)
                {
                    _logger.LogInformation($"Generating report for {module} in {month}");

                    // Create the ReportDocument using QuestPDF
                    var reportDocument = new ReportDocument(reportInfo);

                    // Generate the PDF into a memory stream
                    using (var memoryStream = new MemoryStream())
                    {
                        reportDocument.GeneratePdf(memoryStream);
                        _logger.LogInformation($"Report generated for {module} in {month}");
                        return memoryStream.ToArray();
                    };
                }

                return null!;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError($"Error generating report for {module} in {month}: {ex.Message}");

                return null!;
            }
        }
    }
}
