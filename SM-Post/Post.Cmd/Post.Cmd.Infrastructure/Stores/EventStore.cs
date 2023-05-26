using Post.Cmd.Domain.Aggregates;
using SQRS.Core.Domain;
using SQRS.Core.Events;
using SQRS.Core.Exceptions;
using SQRS.Core.Infrastructure;

namespace Post.Cmd.Infrastructure.Stores;

public class EventStore : IEventStore
{
    private readonly IEventStoreRepository _eventStoreRepository;

    public EventStore(IEventStoreRepository eventStorerepository)
    {
        _eventStoreRepository = eventStorerepository;
    }

    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

        if (eventStream is null || !eventStream.Any())
        {
            throw new AggregateNotFoundException("Incorrect post ID provided");
        }

        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
    }

    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedversion)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

        if (expectedversion != -1 && eventStream[^1].Version != expectedversion)
        {
            throw new ConcurrencyException();
        }

        var version = expectedversion;

        foreach (var @event in events)
        {
            version++;
            @event.Version = version;
            var eventModel = new EventModel
            {
                TimeStamp = DateTime.Now,
                Version = version,
                AggregateIdentifier = aggregateId,
                AggregateType = nameof(PostAggregate),
                EventData = @event,
                EventType = @event.GetType().Name
            };

            await _eventStoreRepository.SaveAsync(eventModel);
        }
    }
}
