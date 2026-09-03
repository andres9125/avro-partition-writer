# Third-party notices

## Runtime dependency

| Dependency | Version range | Purpose | License |
| --- | --- | --- | --- |
| [Apache.Avro](https://www.nuget.org/packages/Apache.Avro) | 1.12.1 | AVRO schema, generic record, and OCF encoding support. | Apache-2.0 |

Apache Avro's license and notice files are distributed with its package. This
repository does not vendor Apache Avro source code.

## Test-only dependencies

| Dependency | Version | Purpose | License |
| --- | --- | --- | --- |
| Microsoft.NET.Test.Sdk | 17.11.1 | Test host and discovery. | MIT |
| xunit | 2.9.2 | Unit-test framework. | Apache-2.0 |
| xunit.runner.visualstudio | 2.8.2 | Test runner integration. | Apache-2.0 |

These packages are not runtime dependencies of the library and are restored
only for development and continuous integration.
