using QuestPDF.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Models
{
    // class representing the data for a report
    [NotMapped]
    public class ReportModel
    {
        // Report Info
        public int ReportNumber { get; set; }

        public DateTime ReportDate { get; set; }


        // Filter Info
        public DateTime Month { get; set; }

        public string Module { get; set; }


        // Claim Info
        public List<Claim> ApprovedClaims { get; set; }

        public List<Claim> PendingClaims { get; set; }


        // Aggregate data

        // Rate for accepted claims
        public decimal AverageRate { get; set; }
        public decimal HighestRate { get; set; }
        public decimal LowestRate { get; set; }
        public decimal MedianRate { get; set; }

        // Total amount for accepted claims
        public decimal AverageTotalAmount { get; set; }
        public decimal HighestTotalAmount { get; set; }
        public decimal LowestTotalAmount { get; set; }
        public decimal MedianTotalAmount { get; set; }

        // Hours for accepted claims
        public decimal AverageHours { get; set; }
        public decimal HighestHours { get; set; }
        public decimal LowestHours { get; set; }
        public decimal MedianHours { get; set; }

        // Sum data

        // Total amount for accepted claims
        public decimal SummedHours { get; set; }
        public decimal SummedTotalAmount { get; set; }
        public int TotalApprovedClaims { get; set; }


        // logo image
        public Image LogoImage { get; set; }

    }
}
