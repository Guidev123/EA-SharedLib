using FluentValidation;
using FluentValidation.Results;

namespace SharedLib.Domain.Messages
{
    public abstract class CommandHandler
    {
        protected static void AddError(ValidationResult validationResult, string message) =>
            validationResult.Errors.Add(new ValidationFailure(string.Empty, message));

        protected static string[] GetAllErrors(ValidationResult validationResult) =>
            validationResult.Errors.Select(e => e.ErrorMessage).ToArray();

        protected static ValidationResult ValidateEntity<TV, TE>(
            TV validation, TE entity) where TV
        : AbstractValidator<TE> where TE : class => validation.Validate(entity);
    }
}
