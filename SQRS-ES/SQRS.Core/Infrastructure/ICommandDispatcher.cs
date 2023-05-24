using SQRS.Core.Commands;

namespace SQRS.Core.Infrastructure;

public interface ICommandDispatcher
{
    void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand;
    Task SendAsync(BaseCommand command);
}
