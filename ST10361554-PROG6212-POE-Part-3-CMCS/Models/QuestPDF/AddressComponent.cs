using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.Models.QuestPDF
{
    public class AddressComponent : IComponent
    {
        private string Title { get; }
        private Address Address { get; }

        // Constructor to initialize the title and address
        public AddressComponent(string title, Address address)
        {
            Title = title;
            Address = address;
        }

        // Method to compose the address component in the PDF container
        public void Compose(IContainer container)
        {
            container.ShowEntire().Column(column =>
            {
                column.Spacing(2); // Set spacing between items

                column.Item().Text(Title).SemiBold(); // Add title with semi-bold font
                column.Item().PaddingBottom(5).LineHorizontal(1); // Add a horizontal line with padding

                // Add address details
                column.Item().Text(Address.Name);
                column.Item().Text(Address.Street);
                column.Item().Text(Address.Area);
                column.Item().Text($"{Address.City}, {Address.Province}");
                column.Item().Text(Address.Email);
                column.Item().Text(Address.PhoneNumber);
            });
        }
    }
}
