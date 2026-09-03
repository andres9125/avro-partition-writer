namespace AvroPartitionWriter.Tests;

public sealed class AvroFileNameBuilderTests
{
    [Fact]
    public void Build_uses_the_default_template()
    {
        var name = AvroFileNameBuilder.Build("events", 4);

        Assert.Equal("events.part4.avro", name);
    }

    [Fact]
    public void Build_allows_a_custom_template()
    {
        var name = AvroFileNameBuilder.Build("events", 4, "batch-{prefix}-{part}.ocf");

        Assert.Equal("batch-events-4.ocf", name);
    }

    [Fact]
    public void Build_rejects_a_path_in_the_template()
    {
        Assert.Throws<ArgumentException>(() => AvroFileNameBuilder.Build("events", 1, "../{prefix}.avro"));
    }
}

