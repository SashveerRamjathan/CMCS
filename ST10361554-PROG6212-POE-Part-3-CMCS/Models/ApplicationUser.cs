using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.Security.Claims;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Models
{
    public class ApplicationUser : IdentityUser
    {
        // General user properties
        [DisplayName("First Name")]
        public string? FirstName { get; set; }

        [DisplayName("Surname")]
        public string? Surname { get; set; }

        [DisplayName("Street Address")]
        public string? StreetAddress { get; set; }

        [DisplayName("Residential Area")]
        public string? AreaAddress { get; set; }

        [DisplayName("City")]
        public string? City { get; set; }

        [DisplayName("Province")]
        public string? Province { get; set; }


        // Lecturer properties
        [DisplayName("Faculty")]
        public string? Faculty { get; set; }

        [DisplayName("Module")]
        public string? Module { get; set; }

        [DisplayName("Bank Account Number")]
        public string? AccountNumber { get; set; }

        [DisplayName("Bank Name")]
        public string? BankName { get; set; }

        [DisplayName("Branch Code")]
        public string? BranchCode { get; set; }

        public bool IsLecturerApproved { get; set; }


        // Academic Manager properties

        public bool IsManagerApproved { get; set; }

        // Navigation properties for claims for lecturer users
        public ICollection<Claim>? Claims { get; set; } = new List<Claim>(); // nullable so if user is HR/AM doesn't affect them
    }
}
