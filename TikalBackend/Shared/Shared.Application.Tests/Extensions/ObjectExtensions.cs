using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Application.Tests.Extensions;

public static class ObjectExtensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.Preserve
    };

    public static T DeepClone<T>(this T source)
    {
        if (source is null)
        {
            return default!;
        }

        var json = JsonSerializer.Serialize(source, Options);
        return JsonSerializer.Deserialize<T>(json, Options)!;
    }
}