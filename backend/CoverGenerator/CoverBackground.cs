using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


public static class CoverBackground
{
    public static void Draw(Image<Rgba32> image, int size, Random random)
    {
        int style = random.Next(6);
        switch (style)
        {
            case 0: DrawLinearGradient(image, size, random); break;
            case 1: DrawRadialGradient(image, size, random); break;
            case 2: DrawTriColorDiagonal(image, size, random); break;
            case 3: DrawNoiseTexture(image, size, random); break;
            case 4: DrawDuotone(image, size, random); break;
            default: DrawSunburst(image, size, random); break;
        }
    }

    private static void DrawLinearGradient(Image<Rgba32> image, int size, Random random)
    {
        var palette = RandomPalette(random);
        var c1 = palette[0];
        var c2 = palette[1];

        for (int y = 0; y < size; y++)
        {
            float t = y / (float)(size - 1);
            var px = Lerp(c1, c2, t);
            for (int x = 0; x < size; x++)
                image[x, y] = px;
        }
    }

    private static void DrawRadialGradient(Image<Rgba32> image, int size, Random random)
    {
        var palette = RandomPalette(random);
        var inner = palette[0];
        var outer = palette[1];

        float cx = size * (0.3f + (float)random.NextDouble() * 0.4f);
        float cy = size * (0.3f + (float)random.NextDouble() * 0.4f);
        float maxDist = size * 0.8f;

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float dist = MathF.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
            float t = Math.Clamp(dist / maxDist, 0f, 1f);

            t = t * t;
            image[x, y] = Lerp(inner, outer, t);
        }
    }

  
    private static void DrawTriColorDiagonal(Image<Rgba32> image, int size, Random random)
    {
        var palette = RandomPalette(random);
        var a = palette[0];
        var b = palette[1];
        var c = palette[2];

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float diag = (x + y) / (float)(size * 2);
            if (diag < 0.33f)      image[x, y] = a;
            else if (diag < 0.66f) image[x, y] = b;
            else                   image[x, y] = c;
        }
    }

  
    private static void DrawNoiseTexture(Image<Rgba32> image, int size, Random random)
    {
        var palette = RandomPalette(random);
        var c1 = palette[0];
        var c2 = palette[1];

        double fx = 2 + random.NextDouble() * 4;
        double fy = 2 + random.NextDouble() * 4;
        double fx2 = 3 + random.NextDouble() * 5;
        double fy2 = 3 + random.NextDouble() * 5;
        double ox  = random.NextDouble() * Math.PI * 2;
        double oy  = random.NextDouble() * Math.PI * 2;

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            double nx = x / (double)size;
            double ny = y / (double)size;
            double v = (Math.Sin(nx * fx * Math.PI + ox) +
                        Math.Sin(ny * fy * Math.PI + oy) +
                        Math.Sin((nx + ny) * fx2 * Math.PI) +
                        Math.Cos((nx - ny) * fy2 * Math.PI)) / 4.0;
            float t = (float)(v * 0.5 + 0.5);
            image[x, y] = Lerp(c1, c2, t);
        }
    }


    private static void DrawDuotone(Image<Rgba32> image, int size, Random random)
    {
        var palette = RandomPalette(random);
        var shadow = Darken(palette[0], 0.3f);
        var highlight = Lighten(palette[1], 0.3f);

        double freq = 3 + random.NextDouble() * 5;
        double angle = random.NextDouble() * Math.PI;

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            double proj = x * Math.Cos(angle) + y * Math.Sin(angle);
            double v = Math.Sin(proj / size * Math.PI * freq);
            float t = (float)(v * 0.5 + 0.5);
            image[x, y] = Lerp(shadow, highlight, t);
        }
    }

    private static void DrawSunburst(Image<Rgba32> image, int size, Random random)
    {
        var palette = RandomPalette(random);
        var a = palette[0];
        var b = palette[1];
        var c = palette[2];

        float cx = size / 2f;
        float cy = size / 2f;
        int rays = 8 + random.Next(12);

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            double angle = Math.Atan2(y - cy, x - cx) + Math.PI; 
            double sector = angle / (Math.PI * 2) * rays;
            int ray = (int)sector;
            float blendT = (float)(sector - ray);

      
            var colA = (ray % 2 == 0) ? a : b;
            var colB = (ray % 2 == 0) ? b : a;

  
            float edge = MathF.Abs(blendT - 0.5f) * 2f; 
            var base2 = Lerp(colB, colA, edge);

 
            float dist = MathF.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
            float radT = Math.Clamp(dist / (size * 0.6f), 0f, 1f);
            image[x, y] = Lerp(c, base2, radT);
        }
    }

    public static Rgba32[] RandomPalette(Random rng)
    {

        var palettes = new Rgba32[][]
        {
  
            [Hex(0xFF6B35), Hex(0xF7C59F), Hex(0xEFEFD0)],
 
            [Hex(0x0D1B2A), Hex(0x1B4F72), Hex(0x5DADE2)],

            [Hex(0x0D0221), Hex(0x650D89), Hex(0xE800FF)],
      
            [Hex(0x2D4A22), Hex(0x6B8F4E), Hex(0xC8B560)],
        
            [Hex(0xC2185B), Hex(0xFF8A65), Hex(0xFFF3E0)],
     
            [Hex(0x0A192F), Hex(0x172A45), Hex(0x64FFDA)],
         
            [Hex(0x1A1A2E), Hex(0xE94560), Hex(0xF5A623)],
      
            [Hex(0xFBC2EB), Hex(0xA6C1EE), Hex(0xFDDB92)],
        
            [Hex(0x1C1C1E), Hex(0x3A3A3C), Hex(0xAEAEB2)],
         
            [Hex(0xFFFF00), Hex(0xFF4500), Hex(0x1E1E1E)],
        
            [Hex(0x008080), Hex(0xFF6B6B), Hex(0xF7FFF7)],
      
            [Hex(0x3D0066), Hex(0x7B00D4), Hex(0xE040FB)],
        };

        return palettes[rng.Next(palettes.Length)];
    }

    public static Rgba32 Lerp(Rgba32 a, Rgba32 b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return new Rgba32(
            (byte)(a.R + (b.R - a.R) * t),
            (byte)(a.G + (b.G - a.G) * t),
            (byte)(a.B + (b.B - a.B) * t),
            255);
    }

    public static Rgba32 Darken(Rgba32 c, float amount) =>
        new((byte)(c.R * (1 - amount)), (byte)(c.G * (1 - amount)), (byte)(c.B * (1 - amount)), 255);

    public static Rgba32 Lighten(Rgba32 c, float amount) =>
        new((byte)(c.R + (255 - c.R) * amount), (byte)(c.G + (255 - c.G) * amount), (byte)(c.B + (255 - c.B) * amount), 255);

    private static Rgba32 Hex(uint rgb) =>
        new((byte)(rgb >> 16), (byte)(rgb >> 8 & 0xFF), (byte)(rgb & 0xFF), 255);
}