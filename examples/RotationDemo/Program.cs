using Avro;
using Avro.File;
using Avro.Generic;
using AvroPartitionWriter;

const int recordCount = 6;
var outputDirectory = Path.Combine(Path.GetTempPath(), $"avro-rotation-demo-{Guid.NewGuid():N}");
var schema = (RecordSchema)Schema.Parse("""
    {
      "type": "record",
      "name": "event",
      "fields": [
        { "name": "id", "type": "int" },
        { "name": "message", "type": "string" }
      ]
    }
    """);

var writer = new PartitionedAvroWriter(schema, outputDirectory, "events", partSizeBytes: 700);
using (writer)
{
    for (var id = 1; id <= recordCount; id++)
    {
        var record = new GenericRecord(schema);
        record.Add("id", id);
        record.Add("message", new string('x', 256));
        writer.Append(record);
    }
}

var readIds = new List<int>();
foreach (var file in writer.WrittenFiles)
{
    using var stream = File.OpenRead(file);
    using var reader = DataFileReader<GenericRecord>.OpenReader(stream);
    var firstRecord = readIds.Count;
    while (reader.HasNext())
    {
        readIds.Add((int)reader.Next()["id"]);
    }

    Console.WriteLine($"{Path.GetFileName(file)}: {new FileInfo(file).Length} bytes, IDs {string.Join(", ", readIds.Skip(firstRecord))}");
}

if (writer.WrittenFiles.Count < 2 || !readIds.SequenceEqual(Enumerable.Range(1, recordCount)))
{
    throw new InvalidOperationException("Expected multiple parts containing every original record in order.");
}

Console.WriteLine($"Verified {readIds.Count} records across {writer.WrittenFiles.Count} independently readable parts.");
Console.WriteLine($"Output: {outputDirectory}");
