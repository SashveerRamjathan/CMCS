using ST10361554_PROG6212_POE_Part_3_CMCS.Models;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.ViewModels
{
    // represents the view model for an invoice item containing an invoice and the associated claim
    public class InvoiceItemViewModel
    {
        public Invoice Invoice { get; set; }

        public Claim Claim { get; set; }
    }
}
