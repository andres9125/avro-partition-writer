using Avro;
using Avro.File;
using Avro.Generic;

namespace AvroPartitionWriter.Tests;

public sealed class PartitionedAvroWriterTests
{
    private const string SchemaText = """
        {
          "type": "record",
          "name": "event",
          "fields": [
            { "name": "id", "type": "int" },
            { "name": "message", "type": "string" }
          ]
        }
        """;

    [Fact]
    public void Append_rotates_and_each_completed_part_is_readable()
    {
        var schema = (RecordSchema)Schema.Parse(SchemaText);
        var outputDirectory = Path.Combine(Path.GetTempPath(), $"avro-partition-writer-{Guid.NewGuid():N}");

        try
        {
            IReadOnlyList<string> writtenFiles;
            using (var writer = new PartitionedAvroWriter(schema, outputDirectory, "events", partSizeBytes: 700))
            {
                for (var index = 0; index < 25; index++)
                {
                    var record = new GenericRecord(schema);
                    record.Add("id", index);
                    record.Add("message", new string('x', 256));
                    writer.Append(record);
                }

                writtenFiles = writer.WrittenFiles;
            }

            Assert.True(writtenFiles.Count > 1);
            Assert.Equal("events.part1.avro", Path.GetFileName(writtenFiles[0]));
            foreach (var part in writtenFiles)
            {
                using var stream = File.OpenRead(part);
                using var reader = DataFileReader<GenericRecord>.OpenReader(stream);
                Assert.True(reader.HasNext());
                Assert.NotNull(reader.Next());
            }
        }
        finally
        {
            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, recursive: true);
            }
        }
    }

    [Fact]
    public void Constructor_rejects_a_non_positive_part_size()
    {
        var schema = (RecordSchema)Schema.Parse(SchemaText);

        Assert.Throws<ArgumentOutOfRangeException>(() => new PartitionedAvroWriter(schema, "output", "events", 0));
    }
}

