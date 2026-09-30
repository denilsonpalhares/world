namespace World.Simulation.Commands;

/// <summary>
/// Única forma de o jogador (ou a IA) alterar o mundo. Comandos são enfileirados e executados no início
/// do próximo tick, na ordem em que foram enfileirados. Devem ser imutáveis (preferir <c>record</c>) e
/// tolerar estados em que não se aplicam mais (ex.: nação que deixou de existir).
/// </summary>
public interface ICommand
{
    void Execute(WorldState world);
}
