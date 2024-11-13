namespace ST10361554_PROG6212_POE_Part_3_CMCS.Models
{
    public class Claim
    {
        public string Id { get; set; } // primary key

        // claim properties
        public string ClaimName { get; set; } // name of the claim

        public DateTime ClaimDate { get; set; } // date of the claim

        public string ClaimDescription { get; set; } // description of the claim

        public decimal HoursWorked { get; set; } // hours worked

        public decimal HourlyRate { get; set; } // hourly rate

        public decimal FinalAmount { get; set; } // final amount = hours worked * hourly rate

        public string Status { get; set; } // status of the claim


        // navigation property for associating a claim with a user
        public virtual ApplicationUser User { get; set; } = null!; // user who made the claim

        // document properties
        public byte[] Document { get; set; } // byte array to store the document

        public string DocumentName { get; set; } // name of the document

        public string DocumentType { get; set; } // type of the document(e.g. "application/pdf")
    }
}
