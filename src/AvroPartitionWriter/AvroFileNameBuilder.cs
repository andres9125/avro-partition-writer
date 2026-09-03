using System.Globalization;

namespace AvroPartitionWriter;

/// <summary>Builds safe, predictable names for AVRO output parts.</summary>
public static class AvroFileNameBuilder
{
    public const string DefaultTemplate = "{prefix}.part{part}.avro";

    public static string Build(string prefix, int partNumber, string? template = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
        if (partNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(partNumber), "Part number must be at least one.");
        }

        var selectedTemplate = string.IsNullOrWhiteSpace(template) ? DefaultTemplate : template;
        var fileName = selectedTemplate
            .Replace("{prefix}", prefix, StringComparison.Ordinal)
            .Replace("{part}", partNumber.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);

        if (string.IsNullOrWhiteSpace(fileName) || !string.Equals(fileName, Path.GetFileName(fileName), StringComparison.Ordinal))
        {
            throw new ArgumentException("The file name template must resolve to a single file name.", nameof(template));
        }

        return fileName;
    }
}

