using System.Text.Json;

namespace Q.FilterBuilder.IntegrationTests.Infrastructure;

/// <summary>
/// Service for loading test data from JSON files
/// </summary>
public class JsonTestDataLoader : IDisposable
{
    private readonly string _basePath;
    private readonly Dictionary<string, JsonDocument> _cache;

    public JsonTestDataLoader(string? basePath = null)
    {
        _basePath = basePath ?? FindJsonSamplesDirectory();
        _cache = new Dictionary<string, JsonDocument>();
    }

    private static string FindJsonSamplesDirectory()
    {
        // Start from the current assembly location
        var assemblyLocation = typeof(JsonTestDataLoader).Assembly.Location;
        var directory = new DirectoryInfo(Path.GetDirectoryName(assemblyLocation)!);

        // Navigate up to find the project root (where JsonSamples directory is located)
        while (directory != null)
        {
            var jsonSamplesPath = Path.Combine(directory.FullName, "JsonSamples");
            if (Directory.Exists(jsonSamplesPath))
            {
                return jsonSamplesPath;
            }

            // Also check if we're in the project root by looking for .csproj file
            if (directory.GetFiles("*.csproj").Length > 0)
            {
                var projectJsonSamplesPath = Path.Combine(directory.FullName, "JsonSamples");
                if (Directory.Exists(projectJsonSamplesPath))
                {
                    return projectJsonSamplesPath;
                }
            }

            directory = directory.Parent;
        }

        // Fallback to AppContext.BaseDirectory + JsonSamples
        return Path.Combine(AppContext.BaseDirectory, "JsonSamples");
    }

    /// <summary>
    /// Load JSON test data from a file
    /// </summary>
    /// <param name="fileName">Name of the JSON file (without extension)</param>
    /// <returns>JsonDocument containing the test data</returns>
    /// <exception cref="FileNotFoundException">Thrown when the JSON file is not found</exception>
    public JsonDocument LoadTestData(string fileName)
    {
        if (_cache.TryGetValue(fileName, out var cachedDocument))
        {
            return cachedDocument;
        }

        var filePath = Path.Combine(_basePath, $"{fileName}.json");

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Test data file not found: {filePath}");
        }

        var jsonContent = File.ReadAllText(filePath);
        var document = JsonDocument.Parse(jsonContent);

        _cache[fileName] = document;
        return document;
    }

    /// <summary>
    /// Load JSON test data as a string
    /// </summary>
    /// <param name="fileName">Name of the JSON file (without extension)</param>
    /// <returns>JSON content as string</returns>
    public string LoadTestDataAsString(string fileName)
    {
        var filePath = Path.Combine(_basePath, $"{fileName}.json");

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Test data file not found: {filePath}");
        }

        return File.ReadAllText(filePath);
    }

    /// <summary>
    /// Get all available test data file names
    /// </summary>
    /// <returns>Collection of available test data file names (without extensions)</returns>
    public IEnumerable<string> GetAvailableTestDataFiles()
    {
        if (!Directory.Exists(_basePath))
        {
            return Enumerable.Empty<string>();
        }

        return Directory.GetFiles(_basePath, "*.json")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => !string.IsNullOrEmpty(name))
            .Cast<string>();
    }

    /// <summary>
    /// Check if a test data file exists
    /// </summary>
    /// <param name="fileName">Name of the JSON file (without extension)</param>
    /// <returns>True if the file exists</returns>
    public bool TestDataExists(string fileName)
    {
        var filePath = Path.Combine(_basePath, $"{fileName}.json");
        return File.Exists(filePath);
    }

    /// <summary>
    /// Clear the cache
    /// </summary>
    public void ClearCache()
    {
        foreach (var document in _cache.Values)
        {
            document.Dispose();
        }
        _cache.Clear();
    }

    /// <summary>
    /// Dispose of all cached documents
    /// </summary>
    public void Dispose()
    {
        ClearCache();
    }
}
