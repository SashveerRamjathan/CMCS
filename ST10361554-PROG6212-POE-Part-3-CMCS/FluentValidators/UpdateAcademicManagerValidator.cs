using FluentValidation;
using ST10361554_PROG6212_POE_Part_3_CMCS.ViewModels;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.FluentValidators
{
    // Code Attribution:
    // Creating your first validator
    // Fluentvalidation.net
    // 14 November 2024
    // https://docs.fluentvalidation.net/en/latest/start.html

    public class UpdateAcademicManagerValidator : AbstractValidator<UpdateAcademicManagerViewModel>
    {
        public UpdateAcademicManagerValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name is required.");
            RuleFor(x => x.Surname).NotEmpty().WithMessage("Surname is required.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid email is required.");

            // For PhoneNumber: Remove all whitespaces temporarily for validation
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Must(phone => !string.IsNullOrWhiteSpace(phone) && phone.Replace(" ", "").Length <= 15)
                .WithMessage("Phone number cannot be longer than 15 characters");

            RuleFor(x => x.StreetAddress).NotEmpty().WithMessage("Street address is required.");
            RuleFor(x => x.AreaAddress).NotEmpty().WithMessage("Area address is required.");
            RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
            RuleFor(x => x.Province).NotEmpty().WithMessage("Province is required.");
        }
    }
}
