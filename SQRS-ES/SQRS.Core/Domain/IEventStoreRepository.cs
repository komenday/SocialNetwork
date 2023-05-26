using SQRS.Core.Events;

namespace SQRS.Core.Domain;
public interface IEventStoreRepository
{
    Task<List<EventModel>> FindByAggregateId(Guid aggregateId);
    Task SaveAsync(EventModel @event);
}
