namespace SQRS.Core.Consumers;

public interface IEventConsumer
{
    void Consume(string topic);
}
