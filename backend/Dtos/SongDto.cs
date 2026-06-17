namespace backend.Dtos;
public class SongDto
{
    public int Index { get; set; }

    public string Title { get; set; }

    public string Artist { get; set; }

    public string Album { get; set; }

    public string Genre { get; set; }

    public int Likes { get; set; }
    public string CoverUrl { get; set; } = "";

    public string PreviewUrl { get; set; } = string.Empty;
}