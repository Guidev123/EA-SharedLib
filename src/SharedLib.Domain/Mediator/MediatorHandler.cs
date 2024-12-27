using MediatR;
using SharedLib.Domain.Messages;
using SharedLib.Domain.Responses;

namespace SharedLib.Domain.Mediator
{
    public class MediatorHandler(IMediator mediator) : IMediatorHandler
    {
        private readonly IMediator _mediator = mediator;
        public async Task<Response<T>> SendCommand<T>(T command) where T : Command<T> => await _mediator.Send(command);
        public async Task PublishEvent<T>(T events) where T : Event => await _mediator.Publish(events);
    }
}
