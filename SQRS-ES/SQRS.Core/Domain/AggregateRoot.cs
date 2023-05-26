using SQRS.Core.Events;

namespace SQRS.Core.Domain;

public abstract class AggregateRoot
{
    private readonly List<BaseEvent> _changes = new();
    public Guid Id { get; protected set; }
    public int Version { get; set; } = -1;

    public IEnumerable<BaseEvent> GetUncommitedChanges()
    {
        return _changes;
    }

    public void MarkChangesAsCommited()
    {
        _changes.Clear();
    }

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach (var @event in events)
        {
            ApplyChange(@event, false);
        }
    }

    protected void RaiseEvent(BaseEvent @event)
    {
        ApplyChange(@event, true);
    }

    private void ApplyChange(BaseEvent @event, bool isNew)
    {
        var method = GetType().GetMethod("Apply", new Type[] { @event.GetType() }) 
            ?? throw new ArgumentNullException(nameof(@event), $"The 'Apply' method was not found in aggregate for {@event.GetType().Name}");

        method.Invoke(this, new object[] { @event });

        if (isNew)
        {
            _changes.Add(@event);
        }
    }
}
