namespace backend.Dtos;

/// <summary>
/// Parameters identifying exactly one previewable song. The same
/// (Seed, Page, Index) triple — combined the same way the existing
/// SongGenerationService combines them — must always resolve to the same
/// audio.
/// </summary>
public sealed class AudioPreviewRequest
{
    public long Seed { get; set; } = 1;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int Index { get; set; } = 1;
    public int DurationSeconds { get; set; } = 25;
}
