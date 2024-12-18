using FluentValidation.Results;
using MediatR;
using SharedLib.Domain.Responses;
using System.Text.Json.Serialization;

namespace SharedLib.Domain.Messages
{
    public abstract class Command<T> : Message, IRequest<Response<T>>
    {
        public DateTime Timestamp { get; private set; }
        [JsonIgnore]
        public ValidationResult ValidationResult { get; set; } = new ValidationResult();
        protected Command() => Timestamp = DateTime.Now;

        public virtual bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}
