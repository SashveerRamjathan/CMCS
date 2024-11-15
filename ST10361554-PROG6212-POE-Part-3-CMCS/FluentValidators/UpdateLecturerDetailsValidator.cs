using FluentValidation;
using ST10361554_PROG6212_POE_Part_3_CMCS.ViewModels;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.FluentValidators
{
    // Code Attribution:
    // Creating your first validator
    // Fluentvalidation.net
    // 14 November 2024
    // https://docs.fluentvalidation.net/en/latest/start.html

    public class UpdateLecturerDetailsValidator : AbstractValidator<UpdateLecturerUserViewModel>
    {
        public UpdateLecturerDetailsValidator(string userRole)
        {
            // Common rules for all users
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

            if (userRole.Equals("Lecturer"))
            {
                // Lecturer can only update specific fields
                RuleFor(x => x.Faculty).NotEmpty().WithMessage("Faculty is required.");
                RuleFor(x => x.Module).NotEmpty().WithMessage("Module is required.");

                // Prevent update of address and banking info for Lecturer
                RuleFor(x => x.AccountNumber).Empty().WithMessage("You cannot update banking information.");
                RuleFor(x => x.BankName).Empty().WithMessage("You cannot update banking information.");
                RuleFor(x => x.BranchCode).Empty().WithMessage("You cannot update banking information.");
                RuleFor(x => x.StreetAddress).Empty().WithMessage("You cannot update your address.");
                RuleFor(x => x.AreaAddress).Empty().WithMessage("You cannot update your address.");
                RuleFor(x => x.City).Empty().WithMessage("You cannot update your address.");
                RuleFor(x => x.Province).Empty().WithMessage("You cannot update your address.");
                RuleFor(x => x.HourlyRate).Empty().WithMessage("You cannot update your hourly rate.");
            }
            else if (userRole.Equals("Academic Manager") || userRole.Equals("HR"))
            {
                // Academic Manager and HR can update all fields
                RuleFor(x => x.Faculty).NotEmpty().WithMessage("Faculty is required.");
                RuleFor(x => x.Module).NotEmpty().WithMessage("Module is required.");
                RuleFor(x => x.HourlyRate)
                    .LessThanOrEqualTo(500)
                    .GreaterThan(0)
                    .NotEmpty()
                    .WithMessage("Hourly rate is required and must be more than 0 but can not exceed 500.");

                RuleFor(x => x.AccountNumber).NotEmpty().WithMessage("Account number is required.");
                RuleFor(x => x.BankName).NotEmpty().WithMessage("Bank name is required.");
                RuleFor(x => x.BranchCode).NotEmpty().WithMessage("Branch code is required.");

                RuleFor(x => x.StreetAddress).NotEmpty().WithMessage("Street address is required.");
                RuleFor(x => x.AreaAddress).NotEmpty().WithMessage("Area address is required.");
                RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
                RuleFor(x => x.Province).NotEmpty().WithMessage("Province is required.");
            }
        }
    }
}
