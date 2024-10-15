using System.Reflection;

namespace Shared.Tests;

public class EmbeddedResourceHelper
{
    public static string GetEmbeddedResourceContent(string resourceName)
    {
        var assembly = Assembly.GetCallingAssembly();
        using Stream stream = assembly!.GetManifestResourceStream(resourceName)!;
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}