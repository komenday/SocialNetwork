using SQRS.Core.Events;

namespace SQRS.Core.Infrastructure;

public interface IEventStore
{
    Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedversion);
    Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId);
    Task<List<Guid>> GetAggregateIdsAsync();
}
