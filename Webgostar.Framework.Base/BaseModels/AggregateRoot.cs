using System.ComponentModel.DataAnnotations.Schema;

namespace Webgostar.Framework.Base.BaseModels
{
    public abstract class AggregateRoot : BaseEntity
    {
        [NotMapped]
        public List<BaseDomainEvent> DomainEvents { get; } = [];

        protected void AddDomainEvent(BaseDomainEvent eventItem)
        {
            DomainEvents.Add(eventItem);
        }

        protected void RemoveDomainEvent(BaseDomainEvent eventItem)
        {
            DomainEvents?.Remove(eventItem);
        }
    }
}