using SQRS.Core.Events;

namespace SQRS.Core.Producers;

public interface IEventProducer
{
    Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent;
}
