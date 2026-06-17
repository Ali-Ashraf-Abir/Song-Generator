namespace backend.MusicEngine.Rendering;

/// <summary>
/// Wraps a stream so that Dispose/Close on this wrapper does not close
/// the underlying stream. NAudio's <see cref="NAudio.Wave.WaveFileWriter"/>
/// closes whatever stream it's given when disposed; wrapping the target
/// <see cref="MemoryStream"/> in this lets us flush/finalize the WAV
/// header via the writer's Dispose while still reading the bytes back out
/// of the original MemoryStream afterward.
/// </summary>
public sealed class IgnoreDisposeStream : Stream
{
    private readonly Stream _inner;

    public IgnoreDisposeStream(Stream inner)
    {
        _inner = inner;
    }

    public override bool CanRead => _inner.CanRead;
    public override bool CanSeek => _inner.CanSeek;
    public override bool CanWrite => _inner.CanWrite;
    public override long Length => _inner.Length;

    public override long Position
    {
        get => _inner.Position;
        set => _inner.Position = value;
    }

    public override void Flush() => _inner.Flush();
    public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
    public override long Seek(long offset, SeekOrigin origin) => _inner.Seek(offset, origin);
    public override void SetLength(long value) => _inner.SetLength(value);
    public override void Write(byte[] buffer, int offset, int count) => _inner.Write(buffer, offset, count);

    protected override void Dispose(bool disposing)
    {
        // Deliberately do not dispose _inner — that's the whole point of this wrapper.
    }
}
