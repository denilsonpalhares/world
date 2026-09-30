namespace World.Simulation.Data;

/// <summary>Conteúdo definido em <c>/data</c> (lei, recurso, profissão...), identificado por um ID único.</summary>
public interface IDefinition
{
    string Id { get; }
}
