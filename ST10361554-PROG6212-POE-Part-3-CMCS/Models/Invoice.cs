using Microsoft.EntityFrameworkCore;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Models
{
    // Class representing an invoice in the database
    public class Invoice
    {
        public string Id { get; set; } // primary key

        public string ClaimId { get; set; } // claim id associated with the invoice

        public byte[] Document { get; set; } // byte array to store the document

        public string DocumentName { get; set; } // name of the document

        public string DocumentType { get; set; } // type of the document(e.g. "application/pdf")
    }
}
