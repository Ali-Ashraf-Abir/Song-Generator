namespace backend.Dtos;
public class SongGenerationRequest
{
    public string Locale { get; set; } = "en-US";

    public long Seed { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public double AvgLikes { get; set; }
}