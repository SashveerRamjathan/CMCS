using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.ViewModels
{
    public class SubmitClaimViewModel
    {
        // Generate a unique claim ID for each claim
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // claim properties
        [Required]
        [DisplayName("Claim Name")]
        [StringLength(100, ErrorMessage = "Claim Name cannot exceed 100 characters")]
        public string ClaimName { get; set; } // name of the claim

        [Required]
        [DisplayName("Claim Date")]
        [DataType(DataType.Date)]
        public DateTime ClaimDate { get; set; } // date of the claim

        [Required]
        [DisplayName("Claim Description")]
        [StringLength(300, ErrorMessage = "Claim Description cannot exceed 300 characters")]
        public string ClaimDescription { get; set; } // description of the claim

        [Required]
        [DisplayName("Hours Worked")]
        [Range(0.1, 200, ErrorMessage = "Hours Worked must be between 0.1 and 200")]
        public decimal HoursWorked { get; set; } // hours worked

        [Required]
        [DisplayName("Hourly Rate")]
        [Range(0.1, 2500, ErrorMessage = "Hourly Rate must be between 0.1 and 2500")]
        public decimal HourlyRate { get; set; } // hourly rate

        // Automatically calculate final amount based on hours worked and hourly rate
        [DisplayName("Final Amount")]
        public decimal FinalAmount => Math.Round(HoursWorked * HourlyRate, 2); // final amount = hours worked * hourly rate

        public string Status { get; set; } = "Pending"; // status of the claim

        // For document upload
        [Required]
        [DisplayName("Upload Supporting Documentation")]
        public IFormFile Document { get; set; }
    }
}
