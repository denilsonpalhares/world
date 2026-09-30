using System.Diagnostics;
using World.Simulation;
using World.Simulation.Core;
using World.Simulation.Data;
using World.Simulation.Systems;

// Uso: dotnet run --project tools/Headless -- [--seed N] [--years N] [--data caminho]
ulong seed = 1;
var years = 10;
string? dataDir = null;

for (var i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--seed":
            seed = ulong.Parse(args[++i]);
            break;
        case "--years":
            years = int.Parse(args[++i]);
            break;
        case "--data":
            dataDir = args[++i];
            break;
        default:
            Console.Error.WriteLine($"Argumento desconhecido: {args[i]}");
            Console.Error.WriteLine("Uso: Headless [--seed N] [--years N] [--data caminho]");
            return 1;
    }
}

dataDir ??= FindDataDirectory();
var config = DataLoader.LoadObject<SimulationConfig>(Path.Combine(dataDir, "config", "simulation.json"));

var world = new WorldState(seed, config.StartYear);
var simulator = new Simulator(world, SystemPipeline.CreateDefault());

Console.WriteLine($"{SimulationInfo.Name} v{SimulationInfo.Version} (headless)");
Console.WriteLine($"Dados: {dataDir}");
Console.WriteLine($"Seed: {seed} | Início: {world.Date} | Simulando {years} ano(s)...");

var stopwatch = Stopwatch.StartNew();
simulator.Run((long)years * GameDate.DaysPerYear);
stopwatch.Stop();

Console.WriteLine($"Fim: {world.Date} | Ticks: {world.Tick} | Tempo: {stopwatch.Elapsed.TotalMilliseconds:F1} ms");
Console.WriteLine($"Checksum: {world.ComputeChecksum():X16}");
return 0;

// Procura a pasta "data" subindo a partir do diretório atual (funciona de qualquer lugar do repositório).
static string FindDataDirectory()
{
    for (var dir = new DirectoryInfo(Directory.GetCurrentDirectory()); dir is not null; dir = dir.Parent)
    {
        var candidate = Path.Combine(dir.FullName, "data");
        if (File.Exists(Path.Combine(candidate, "config", "simulation.json")))
            return candidate;
    }
    throw new DirectoryNotFoundException("Pasta 'data' não encontrada. Use --data <caminho>.");
}
