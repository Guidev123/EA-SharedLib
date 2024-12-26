using FluentValidation.Results;

namespace SharedLib.Domain.Messages.Integration
{
    public class ResponseMessage : Message
    {
        public ResponseMessage(ValidationResult validationResult) => ValidationResult = validationResult;
        public ValidationResult ValidationResult { get; set; }
    }
    public class ResponseMessage<T> : Message
    {
        public ResponseMessage(ValidationResult validationResult) => ValidationResult = validationResult;
        public ValidationResult ValidationResult { get; set; }
        public T? Data { get; set; }
    }
}
