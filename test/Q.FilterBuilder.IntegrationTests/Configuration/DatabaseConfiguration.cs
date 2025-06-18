namespace Q.FilterBuilder.IntegrationTests.Configuration;

/// <summary>
/// Configuration model for database provider settings
/// </summary>
public class DatabaseConfiguration
{
    public string ImageName { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Dictionary<string, string> Environment { get; set; } = new();
}
