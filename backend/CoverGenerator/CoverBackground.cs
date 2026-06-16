using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public static class CoverBackground
{
    public static void Draw(Image<Rgba32> image, int size, Random random)
    {
        var c1 = new Rgba32(
            (byte)random.Next(256),
            (byte)random.Next(256),
            (byte)random.Next(256));
        var c2 = new Rgba32(
            (byte)random.Next(256),
            (byte)random.Next(256),
            (byte)random.Next(256));

        for (int y = 0; y < size; y++)
        {
            float t = y / (float)(size - 1);
            byte r = (byte)(c1.R + (c2.R - c1.R) * t);
            byte g = (byte)(c1.G + (c2.G - c1.G) * t);
            byte b = (byte)(c1.B + (c2.B - c1.B) * t);
            for (int x = 0; x < size; x++)
                image[x, y] = new Rgba32(r, g, b);
        }
    }
}