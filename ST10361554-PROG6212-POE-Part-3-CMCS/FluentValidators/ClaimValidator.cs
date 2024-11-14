using FluentValidation;
using ST10361554_PROG6212_POE_Part_3_CMCS.Models;

namespace ST10361554_PROG6212_POE_Part_3_CMCS.FluentValidators
{
    public class ClaimValidator : AbstractValidator<Claim>
    {
        public ClaimValidator() 
        {
            // validate Id
            RuleFor(c => c.Id)
                .NotEmpty()
                .NotEqual(Guid.Empty.ToString())
                .WithMessage("Invalid claim ID");

            // validate ClaimName
            RuleFor(c => c.ClaimName)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Claim Name cannot exceed 100 characters");

            // validate ClaimDate
            RuleFor(c => c.ClaimDate)
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage("Claim Date cannot be in the future");

            // validate ClaimDescription
            RuleFor(c => c.ClaimDescription)
                .NotEmpty()
                .MaximumLength(300)
                .WithMessage("Claim Description cannot exceed 300 characters");

            // validate HoursWorked
            RuleFor(c => c.HoursWorked)
                .GreaterThan(0)
                .LessThanOrEqualTo(50)
                .WithMessage("Hours Worked must be between 0.1 and 50");

            // validate HourlyRate
            RuleFor(c => c.HourlyRate)
                .GreaterThan(0)
                .LessThanOrEqualTo(500)
                .WithMessage("Hourly Rate must be between 0.1 and 500");

            // validate final amount
            RuleFor(c => c.FinalAmount)
                .Equal(c => c.HoursWorked * c.HourlyRate)
                .WithMessage("Final Amount must be equal to Hours Worked * Hourly Rate");

            // validate Status
            RuleFor(c => c.Status)
                .NotEmpty()
                .WithMessage("Status cannot be empty");

            // validate document
            RuleFor(c => c.Document)
                .NotEmpty()
                .WithMessage("Document cannot be empty");

            // validate DocumentName
            RuleFor(c => c.DocumentName)
                .NotEmpty()
                .WithMessage("Document Name cannot be empty");

            // validate DocumentType
            RuleFor(c => c.DocumentType)
                .Must(docType => new[]
                {
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // Excel
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // Word
                    "application/pdf" // PDF
                }.Contains(docType))
                .WithMessage("Only Excel, Word, and PDF files are allowed.");
        }

    }
}
