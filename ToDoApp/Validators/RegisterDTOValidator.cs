using FluentValidation;
using ToDoApp.DTOs;

namespace ToDoApp.Validators
{
    public class RegisterDTOValidator : AbstractValidator<RegistrationDTO>
    {

        public RegisterDTOValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid Email Format");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");

            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm password is required")
                .Equal(x => x.Password).WithMessage("Confirm password must match password.");
        }
    }
}
