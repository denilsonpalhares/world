using System.Diagnostics.CodeAnalysis;

namespace World.Simulation.Data;

/// <summary>
/// Conjunto imutável de definições de um tipo. <see cref="All"/> preserva a ordem de carregamento
/// (arquivos em ordem alfabética, itens na ordem do arquivo), o que mantém a iteração determinística.
/// </summary>
public sealed class DefinitionRegistry<T> where T : IDefinition
{
    private readonly T[] _items;
    private readonly Dictionary<string, T> _byId;

    public DefinitionRegistry(IEnumerable<T> items)
    {
        _items = items.ToArray();
        _byId = new Dictionary<string, T>(_items.Length, StringComparer.Ordinal);
        foreach (var item in _items)
        {
            if (string.IsNullOrWhiteSpace(item.Id))
                throw new DataLoadException($"{typeof(T).Name} sem id.");
            if (!_byId.TryAdd(item.Id, item))
                throw new DataLoadException($"{typeof(T).Name} com id duplicado: '{item.Id}'.");
        }
    }

    public IReadOnlyList<T> All => _items;

    public int Count => _items.Length;

    public T Get(string id) =>
        _byId.TryGetValue(id, out var item)
            ? item
            : throw new KeyNotFoundException($"{typeof(T).Name} não encontrado: '{id}'.");

    public bool TryGet(string id, [MaybeNullWhen(false)] out T item) => _byId.TryGetValue(id, out item);
}
