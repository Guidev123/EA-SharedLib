using MediatR;
using SharedLib.Domain.Messages;

namespace SharedLib.Domain.DomainObjects
{
    public class Event : Message, INotification
    {
        public DateTime Timestamp { get; private set; }

        public Event()
        {
            Timestamp = DateTime.Now;
        }
    }
}
