using BankingApp.Web.Constants;
using BankingApp.Web.Models.Registration;
using FluentValidation;

namespace BankingApp.Web.Validators.Registration;

public class RegistrationRequestValidator
    : AbstractValidator<RegistrationRequest>
{
    public RegistrationRequestValidator()
    {
        // =========================================
        // COMMON USER VALIDATION
        // =========================================

        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username is required.")
            .Length(3, 50)
            .WithMessage(
                "Username must be between 3 and 50 characters.");

        RuleFor(x => x.LoginPassword)
            .NotEmpty()
            .WithMessage("Login password is required.")
            .MinimumLength(8)
            .WithMessage(
                "Login password must be at least 8 characters.")
            .Matches("[A-Z]")
            .WithMessage(
                "Login password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage(
                "Login password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage(
                "Login password must contain at least one number.")
            .Matches(@"[^a-zA-Z0-9]")
            .WithMessage(
                "Login password must contain at least one special character.");

        RuleFor(x => x.ConfirmLoginPassword)
            .NotEmpty()
            .WithMessage(
                "Confirm login password is required.")
            .Equal(x => x.LoginPassword)
            .WithMessage(
                "Login passwords do not match.");


        RuleFor(x => x.ProfilePassword)
            .NotEmpty()
            .WithMessage(
                "Profile password is required.")
            .MinimumLength(8)
            .WithMessage(
                "Profile password must be at least 8 characters.")
            .Matches("[A-Z]")
            .WithMessage(
                "Profile password must contain at least one uppercase letter.")
            .Matches("[a-z]")
            .WithMessage(
                "Profile password must contain at least one lowercase letter.")
            .Matches("[0-9]")
            .WithMessage(
                "Profile password must contain at least one number.")
            .Matches(@"[^a-zA-Z0-9]")
            .WithMessage(
                "Profile password must contain at least one special character.");

        RuleFor(x => x.ConfirmProfilePassword)
            .NotEmpty()
            .WithMessage(
                "Confirm profile password is required.")
            .Equal(x => x.ProfilePassword)
            .WithMessage(
                "Profile passwords do not match.");


        // =========================================
        // CUSTOMER VALIDATION
        // =========================================

        When(
            x => x.UserType == UserType.Customer,
            () =>
            {
                RuleFor(x => x.FirstName)
                    .NotEmpty()
                    .WithMessage(
                        "First name is required.")
                    .MaximumLength(50)
                    .WithMessage(
                        "First name cannot exceed 50 characters.");


                RuleFor(x => x.LastName)
                    .NotEmpty()
                    .WithMessage(
                        "Last name is required.")
                    .MaximumLength(50)
                    .WithMessage(
                        "Last name cannot exceed 50 characters.");


                RuleFor(x => x.Email)
                    .NotEmpty()
                    .WithMessage(
                        "Email is required.")
                    .EmailAddress()
                    .WithMessage(
                        "Enter a valid email address.");


                RuleFor(x => x.MobileNumber)
                    .NotEmpty()
                    .WithMessage(
                        "Mobile number is required.")
                    .Matches(@"^[6-9]\d{9}$")
                    .WithMessage(
                        "Enter a valid 10-digit mobile number.");


                RuleFor(x => x.DateOfBirth)
                    .NotNull()
                    .WithMessage(
                        "Date of birth is required.");


                RuleFor(x => x.DateOfBirth)
                    .Must(BeAtLeast18YearsOld)
                    .When(x => x.DateOfBirth.HasValue)
                    .WithMessage(
                        "Customer must be at least 18 years old.");


                RuleFor(x => x.DateOfBirth)
                    .LessThanOrEqualTo(DateTime.Today)
                    .When(x => x.DateOfBirth.HasValue)
                    .WithMessage(
                        "Date of birth cannot be in the future.");


                RuleFor(x => x.EmploymentType)
                    .IsInEnum()
                    .WithMessage(
                        "Select a valid employment type.");


                RuleFor(x => x.AnnualIncome)
                    .InclusiveBetween(0, 100_000_000)
                    .WithMessage(
                        "Enter a valid annual income.");


                RuleFor(x => x.AnnualIncome)
                    .GreaterThan(0)
                    .When(x =>
                        x.EmploymentType ==
                            EmploymentType.Salaried ||
                        x.EmploymentType ==
                            EmploymentType.SelfEmployed)
                    .WithMessage(
                        "Annual income is required.");


                RuleFor(x => x.AnnualIncome)
                    .Equal(0)
                    .When(x =>
                        x.EmploymentType ==
                            EmploymentType.Unemployed)
                    .WithMessage(
                        "Annual income must be zero for unemployed customers.");


                RuleFor(x => x.CreditScore)
                    .InclusiveBetween(300, 900)
                    .WithMessage(
                        "Credit score must be between 300 and 900.");


                RuleFor(x => x.KycDocuments)
                    .NotEmpty()
                    .WithMessage(
                        "Please add at least one KYC document.");
            });
    }


    private static bool BeAtLeast18YearsOld(
        DateTime? dateOfBirth)
    {
        if (!dateOfBirth.HasValue)
        {
            return false;
        }

        var today = DateTime.Today;

        var age =
            today.Year - dateOfBirth.Value.Year;

        if (dateOfBirth.Value.Date >
            today.AddYears(-age))
        {
            age--;
        }

        return age >= 18;
    }
}

