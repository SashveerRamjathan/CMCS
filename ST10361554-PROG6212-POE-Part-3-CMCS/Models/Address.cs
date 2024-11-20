using System.ComponentModel.DataAnnotations.Schema;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Models
{
    // Class representing an address
    [NotMapped]
    public class Address
    {
        public string Name { get; set; } // Name of the person or entity
        public string Street { get; set; } // Street address
        public string Area { get; set; } // Area
        public string City { get; set; } // City
        public string Province { get; set; } // Province
        public string PhoneNumber { get; set; } // Contact phone number
        public string Email { get; set; } // Contact email address
    }
}
