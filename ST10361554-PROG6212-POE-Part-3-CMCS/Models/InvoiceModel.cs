using QuestPDF.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Models
{
    // Class representing the model for an invoice
    [NotMapped]
    public class InvoiceModel
    {
        public int InvoiceNumber { get; set; } // Unique number identifying the invoice
        public DateTime IssueDate { get; set; } // Date when the invoice was issued

        public Address LecturerAddress { get; set; } // Address of the Lecturer the invoice is issued to
        public Address CompanyAddress { get; set; } // Address of the company issuing the invoice

        public Claim LecturerClaim { get; set; } // Claim details for the lecturer

        public string Comments { get; set; } // Additional comments or notes for the invoice

        public Image LogoImage { get; set; } // Claim logo image to be included in the invoice
    }
}
