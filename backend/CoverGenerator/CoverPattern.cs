using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public static class CoverPattern
{
    public static void Draw(Image<Rgba32> image, int size, Random random)
    {
        int patternType = random.Next(3);

        if (patternType == 0)
            DrawWaves(image, size, random);
        else if (patternType == 1)
            DrawCircles(image, size, random);
        else
            DrawSquares(image, size, random);
    }

    private static void DrawWaves(Image<Rgba32> image, int size, Random random)
    {
        int lineCount = 8 + random.Next(6);
        double freq = 0.015 + random.NextDouble() * 0.03;
        double amp = 20 + random.NextDouble() * 40;

        for (int l = 0; l < lineCount; l++)
        {
            float yBase = (size / (float)(lineCount + 1)) * (l + 1);
            double phase = random.NextDouble() * Math.PI * 2;
            int thickness = 1 + random.Next(4);
            var waveColor = new Rgba32(
                (byte)random.Next(256),
                (byte)random.Next(256),
                (byte)random.Next(256),
                (byte)(120 + random.Next(100)));

            for (int x = 0; x < size; x++)
            {
                int y = (int)(yBase + Math.Sin(x * freq + phase) * amp);
                for (int t = -thickness; t <= thickness; t++)
                {
                    int py = y + t;
                    if (py >= 0 && py < size)
                        image[x, py] = waveColor;
                }
            }
        }
    }

    private static void DrawCircles(Image<Rgba32> image, int size, Random random)
    {
        int circleCount = 10 + random.Next(8);

        for (int i = 0; i < circleCount; i++)
        {
            int cx = random.Next(size);
            int cy = random.Next(size);
            int radius = 20 + random.Next(120);
            int thickness = 1 + random.Next(5);
            var circleColor = new Rgba32(
                (byte)random.Next(256),
                (byte)random.Next(256),
                (byte)random.Next(256),
                (byte)(100 + random.Next(120)));

            for (int py = 0; py < size; py++)
            for (int px = 0; px < size; px++)
            {
                double dist = Math.Sqrt((px - cx) * (px - cx) + (py - cy) * (py - cy));
                if (dist >= radius - thickness && dist <= radius + thickness)
                    image[px, py] = circleColor;
            }
        }
    }

    private static void DrawSquares(Image<Rgba32> image, int size, Random random)
    {
        for (int i = 0; i < 25; i++)
        {
            int x = random.Next(size);
            int y = random.Next(size);
            int w = random.Next(20, 120);
            int h = random.Next(20, 120);
            var color = new Rgba32(
                (byte)random.Next(256),
                (byte)random.Next(256),
                (byte)random.Next(256),
                120);

            for (int py = y; py < Math.Min(y + h, size); py++)
            for (int px = x; px < Math.Min(x + w, size); px++)
                image[px, py] = color;
        }
    }
}