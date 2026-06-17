using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public static class CoverGenerator
{
    public static byte[] Generate(int songSeed, string title, string album,string artist)
    {
        var seed = HashCode.Combine(songSeed, "cover-v2");
        var random = new Random(seed);

        const int size = 512;
        using var image = new Image<Rgba32>(size, size);

        CoverBackground.Draw(image, size, random);

    
        CoverPattern.Draw(image, size, random);

     
        DrawTopVignette(image, size);

        CoverText.Draw(image, size, title, album,artist);

        using var stream = new MemoryStream();
        image.SaveAsPng(stream);
        return stream.ToArray();
    }


    private static void DrawTopVignette(Image<Rgba32> image, int size)
    {
        int vignetteEnd = (int)(size * 0.15f);
        for (int y = 0; y < vignetteEnd; y++)
        {
            float t = 1f - y / (float)vignetteEnd;
            float alpha = t * t * 0.35f;
            for (int x = 0; x < size; x++)
            {
                var px = image[x, y];
                image[x, y] = new Rgba32(
                    (byte)(px.R * (1 - alpha)),
                    (byte)(px.G * (1 - alpha)),
                    (byte)(px.B * (1 - alpha)),
                    px.A);
            }
        }
    }
}