using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public static class CoverPattern
{
    public static void Draw(Image<Rgba32> image, int size, Random random)
    {
   
        int primary = random.Next(8);
        DrawPattern(image, size, random, primary);

     
        if (random.NextDouble() < 0.4)
        {
    
            int secondary = (primary + 1 + random.Next(7)) % 8;
            DrawPattern(image, size, random, secondary);
        }
    }

    private static void DrawPattern(Image<Rgba32> image, int size, Random random, int style)
    {
        switch (style)
        {
            case 0: DrawHalftone(image, size, random);      break;
            case 1: DrawGeometricGrid(image, size, random); break;
            case 2: DrawBokeh(image, size, random);         break;
            case 3: DrawFilmGrain(image, size, random);     break;
            case 4: DrawScanLines(image, size, random);     break;
            case 5: DrawVinylGrooves(image, size, random);  break;
            case 6: DrawSplatter(image, size, random);      break;
            default: DrawConstellation(image, size, random); break;
        }
    }


    private static void DrawHalftone(Image<Rgba32> image, int size, Random random)
    {
        var palette = CoverBackground.RandomPalette(random);
        var dotColor = WithAlpha(palette[random.Next(palette.Length)], (byte)(80 + random.Next(100)));
        int spacing = 12 + random.Next(14);
        float maxRadius = spacing * 0.48f;


        float cx = size / 2f, cy = size / 2f;

        for (int gy = 0; gy <= size / spacing + 1; gy++)
        for (int gx = 0; gx <= size / spacing + 1; gx++)
        {
            int px = gx * spacing;
            int py = gy * spacing;
            float dist = MathF.Sqrt((px - cx) * (px - cx) + (py - cy) * (py - cy));
            float density = 1f - Math.Clamp(dist / (size * 0.7f), 0f, 1f);
            float radius = maxRadius * (0.2f + density * 0.8f);

            FillCircle(image, size, px, py, (int)radius, dotColor);
        }
    }


    private static void DrawGeometricGrid(Image<Rgba32> image, int size, Random random)
    {
        var palette = CoverBackground.RandomPalette(random);
        int divisions = 3 + random.Next(3); // 3–5 divisions
        int cellSize = size / divisions;

        for (int row = 0; row < divisions; row++)
        for (int col = 0; col < divisions; col++)
        {

            if (random.NextDouble() < 0.45)
            {
                var color = WithAlpha(palette[random.Next(palette.Length)], 160);
                int x = col * cellSize;
                int y = row * cellSize;


                int inset = random.Next(2) * (4 + random.Next(8));
                FillRect(image, size, x + inset, y + inset,
                         cellSize - inset * 2, cellSize - inset * 2, color);
            }
        }


        var lineColor = WithAlpha(palette[0], 80);
        for (int i = 1; i < divisions; i++)
        {
            int pos = i * cellSize;
            DrawHLine(image, size, 0, pos, size, lineColor, 2);
            DrawVLine(image, size, pos, 0, size, lineColor, 2);
        }
    }


    private static void DrawBokeh(Image<Rgba32> image, int size, Random random)
    {
        var palette = CoverBackground.RandomPalette(random);
        int count = 15 + random.Next(20);

        for (int i = 0; i < count; i++)
        {
            int cx = random.Next(size);
            int cy = random.Next(size);
            int radius = 20 + random.Next(80);
            var col = palette[random.Next(palette.Length)];
            byte alpha = (byte)(30 + random.Next(70));

            DrawSoftCircle(image, size, cx, cy, radius, col, alpha);
        }
    }


    private static void DrawFilmGrain(Image<Rgba32> image, int size, Random random)
    {
        float intensity = 0.06f + (float)random.NextDouble() * 0.10f;
        int grainSize = 1 + random.Next(2);

        for (int y = 0; y < size; y += grainSize)
        for (int x = 0; x < size; x += grainSize)
        {
            int noise = (int)((random.NextDouble() - 0.5) * 255 * intensity);
            var px = image[x, y];
            var nx = new Rgba32(
                (byte)Math.Clamp(px.R + noise, 0, 255),
                (byte)Math.Clamp(px.G + noise, 0, 255),
                (byte)Math.Clamp(px.B + noise, 0, 255),
                px.A);

            for (int dy = 0; dy < grainSize && y + dy < size; dy++)
            for (int dx = 0; dx < grainSize && x + dx < size; dx++)
                image[x + dx, y + dy] = nx;
        }
    }

    private static void DrawScanLines(Image<Rgba32> image, int size, Random random)
    {
        bool horizontal = random.Next(2) == 0;
        int spacing = 3 + random.Next(5);
        byte darkness = (byte)(30 + random.Next(60));

        for (int i = 0; i < size; i += spacing)
        {
            for (int j = 0; j < size; j++)
            {
                int x = horizontal ? j : i;
                int y = horizontal ? i : j;
                if (x >= size || y >= size) continue;
                var px = image[x, y];
                image[x, y] = new Rgba32(
                    (byte)Math.Max(0, px.R - darkness),
                    (byte)Math.Max(0, px.G - darkness),
                    (byte)Math.Max(0, px.B - darkness),
                    px.A);
            }
        }
    }


    private static void DrawVinylGrooves(Image<Rgba32> image, int size, Random random)
    {
        float cx = size / 2f, cy = size / 2f;
        int grooveCount = 30 + random.Next(20);
        float maxR = size * 0.48f;
        float minR = size * 0.06f;
        byte alpha = (byte)(40 + random.Next(50));

        for (int i = 0; i < grooveCount; i++)
        {
            float t = i / (float)grooveCount;
            float r = minR + (maxR - minR) * t;
            // Alternate light / dark grooves
            byte bright = (byte)(i % 2 == 0 ? 255 : 0);
            var color = new Rgba32(bright, bright, bright, alpha);
            DrawRingPixels(image, size, (int)cx, (int)cy, (int)r, 1, color);
        }

        // Center label circle
        var labelCol = WithAlpha(CoverBackground.RandomPalette(random)[0], 200);
        FillCircle(image, size, (int)cx, (int)cy, (int)(size * 0.12f), labelCol);
    }


    private static void DrawSplatter(Image<Rgba32> image, int size, Random random)
    {
        var palette = CoverBackground.RandomPalette(random);
        int dropCount = 40 + random.Next(60);

        for (int i = 0; i < dropCount; i++)
        {
            int cx = random.Next(size);
            int cy = random.Next(size);
            int r = 2 + random.Next(28);
            var col = WithAlpha(palette[random.Next(palette.Length)], (byte)(100 + random.Next(155)));

            FillCircle(image, size, cx, cy, r, col);

           
            if (random.NextDouble() < 0.5)
            {
                int dripLen = r + random.Next(r * 2);
                int dx = random.Next(-1, 2);
                int dy = 1 + random.Next(3); 
                for (int s = 0; s < dripLen; s++)
                {
                    int nx = cx + dx * s;
                    int ny = cy + dy * s;
                    int nr = Math.Max(1, r - s / 3);
                    FillCircle(image, size, nx, ny, nr, col);
                }
            }
        }
    }

   
    private static void DrawConstellation(Image<Rgba32> image, int size, Random random)
    {
        int starCount = 80 + random.Next(120);
        var stars = new (int x, int y)[starCount];


        for (int i = 0; i < starCount; i++)
        {
            int x = random.Next(size);
            int y = random.Next(size);
            stars[i] = (x, y);
            int r = random.Next(3) == 0 ? 2 : 1;
            byte bright = (byte)(180 + random.Next(75));
            FillCircle(image, size, x, y, r, new Rgba32(bright, bright, bright, bright));
        }


        var lineCol = new Rgba32(255, 255, 255, 25);
        for (int i = 0; i < starCount; i++)
        for (int j = i + 1; j < starCount; j++)
        {
            float dist = MathF.Sqrt(
                MathF.Pow(stars[i].x - stars[j].x, 2) +
                MathF.Pow(stars[i].y - stars[j].y, 2));
            if (dist < size * 0.12f)
                DrawLineBresenham(image, size, stars[i].x, stars[i].y, stars[j].x, stars[j].y, lineCol);
        }
    }



    private static void FillCircle(Image<Rgba32> image, int size, int cx, int cy, int r, Rgba32 color)
    {
        for (int dy = -r; dy <= r; dy++)
        for (int dx = -r; dx <= r; dx++)
        {
            if (dx * dx + dy * dy > r * r) continue;
            int x = cx + dx, y = cy + dy;
            if (x >= 0 && x < size && y >= 0 && y < size)
                BlendPixel(image, x, y, color);
        }
    }


    private static void DrawSoftCircle(Image<Rgba32> image, int size, int cx, int cy, int r, Rgba32 color, byte maxAlpha)
    {
        for (int dy = -r; dy <= r; dy++)
        for (int dx = -r; dx <= r; dx++)
        {
            float dist = MathF.Sqrt(dx * dx + dy * dy);
            if (dist > r) continue;
            float falloff = 1f - (dist / r);
            falloff = falloff * falloff; // quadratic — bright center
            byte alpha = (byte)(maxAlpha * falloff);
            int x = cx + dx, y = cy + dy;
            if (x >= 0 && x < size && y >= 0 && y < size)
                BlendPixel(image, x, y, new Rgba32(color.R, color.G, color.B, alpha));
        }
    }

    private static void FillRect(Image<Rgba32> image, int size, int x, int y, int w, int h, Rgba32 color)
    {
        for (int py = y; py < Math.Min(y + h, size); py++)
        for (int px = x; px < Math.Min(x + w, size); px++)
            if (px >= 0 && py >= 0)
                BlendPixel(image, px, py, color);
    }

    private static void DrawHLine(Image<Rgba32> image, int size, int x, int y, int len, Rgba32 color, int thickness)
    {
        for (int t = -thickness / 2; t <= thickness / 2; t++)
        for (int i = 0; i < len; i++)
        {
            int px = x + i, py = y + t;
            if (px >= 0 && px < size && py >= 0 && py < size)
                BlendPixel(image, px, py, color);
        }
    }

    private static void DrawVLine(Image<Rgba32> image, int size, int x, int y, int len, Rgba32 color, int thickness)
    {
        for (int t = -thickness / 2; t <= thickness / 2; t++)
        for (int i = 0; i < len; i++)
        {
            int px = x + t, py = y + i;
            if (px >= 0 && px < size && py >= 0 && py < size)
                BlendPixel(image, px, py, color);
        }
    }

    private static void DrawRingPixels(Image<Rgba32> image, int size, int cx, int cy, int r, int thickness, Rgba32 color)
    {
        for (int dy = -(r + thickness); dy <= r + thickness; dy++)
        for (int dx = -(r + thickness); dx <= r + thickness; dx++)
        {
            float dist = MathF.Sqrt(dx * dx + dy * dy);
            if (dist < r - thickness || dist > r + thickness) continue;
            int x = cx + dx, y = cy + dy;
            if (x >= 0 && x < size && y >= 0 && y < size)
                BlendPixel(image, x, y, color);
        }
    }

    private static void DrawLineBresenham(Image<Rgba32> image, int size, int x0, int y0, int x1, int y1, Rgba32 color)
    {
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;
        while (true)
        {
            if (x0 >= 0 && x0 < size && y0 >= 0 && y0 < size)
                BlendPixel(image, x0, y0, color);
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
    }


    private static void BlendPixel(Image<Rgba32> image, int x, int y, Rgba32 src)
    {
        if (src.A == 0) return;
        if (src.A == 255) { image[x, y] = src; return; }

        var dst = image[x, y];
        float a = src.A / 255f;
        float ia = 1f - a;
        image[x, y] = new Rgba32(
            (byte)(src.R * a + dst.R * ia),
            (byte)(src.G * a + dst.G * ia),
            (byte)(src.B * a + dst.B * ia),
            255);
    }

    private static Rgba32 WithAlpha(Rgba32 c, byte alpha) => new(c.R, c.G, c.B, alpha);
}