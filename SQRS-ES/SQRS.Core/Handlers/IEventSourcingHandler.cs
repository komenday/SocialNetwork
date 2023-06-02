using SQRS.Core.Domain;

namespace SQRS.Core.Handlers;

public interface IEventSourcingHandler<T>
{
    Task<T> GetByIdAsync(Guid aggregateId);
    Task SaveAsync(AggregateRoot aggregate);
    Task RepublishEventsAsync();
}
