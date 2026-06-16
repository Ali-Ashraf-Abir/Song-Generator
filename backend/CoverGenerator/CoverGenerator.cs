using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public static class CoverGenerator
{
    public static byte[] Generate(int songSeed, string title, string album)
    {
        var seed = HashCode.Combine(songSeed, "cover-v1");
        var random = new Random(seed);

        const int size = 512;
        using var image = new Image<Rgba32>(size, size);

        CoverBackground.Draw(image, size, random);
        CoverPattern.Draw(image, size, random);
        CoverText.Draw(image, size, title, album);

        using var stream = new MemoryStream();
        image.SaveAsPng(stream);
        return stream.ToArray();
    }
}