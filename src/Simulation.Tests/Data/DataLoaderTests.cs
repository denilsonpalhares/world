using World.Simulation.Data;

namespace World.Simulation.Tests.Data;

public sealed class DataLoaderTests : IDisposable
{
    private sealed record TestDefinition(string Id, int Value) : IDefinition;

    private readonly string _dir = Directory.CreateTempSubdirectory("world-data-tests-").FullName;

    public void Dispose() => Directory.Delete(_dir, recursive: true);

    private void WriteFile(string name, string json) => File.WriteAllText(Path.Combine(_dir, name), json);

    [Fact]
    public void LoadsDefinitionsFromAllFilesInAlphabeticalOrder()
    {
        WriteFile("b.json", """[{ "id": "b1", "value": 3 }]""");
        WriteFile("a.json", """
            // comentários e vírgula final são permitidos
            [
              { "id": "a1", "value": 1 },
              { "id": "a2", "value": 2 },
            ]
            """);

        var registry = DataLoader.LoadDefinitions<TestDefinition>(_dir);

        Assert.Equal(["a1", "a2", "b1"], registry.All.Select(d => d.Id));
        Assert.Equal(2, registry.Get("a2").Value);
        Assert.True(registry.TryGet("b1", out var b1));
        Assert.Equal(3, b1.Value);
    }

    [Fact]
    public void DuplicateIdsAcrossFilesThrow()
    {
        WriteFile("a.json", """[{ "id": "x", "value": 1 }]""");
        WriteFile("b.json", """[{ "id": "x", "value": 2 }]""");

        var ex = Assert.Throws<DataLoadException>(() => DataLoader.LoadDefinitions<TestDefinition>(_dir));
        Assert.Contains("'x'", ex.Message);
    }

    [Fact]
    public void UnknownPropertyThrows()
    {
        WriteFile("a.json", """[{ "id": "x", "value": 1, "valeu": 2 }]""");

        var ex = Assert.Throws<DataLoadException>(() => DataLoader.LoadDefinitions<TestDefinition>(_dir));
        Assert.Contains("a.json", ex.Message);
    }

    [Fact]
    public void MissingRequiredPropertyThrows()
    {
        WriteFile("a.json", """[{ "value": 1 }]""");

        Assert.Throws<DataLoadException>(() => DataLoader.LoadDefinitions<TestDefinition>(_dir));
    }

    [Fact]
    public void MissingDirectoryThrows()
    {
        Assert.Throws<DataLoadException>(() => DataLoader.LoadDefinitions<TestDefinition>(Path.Combine(_dir, "nope")));
    }

    [Fact]
    public void GetUnknownIdThrows()
    {
        WriteFile("a.json", """[{ "id": "x", "value": 1 }]""");
        var registry = DataLoader.LoadDefinitions<TestDefinition>(_dir);

        Assert.Throws<KeyNotFoundException>(() => registry.Get("y"));
    }

    [Fact]
    public void RepositoryConfigLoads()
    {
        var config = DataLoader.LoadObject<SimulationConfig>(Path.Combine(RepositoryDataDirectory(), "config", "simulation.json"));

        Assert.True(config.StartYear > 0);
    }

    private static string RepositoryDataDirectory()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, "data");
            if (File.Exists(Path.Combine(candidate, "config", "simulation.json")))
                return candidate;
        }
        throw new DirectoryNotFoundException("Pasta 'data' do repositório não encontrada.");
    }
}
