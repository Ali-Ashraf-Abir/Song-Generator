using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public static class CoverText
{
    public static void Draw(Image<Rgba32> image, int size, string title, string album,string artist)
    {
        DrawBottomVignette(image, size);

        float titleSize = size * 0.062f;
        float albumSize = size * 0.038f;
        float labelSize = size * 0.026f;

        var titleFont = GetFont(titleSize, FontStyle.Bold);
        var albumFont = GetFont(albumSize, FontStyle.Regular);
        var labelFont = GetFont(labelSize, FontStyle.Regular);

        const int pad = 28;
        int wrap = size - pad * 2;

        var white = new Rgba32(255, 255, 255, 240);
        var offWhite = new Rgba32(220, 220, 220, 210);
        var accent = new Rgba32(255, 210, 100, 230);
        var shadow = new Rgba32(0, 0, 0, 180);

        string eyebrowText = artist.ToUpperInvariant();
        if (eyebrowText.Length > 30) eyebrowText = eyebrowText[..30] + "…";

        var eyebrowOpts = new RichTextOptions(labelFont)
        {
            Origin = new PointF(pad, size - 122),
            WrappingLength = wrap,
        };

        var titleOpts = new RichTextOptions(titleFont)
        {
            Origin = new PointF(pad, size - 100),
            WrappingLength = wrap,
        };
        var titleShadow = new RichTextOptions(titleFont)
        {
            Origin = new PointF(pad + 2, size - 98),
            WrappingLength = wrap,
        };

        var albumOpts = new RichTextOptions(albumFont)
        {
            Origin = new PointF(pad, size - 42),
            WrappingLength = wrap,
        };
        var albumShadow = new RichTextOptions(albumFont)
        {
            Origin = new PointF(pad + 2, size - 40),
            WrappingLength = wrap,
        };

        image.Mutate(ctx => ctx.Paint(canvas =>
        {
            canvas.DrawText(eyebrowOpts, eyebrowText, Brushes.Solid(Color.FromPixel(accent)), pen: null);
            canvas.DrawLine(
                Pens.Solid(Color.FromPixel(accent), 2),
                new PointF(pad, size - 107),
                new PointF(size - pad, size - 107));
            canvas.DrawText(titleShadow, title, Brushes.Solid(Color.FromPixel(shadow)), pen: null);
            canvas.DrawText(titleOpts, title, Brushes.Solid(Color.FromPixel(white)), pen: null);

            canvas.DrawText(albumShadow, album, Brushes.Solid(Color.FromPixel(shadow)), pen: null);
            canvas.DrawText(albumOpts, album, Brushes.Solid(Color.FromPixel(offWhite)), pen: null);
        }));
    }

    private static void DrawBottomVignette(Image<Rgba32> image, int size)
    {
        int vignetteStart = (int)(size * 0.65f);

        for (int y = vignetteStart; y < size; y++)
        {
            float t = (y - vignetteStart) / (float)(size - vignetteStart);
            float alpha = t * t * 0.82f;
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

    private static Font GetFont(float size, FontStyle style)
    {
        string[] candidates = ["Liberation Sans", "DejaVu Sans", "Arial", "Helvetica", "FreeSans"];
        foreach (var name in candidates)
        {
            if (SystemFonts.TryGet(name, out var family))
                return family.CreateFont(size, style);
        }
        return SystemFonts.Families.First().CreateFont(size, style);
    }
}