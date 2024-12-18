using MediatR;
using SharedLib.Domain.Responses;

namespace SharedLib.Domain.Messages
{
    public class Query<T, Y> : Message, IRequest<Response<Y>>
    {
        public DateTime Timestamp { get; private set; }
        protected Query() => Timestamp = DateTime.Now;

        public virtual bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}
