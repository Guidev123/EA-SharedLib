using SharedLib.Domain.Messages;

namespace SharedLib.Domain.DomainObjects
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        private List<Event> _notifications = [];
        public IReadOnlyCollection<Event>? Notifications => _notifications.AsReadOnly();
        public void AddEvent(Event events)
        {
            _notifications = _notifications ?? [];
            _notifications.Add(events);
        }
        public void RemoveEvent(Event eventItem) => _notifications?.Remove(eventItem);
        public void ClearEvents() => _notifications?.Clear();
        public static bool operator !=(Entity a, Entity b) => !(a == b);
        public override int GetHashCode() => GetType().GetHashCode() * 907 + Id.GetHashCode();
        public override string ToString() => $"{GetType().Name} [Id ={Id}";
        public override bool Equals(object? obj)
        {
            var compareTo = obj as Entity;

            if (ReferenceEquals(this, compareTo)) return true;
            if (compareTo is null) return false;

            return Id.Equals(compareTo.Id);
        }
        public static bool operator ==(Entity a, Entity b)
        {
            if (a is null && b is null)
                return true;
            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }
    }
}
