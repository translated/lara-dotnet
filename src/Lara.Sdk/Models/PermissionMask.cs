using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lara.Sdk;

/// Resource permissions in read, write, export, share order; a dash means absent.
[JsonConverter(typeof(PermissionMaskJsonConverter))]
public sealed record PermissionMask(string Value)
{
    public static readonly PermissionMask Read = new("r---");
    public static readonly PermissionMask ReadWrite = new("rw--");
    public static readonly PermissionMask ReadExport = new("r-e-");
    public static readonly PermissionMask ReadShare = new("r--s");
    public static readonly PermissionMask ReadWriteExport = new("rwe-");
    public static readonly PermissionMask ReadWriteShare = new("rw-s");
    public static readonly PermissionMask ReadExportShare = new("r-es");
    public static readonly PermissionMask ReadWriteExportShare = new("rwes");

    public override string ToString() => Value;
}

internal sealed class PermissionMaskJsonConverter : JsonConverter<PermissionMask>
{
    public override PermissionMask Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new PermissionMask(reader.GetString() ?? throw new JsonException("Expected a permission mask."));
    }

    public override void Write(Utf8JsonWriter writer, PermissionMask value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
