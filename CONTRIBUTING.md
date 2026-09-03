# Contributing

Thanks for contributing to Avro Partition Writer.

## Development

~~~powershell
dotnet restore tests/AvroPartitionWriter.Tests/AvroPartitionWriter.Tests.csproj
dotnet build src/AvroPartitionWriter/AvroPartitionWriter.csproj --configuration Release --no-restore
dotnet test tests/AvroPartitionWriter.Tests/AvroPartitionWriter.Tests.csproj --configuration Release --no-restore
~~~

## Guidelines

- Keep the library schema-agnostic and free of domain-specific mappings.
- Do not add database clients, credentials, sample production data, or network
  transports.
- Add tests for observable behavior changes.
- Preserve the guarantee that completed parts are independently readable AVRO
  OCF files.
- Submit changes through a focused pull request.

By submitting a contribution, you agree to license it under Apache-2.0.

