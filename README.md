# Avro Partition Writer

[![Validation](https://github.com/andres9125/avro-partition-writer/actions/workflows/validation.yml/badge.svg)](https://github.com/andres9125/avro-partition-writer/actions/workflows/validation.yml)
[![License](https://img.shields.io/badge/license-Apache--2.0-blue.svg)](LICENSE)

Avro Partition Writer is a focused .NET library for writing Apache Avro Object
Container Files (OCF) in size-bounded parts. It accepts `GenericRecord`
instances, flushes each record, and closes the current file once the configured
threshold is reached. The next record starts a new valid AVRO file.

It is intended for batch exports that must avoid a single oversized output
file while keeping every produced part independently readable.

## What it does

- Writes `GenericRecord` values to standard AVRO OCF files.
- Rotates files after a configurable byte threshold.
- Produces deterministic file names from a prefix and a safe template.
- Exposes the list of completed files after disposal.
- Works with caller-provided schemas and records.

## What it does not do

- It does not connect to a database, build SQL queries, or map domain rows.
- It does not own, infer, validate, or distribute your AVRO schema.
- It does not send data over the network.
- It is not an asynchronous pipeline or a schema-registry client.

## Install from source

This first public release is source-first; it is not published to NuGet.
Clone the repository and add a project reference to `src/AvroPartitionWriter`.

~~~powershell
dotnet restore tests/AvroPartitionWriter.Tests/AvroPartitionWriter.Tests.csproj
dotnet test tests/AvroPartitionWriter.Tests/AvroPartitionWriter.Tests.csproj
~~~

## Example

~~~csharp
using Avro;
using Avro.Generic;
using AvroPartitionWriter;

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

using var writer = new PartitionedAvroWriter(
    schema,
    outputDirectory: "./artifacts",
    filePrefix: "events",
    partSizeBytes: 50 * 1024 * 1024);

var record = new GenericRecord(schema);
record.Add("id", 1);
record.Add("message", "hello");
writer.Append(record);
~~~

The default names are `events.part1.avro`, `events.part2.avro`, and so on. Use
`fileNameTemplate` to customize them with the `{prefix}` and `{part}` tokens.
Templates must resolve to a single file name; directory traversal is rejected.

## Rotation behavior

A record is always completed before a file rotates. Therefore a part can be
slightly larger than `partSizeBytes` when one record itself crosses the
threshold. Every closed part remains a valid AVRO OCF file.

## Development

~~~powershell
dotnet restore tests/AvroPartitionWriter.Tests/AvroPartitionWriter.Tests.csproj
dotnet build src/AvroPartitionWriter/AvroPartitionWriter.csproj --configuration Release --no-restore
dotnet test tests/AvroPartitionWriter.Tests/AvroPartitionWriter.Tests.csproj --configuration Release --no-restore
~~~

See [CONTRIBUTING.md](CONTRIBUTING.md) and [SECURITY.md](SECURITY.md) before
opening a pull request.

## License

Copyright 2026 Eximus. Licensed under [Apache-2.0](LICENSE). Dependency
attribution is in [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).

