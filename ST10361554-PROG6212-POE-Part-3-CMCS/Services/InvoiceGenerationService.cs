using QuestPDF.Fluent;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models.QuestPDF;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Services
{
    // Service for generating invoices in PDF format
    public class InvoiceGenerationService
    {
        private readonly InvoiceInformationService _invoiceInformationService;
        private readonly ILogger<InvoiceGenerationService> _logger;

        // Constructor to initialize the service with dependencies
        public InvoiceGenerationService(
            InvoiceInformationService invoiceInformationService,
            ILogger<InvoiceGenerationService> logger)
        {
            _invoiceInformationService = invoiceInformationService;
            _logger = logger;
        }

        public async Task<byte[]> GenerateInvoiceAsync(string claimId)
        {
            try
            {
                // check if claimId is null or empty
                if (string.IsNullOrEmpty(claimId))
                {
                    _logger.LogError("Claim Id is null or empty");
                    return null!;
                }

                // Retrieve invoice details from the invoice information service
                var invoiceDetails = await _invoiceInformationService.GetInvoiceInfoAsync(claimId);

                if (invoiceDetails != null)
                {
                    _logger.LogInformation($"Generating Invoice for claim {claimId}");

                    // Create the InvoiceDocument using QuestPDF
                    var invoiceDocument = new InvoiceDocument(invoiceDetails);

                    // Generate the PDF into a memory stream
                    using(var memoryStream = new MemoryStream())
                    {
                        invoiceDocument.GeneratePdf(memoryStream);
                        _logger.LogInformation($"Invoice generated for claim {claimId}");

                        return memoryStream.ToArray();
                    };
                }

                return null!;
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError($"Error generating invoice for claim {claimId}: {ex.Message}");
                
                return null!;
            }
        }
    }
}
