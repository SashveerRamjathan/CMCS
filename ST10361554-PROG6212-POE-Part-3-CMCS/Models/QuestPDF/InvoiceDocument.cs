using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Models.QuestPDF
{
    [NotMapped]
    public class InvoiceDocument : IDocument
    {
        public InvoiceModel Model { get; }

        // Constructor to initialize the invoice model
        public InvoiceDocument(InvoiceModel model)
        {
            Model = model;
        }

        // Method to compose the document structure
        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50); // Set page margins

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
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column
                        .Item().Text($"Invoice #{Model.InvoiceNumber}")
                        .FontSize(20).SemiBold().FontColor(Colors.Orange.Medium); // Invoice number

                    column.Item().Text(text =>
                    {
                        text.Span("Issue date: ").Bold();
                        text.Span($"{Model.IssueDate:d}"); // Issue date
                    });

                    column.Item().Text(text =>
                    {
                        text.Span("Issue time: ").Bold();
                        text.Span($"{Model.IssueDate:t}"); // Issue time
                    });

                });

                row.ConstantItem(135).Image(Model.LogoImage); // Company logo
            });
        }

        // Method to compose the comments section
        private void ComposeComments(IContainer container)
        {
            container.ShowEntire().Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
            {
                column.Spacing(5);
                column.Item().Text("Comments").FontSize(14).SemiBold(); // Comments title
                column.Item().Text(Model.Comments); // Comments content
            });
        }

        private void ComposeClaimDetails(IContainer container)
        {
            Claim claim = Model.LecturerClaim; // Get the claim details

            var labelStyle = TextStyle.Default.FontSize(12).Bold(); // Style for labels
            var valueStyle = TextStyle.Default.FontSize(12); // Style for values

            container.Padding(10).Column(column =>
            {
                // Claim Title
                column.Item().Text("Claim Details").Style(TextStyle.Default.FontSize(16).Bold());

                column.Spacing(5); // Add spacing between rows

                // Add each claim detail row
                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Claim ID:").Style(labelStyle);
                    row.RelativeItem(3).Text(claim.Id).Style(valueStyle);
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Claim Name:").Style(labelStyle);
                    row.RelativeItem(3).Text(claim.ClaimName).Style(valueStyle);
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Claim Date:").Style(labelStyle);
                    row.RelativeItem(3).Text(claim.ClaimDate.ToString("dd MMM yyyy")).Style(valueStyle);
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Description:").Style(labelStyle);
                    row.RelativeItem(3).Text(claim.ClaimDescription).Style(valueStyle);
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Hours Worked:").Style(labelStyle);
                    row.RelativeItem(3).Text($"{claim.HoursWorked} hours").Style(valueStyle);
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Hourly Rate:").Style(labelStyle);
                    row.RelativeItem(3).Text(claim.HourlyRate.ToString("C", new CultureInfo("en-ZA"))).Style(valueStyle);
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Final Amount:").Style(labelStyle);
                    row.RelativeItem(3).Text(claim.FinalAmount.ToString("C", new CultureInfo("en-ZA"))).Style(valueStyle);
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem(2).Text("Status:").Style(labelStyle);
                    row.RelativeItem(3).Text(claim.Status).Style(valueStyle);
                });
            });
        }


        // Method to compose the main content of the invoice
        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(20);

                // Add address components for sender and recipient
                column.Item().Row(row =>
                {
                    row.RelativeItem().Component(new AddressComponent("From", Model.CompanyAddress));
                    row.ConstantItem(50);
                    row.RelativeItem().Component(new AddressComponent("For", Model.LecturerAddress));
                });

                column.Item().PaddingBottom(5).LineHorizontal(1); // Add a horizontal line with padding

                column.Item().Element(ComposeClaimDetails); // Add the claim details

                column.Item().PaddingBottom(5).LineHorizontal(1); // Add a horizontal line with padding

                // Add comments section if there are any comments
                if (!string.IsNullOrWhiteSpace(Model.Comments))
                {
                    column.Item().PaddingTop(25).Element(ComposeComments);
                }
            });
        }
    }
}
