namespace World.Simulation.Commands;

/// <summary>
/// Fila de comandos pendentes. Thread-safe para permitir que a UI enfileire enquanto a simulação roda
/// em outra thread; a ordem de execução é sempre a ordem de chegada.
/// </summary>
public sealed class CommandQueue
{
    private readonly Lock _lock = new();
    private List<ICommand> _pending = [];

    public int Count
    {
        get
        {
            lock (_lock)
                return _pending.Count;
        }
    }

    public void Enqueue(ICommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        lock (_lock)
            _pending.Add(command);
    }

    /// <summary>Remove e retorna todos os comandos pendentes, na ordem de chegada.</summary>
    internal List<ICommand> Drain()
    {
        lock (_lock)
        {
            var drained = _pending;
            _pending = [];
            return drained;
        }
    }
}
