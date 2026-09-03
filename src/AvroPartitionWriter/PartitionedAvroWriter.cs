using Avro;
using Avro.File;
using Avro.Generic;

namespace AvroPartitionWriter;

/// <summary>
/// Writes generic AVRO records into independently readable OCF files that
/// rotate after a byte threshold.
/// </summary>
public sealed class PartitionedAvroWriter : IDisposable
{
    private readonly RecordSchema _schema;
    private readonly string _outputDirectory;
    private readonly string _filePrefix;
    private readonly long _partSizeBytes;
    private readonly string? _fileNameTemplate;
    private readonly List<string> _writtenFiles = [];

    private IFileWriter<GenericRecord>? _writer;
    private FileStream? _stream;
    private int _partNumber;
    private bool _disposed;

    public PartitionedAvroWriter(
        RecordSchema schema,
        string outputDirectory,
        string filePrefix,
        long partSizeBytes,
        string? fileNameTemplate = null)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePrefix);
        if (partSizeBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(partSizeBytes), "Part size must be positive.");
        }

        _schema = schema;
        _outputDirectory = outputDirectory;
        _filePrefix = filePrefix;
        _partSizeBytes = partSizeBytes;
        _fileNameTemplate = fileNameTemplate;
        Directory.CreateDirectory(_outputDirectory);
    }

    /// <summary>Gets completed part paths in creation order.</summary>
    public IReadOnlyList<string> WrittenFiles => _writtenFiles.AsReadOnly();

    /// <summary>Appends a record and rotates only after the record is complete.</summary>
    public void Append(GenericRecord record)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(record);

        EnsureWriter();
        _writer!.Append(record);
        _writer.Flush();
        _stream!.Flush(flushToDisk: true);

        if (_stream.Position >= _partSizeBytes)
        {
            CloseCurrentPart();
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        CloseCurrentPart();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private void EnsureWriter()
    {
        if (_writer is not null)
        {
            return;
        }

        var fileName = AvroFileNameBuilder.Build(_filePrefix, ++_partNumber, _fileNameTemplate);
        var destination = Path.Combine(_outputDirectory, fileName);
        _stream = new FileStream(destination, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
        _writer = DataFileWriter<GenericRecord>.OpenWriter(new GenericDatumWriter<GenericRecord>(_schema), _stream);
        _writtenFiles.Add(destination);
    }

    private void CloseCurrentPart()
    {
        _writer?.Close();
        _writer = null;
        _stream?.Dispose();
        _stream = null;
    }
}
