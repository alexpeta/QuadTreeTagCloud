using Core.Geometry;

namespace Core.Cloud
{
  /// <summary>
  /// Turns a word at a size into its ink. The placer never looks at a font: it only ever
  /// sees a mask, which is what lets the console prove the algorithm with a stand-in and
  /// the window draw it with real glyphs.
  /// </summary>
  public interface IGlyphRasterizer
  {
    /// <summary>
    /// The mask's top-left is where the drawn text's top-left will be. The renderer must
    /// agree with this contract, or the gutter drifts.
    /// </summary>
    Mask Rasterize(string text, double fontSize);
  }
}
