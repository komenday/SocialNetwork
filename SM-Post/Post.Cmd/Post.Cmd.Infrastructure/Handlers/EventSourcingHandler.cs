using Post.Cmd.Domain.Aggregates;
using SQRS.Core.Domain;
using SQRS.Core.Handlers;
using SQRS.Core.Infrastructure;
using SQRS.Core.Producers;

namespace Post.Cmd.Infrastructure.Handlers;

public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStore _eventStore;
    private readonly IEventProducer _eventProducer;

    public EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer)
    {
        _eventStore = eventStore;
        _eventProducer = eventProducer;
    }

    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
        var aggregate = new PostAggregate();
        var events = await _eventStore.GetEventsAsync(aggregateId);

        if (events is null || !events.Any()) 
        {
            return aggregate;
        }

        aggregate.ReplayEvents(events);
        aggregate.Version = events.Select(e => e.Version).Max();

        return aggregate;
    }

    public async Task SaveAsync(AggregateRoot aggregate)
    {
        await _eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommitedChanges(), aggregate.Version);
        aggregate.MarkChangesAsCommited();
    }

    public async Task RepublishEventsAsync()
    {
        var aggregateIds = await _eventStore.GetAggregateIdsAsync();

        if (aggregateIds is null || !aggregateIds.Any()) return;

        foreach (var aggregateId in aggregateIds)
        {
            var aggregate = await GetByIdAsync(aggregateId);

            if (aggregate is null || !aggregate.Active) continue;

            var events = await _eventStore.GetEventsAsync(aggregateId);
            foreach (var @event in events)
            {
                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC")!;
                await _eventProducer.ProduceAsync(topic, @event);
            }
        }
    }
}
