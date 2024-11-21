namespace ST10361554_PROG6212_POE_Part_3_CMCS.Models
{
    // Class representing a report in the database
    public class Report
    {
        public string Id { get; set; } // primary key

        public byte[] Document { get; set; } // byte array to store the document

        public string DocumentName { get; set; } // name of the document

        public string DocumentType { get; set; } // type of the document(e.g. "application/pdf")
    }
}
