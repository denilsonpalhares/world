using System.Text.Json;
using System.Text.Json.Serialization;

namespace World.Simulation.Data;

public sealed class DataLoadException(string message, Exception? inner = null) : Exception(message, inner);

/// <summary>
/// Lê conteúdo de <c>/data</c>. Convenções dos arquivos JSON: propriedades em snake_case, comentários
/// e vírgulas finais permitidos, propriedades desconhecidas são erro (pega erros de digitação nos dados).
/// </summary>
public static class DataLoader
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        RespectNullableAnnotations = true,
        RespectRequiredConstructorParameters = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) },
    };

    /// <summary>Lê um único objeto de um arquivo JSON.</summary>
    public static T LoadObject<T>(string filePath)
    {
        if (!File.Exists(filePath))
            throw new DataLoadException($"Arquivo de dados não encontrado: {filePath}");

        return Deserialize<T>(filePath);
    }

    /// <summary>
    /// Lê todas as definições de um diretório: cada <c>*.json</c> contém um array de <typeparamref name="T"/>.
    /// Arquivos são lidos em ordem alfabética (ordinal) para manter o resultado determinístico.
    /// </summary>
    public static DefinitionRegistry<T> LoadDefinitions<T>(string directory) where T : IDefinition
    {
        if (!Directory.Exists(directory))
            throw new DataLoadException($"Diretório de dados não encontrado: {directory}");

        var files = Directory.GetFiles(directory, "*.json", SearchOption.TopDirectoryOnly);
        Array.Sort(files, StringComparer.Ordinal);

        var items = new List<T>();
        foreach (var file in files)
            items.AddRange(Deserialize<T[]>(file));

        return new DefinitionRegistry<T>(items);
    }

    private static T Deserialize<T>(string filePath)
    {
        try
        {
            using var stream = File.OpenRead(filePath);
            return JsonSerializer.Deserialize<T>(stream, JsonOptions)
                   ?? throw new DataLoadException($"Arquivo de dados vazio: {filePath}");
        }
        catch (JsonException ex)
        {
            throw new DataLoadException($"Erro em {filePath}: {ex.Message}", ex);
        }
    }
}
