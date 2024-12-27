using SharedLib.Domain.Messages;
using SharedLib.Domain.Responses;

namespace SharedLib.Domain.Mediator
{
    public interface IMediatorHandler
    {
        Task PublishEvent<T>(T events) where T : Event;
        Task<Response<T>> SendCommand<T>(T command) where T : Command<T>;
    }
}
