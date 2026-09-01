using Core.Cloud;
using Core.Geometry;
using System;

namespace ConsoleTests
{
  /// <summary>
  /// A font with no letters: every word is a solid box 0.6em per character and 1em tall.
  /// Because the ink IS the bounding box, a cloud laid out with this rasterizer can be
  /// checked with nothing but rectangle arithmetic, which shares no code with Mask.
  /// </summary>
  public class BoxRasterizer : IGlyphRasterizer
  {
    public Mask Rasterize(string text, double fontSize)
    {
      int width = (int)Math.Ceiling(text.Length * fontSize * 0.6);
      int height = (int)Math.Ceiling(fontSize);
      var mask = new Mask(width, height);
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          mask.Set(x, y);
        }
      }
      return mask;
    }
  }
}
