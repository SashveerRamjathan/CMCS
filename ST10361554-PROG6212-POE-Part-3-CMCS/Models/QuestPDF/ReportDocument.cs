using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Models.QuestPDF
{
    [NotMapped]
    public class ReportDocument : IDocument
    {
        public ReportModel Model { get; set; }

        // Constructor to initialize the report model
        public ReportDocument(ReportModel reportModel)
        {
            Model = reportModel;
        }

        // Method to compose the document structure
        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(35); // Set page margins

                    page.Header().Element(ComposeHeader); // Compose the header
                    page.Content().Element(ComposeContent); // Compose the content

                    // Compose the footer with page numbers
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
        }

        // Method to get document metadata
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        // Method to compose the header section
        private void ComposeHeader(IContainer container)
        {
            container.ShowOnce().Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column
                        .Item().Text($"Report #{Model.ReportNumber}")
                        .FontSize(20).SemiBold().FontColor(Colors.Orange.Medium); // Invoice number

                    column.Item().Row(row => row.RelativeItem().Height(5)); // Adds space without affecting layout

                    column.Item().Text(text =>
                    {
                        text.Span("Issue date: ").Bold();
                        text.Span($"{Model.ReportDate:d}"); // Issue date
                    });

                    column.Item().Text(text =>
                    {
                        text.Span("Issue time: ").Bold();
                        text.Span($"{Model.ReportDate:t}"); // Issue time
                    });

                    column.Item().Text(text =>
                    {
                        text.Span("Month Filtered: ").Bold();
                        text.Span($"{Model.Month:Y}"); // Month Filtered
                    });

                    column.Item().Text(text =>
                    {
                        text.Span("Module Filtered: ").Bold();
                        text.Span($"{Model.Module}"); // Module Filtered

                    });

                });

                row.ConstantItem(135).Image(Model.LogoImage); // report logo
            });
        }

        // method to compose the approved claims table
        private void ComposeApprovedClaimsTable(IContainer container)
        {
            var headerStyle = TextStyle.Default.SemiBold(); // Header text style

            container.Padding(5).Column(column =>
            {
                // Claim Title
                column.Item().Text("Approved Claims").Style(TextStyle.Default.FontSize(16).Bold().FontColor(Colors.Orange.Medium));

                // Add spacing between the title and the table
                column.Item().Height(5);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(25); // Index column
                        columns.RelativeColumn(); // Claim name column
                        columns.RelativeColumn(); // claim status column
                        columns.RelativeColumn(); // Hours worked column
                        columns.RelativeColumn(); // Hourly Rate column
                        columns.RelativeColumn(); // Final Amount column
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("#");
                        header.Cell().Text("Claim Name").Style(headerStyle);
                        header.Cell().Text("Claim Status").Style(headerStyle);
                        header.Cell().AlignRight().Text("Hours Worked").Style(headerStyle);
                        header.Cell().AlignRight().Text("Hourly Rate").Style(headerStyle);
                        header.Cell().AlignRight().Text("Final Amount").Style(headerStyle);

                        header.Cell().ColumnSpan(6).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Black); // Header bottom border
                    });

                    // Add each item to the table
                    foreach (var item in Model.ApprovedClaims)
                    {
                        var index = Model.ApprovedClaims.IndexOf(item) + 1;

                        table.Cell().Element(CellStyle).Text($"{index}");
                        table.Cell().Element(CellStyle).Text(item.ClaimName);

                        table.Cell().Element(CellStyle).Text(item.Status);

                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.HoursWorked}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.HourlyRate.ToString("C", new CultureInfo("en-ZA"))}");

                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.FinalAmount.ToString("C", new CultureInfo("en-ZA"))}");

                        // Style for each cell
                        static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                });
            });
        }

        // method to compose the pending claims table
        private void ComposePendingClaimsTable(IContainer container)
        {
            var headerStyle = TextStyle.Default.SemiBold(); // Header text style

            container.Padding(5).Column(column =>
            {
                // Claim Title
                column.Item().Text("Pending Claims").Style(TextStyle.Default.FontSize(16).Bold().FontColor(Colors.Orange.Medium));

                // Add spacing between the title and the table
                column.Item().Height(5);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(25); // Index column
                        columns.RelativeColumn(); // Claim name column
                        columns.RelativeColumn(); // Claim status column
                        columns.RelativeColumn(); // Hours worked column
                        columns.RelativeColumn(); // Hourly Rate column
                        columns.RelativeColumn(); // Final Amount column
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("#");
                        header.Cell().Text("Claim Name").Style(headerStyle);
                        header.Cell().Text("Claim Status").Style(headerStyle);
                        header.Cell().AlignRight().Text("Hours Worked").Style(headerStyle);
                        header.Cell().AlignRight().Text("Hourly Rate").Style(headerStyle);
                        header.Cell().AlignRight().Text("Final Amount").Style(headerStyle);

                        header.Cell().ColumnSpan(6).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Black); // Header bottom border
                    });

                    // Add each item to the table
                    foreach (var item in Model.PendingClaims)
                    {
                        var index = Model.PendingClaims.IndexOf(item) + 1;

                        table.Cell().Element(CellStyle).Text($"{index}");
                        table.Cell().Element(CellStyle).Text(item.ClaimName);

                        table.Cell().Element(CellStyle).Text(item.Status);

                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.HoursWorked}");
                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.HourlyRate.ToString("C", new CultureInfo("en-ZA"))}");

                        table.Cell().Element(CellStyle).AlignRight().Text($"{item.FinalAmount.ToString("C", new CultureInfo("en-ZA"))}");

                        // Style for each cell
                        static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                });
            });
        }

        private void ComposeSummedDetails(IContainer container)
        {
            var headingStyle = TextStyle.Default.FontSize(16).Bold().FontColor(Colors.Orange.Medium); // Main heading style
            var labelStyle = TextStyle.Default.FontSize(12).Bold(); // Style for labels
            var valueStyle = TextStyle.Default.FontSize(12); // Style for values

            container.Padding(5).Column(column =>
            {
                // Main Heading: Aggregate Data for Accepted Claims
                column.Item().Text("Summed Data for Accepted Claims").Style(headingStyle);
                column.Spacing(10); // Add spacing between sections

                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Total Number of Approved Claims:").Style(labelStyle);
                    row.RelativeItem(3).Text(Model.TotalApprovedClaims.ToString());
                });
                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Total Hours Worked:").Style(labelStyle);
                    row.RelativeItem(3).Text($"{Model.SummedHours} hours").Style(valueStyle);
                });
                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Total Claims Value:").Style(labelStyle);
                    row.RelativeItem(3).Text(Model.SummedTotalAmount.ToString("C", new CultureInfo("en-ZA"))).Style(valueStyle);
                });

            });
        }

        private void ComposeAggregateDetails(IContainer container)
        {
            var headingStyle = TextStyle.Default.FontSize(16).Bold(); // Main heading style
            var subheadingStyle = TextStyle.Default.FontSize(14).Bold(); // Subheading style
            var labelStyle = TextStyle.Default.FontSize(12).Bold(); // Style for labels
            var valueStyle = TextStyle.Default.FontSize(12); // Style for values

            container.Padding(5).Column(column =>
            {
                // Main Heading: Aggregate Data for Accepted Claims
                column.Item().Text("Aggregate Data for Accepted Claims").Style(headingStyle);
                column.Spacing(10); // Add spacing between sections

                // Subheading: Claim Hours
                column.Item().Text("Claim Hours").Style(subheadingStyle);
                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Average Hours:").Style(labelStyle);
                    row.RelativeItem(3).Text($"{Model.AverageHours} hours").Style(valueStyle);
                });
                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Highest Hours:").Style(labelStyle);
                    row.RelativeItem(3).Text($"{Model.HighestHours} hours").Style(valueStyle);
                });
                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Lowest Hours:").Style(labelStyle);
                    row.RelativeItem(3).Text($"{Model.LowestHours} hours").Style(valueStyle);
                });
                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Median Hours:").Style(labelStyle);
                    row.RelativeItem(3).Text($"{Model.MedianHours} hours").Style(valueStyle);
                });

                column.Item().PaddingBottom(1).LineHorizontal(1); // Add a horizontal line with padding

                // Subheading: Claim Totals
                column.Item().Text("Claim Totals").Style(subheadingStyle);
                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Average Total Amount:").Style(labelStyle);
                    row.RelativeItem(3).Text(Model.AverageTotalAmount.ToString("C", new CultureInfo("en-ZA"))).Style(valueStyle);
                });
                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Highest Total Amount:").Style(labelStyle);
                    row.RelativeItem(3).Text(Model.HighestTotalAmount.ToString("C", new CultureInfo("en-ZA"))).Style(valueStyle);
                });
                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Lowest Total Amount:").Style(labelStyle);
                    row.RelativeItem(3).Text(Model.LowestTotalAmount.ToString("C", new CultureInfo("en-ZA"))).Style(valueStyle);
                });
                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Median Total Amount:").Style(labelStyle);
                    row.RelativeItem(3).Text(Model.MedianTotalAmount.ToString("C", new CultureInfo("en-ZA"))).Style(valueStyle);
                });
            });
        }

        // Method to compose the main content of the report
        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(5);

                column.Item().Element(ComposePendingClaimsTable); // Add the pending claims table

                column.Item().PaddingBottom(1).LineHorizontal(1); // Add a horizontal line with padding

                column.Item().Element(ComposeApprovedClaimsTable); // Add the approved claims table

                column.Item().PaddingBottom(1).LineHorizontal(1); // Add a horizontal line with padding

                column.Item().Element(ComposeAggregateDetails); // Add the aggregate details

                column.Item().PaddingBottom(1).LineHorizontal(1); // Add a horizontal line with padding

                column.Item().Element(ComposeSummedDetails); // Add the summed details
            });

        }
    }
}
