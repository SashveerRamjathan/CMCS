using ST10361554_PROG6212_POE_Part_3_CMCS.Models;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.ViewModels
{
    public class GenerateInvoiceViewModel
    {
        public List<Claim> Claims { get; set; } // List of claims to generate invoices for

        public List<InvoiceItemViewModel> InvoiceItems { get; set; } // List of invoices generated
    }
}
