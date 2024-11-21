using ST10361554_PROG6212_POE_Part_3_CMCS.Models;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.ViewModels
{
    public class ReportIndexViewModel
    {
        public List<string> Months { get; set; } = new List<string>(); // Months in "MMM yyyy" format

        public List<string> Modules { get; set; } = new List<string>(); // List of distinct modules

        public List<Report> Reports { get; set; } = new List<Report>(); // List of reports generated
    }
}
