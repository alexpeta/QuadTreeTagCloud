using Core.Cloud;
using Core.Geometry;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CloudTagWithQuadTree.Rasterizers
{
  /// <summary>
  /// The real font. Lays the word out with FormattedText, renders it once to an offscreen
  /// bitmap and keeps every pixel whose alpha clears the threshold. The TextBlock in the
  /// window uses the same typeface and size, so the mask's top-left is the TextBlock's.
  /// </summary>
  public class FormattedTextRasterizer : IGlyphRasterizer
  {
    private const byte ALPHA_THRESHOLD = 96;
    private readonly Typeface _typeface;

    public FormattedTextRasterizer(Typeface typeface)
    {
      if (typeface == null)
      {
        throw new ArgumentNullException("typeface");
      }
      _typeface = typeface;
    }

    public Mask Rasterize(string text, double fontSize)
    {
      var formatted = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, _typeface, fontSize, Brushes.Black);
      int width = (int)Math.Ceiling(formatted.WidthIncludingTrailingWhitespace);
      int height = (int)Math.Ceiling(formatted.Height);
      if (width <= 0 || height <= 0)
      {
        return new Mask(0, 0);
      }

      var visual = new DrawingVisual();
      using (DrawingContext context = visual.RenderOpen())
      {
        context.DrawText(formatted, new Point(0, 0));
      }
      var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
      bitmap.Render(visual);

      int stride = width * 4;
      var pixels = new byte[stride * height];
      bitmap.CopyPixels(pixels, stride, 0);

      var mask = new Mask(width, height);
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          if (pixels[y * stride + x * 4 + 3] >= ALPHA_THRESHOLD)
          {
            mask.Set(x, y);
          }
        }
      }
      return mask;
    }
  }
}
