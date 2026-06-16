using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public static class CoverText
{
    public static void Draw(Image<Rgba32> image, int size, string title, string album)
    {
        var titleFont = SystemFonts.CreateFont("Arial", 32, FontStyle.Bold);
        var albumFont = SystemFonts.CreateFont("Arial", 18, FontStyle.Regular);

        Color white = Color.FromPixel(new Rgba32(255, 255, 255, 230));
        Color subtitleTint = Color.FromPixel(new Rgba32(220, 220, 220, 200));
        Color shadow = Color.FromPixel(new Rgba32(0, 0, 0, 160));

        const int pad = 28;
        int wrap = size - pad * 2;

        var titleOpts = new RichTextOptions(titleFont)
        {
            Origin = new PointF(pad, size - 90),
            WrappingLength = wrap,
        };
        var titleShadowOpts = new RichTextOptions(titleFont)
        {
            Origin = new PointF(pad + 2, size - 88),
            WrappingLength = wrap,
        };
        var albumOpts = new RichTextOptions(albumFont)
        {
            Origin = new PointF(pad, size - 50),
            WrappingLength = wrap,
        };
        var albumShadowOpts = new RichTextOptions(albumFont)
        {
            Origin = new PointF(pad + 2, size - 48),
            WrappingLength = wrap,
        };

        image.Mutate(ctx => ctx.Paint(canvas =>
        {
            canvas.DrawText(titleShadowOpts, title, Brushes.Solid(shadow), pen: null);
            canvas.DrawText(titleOpts, title, Brushes.Solid(white), pen: null);
            canvas.DrawText(albumShadowOpts, album, Brushes.Solid(shadow), pen: null);
            canvas.DrawText(albumOpts, album, Brushes.Solid(subtitleTint), pen: null);
        }));
    }
}