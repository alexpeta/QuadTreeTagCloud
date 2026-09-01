using Core.Cloud;
using System;
using System.Collections.Generic;

namespace Core.Helpers
{
  /// <summary>
  /// The vocabulary of this solution, weighted on a Zipf curve so the cloud has a few
  /// giants and a long tail of mortar. Shared by the console and the window so both
  /// harnesses lay out the same words.
  /// </summary>
  public static class SampleTags
  {
    private static readonly string[] WORDS = new string[]
    {
      "QuadTree", "Spiral", "Bitmask", "Gutter", "Occupancy", "Rasterize", "Sprite", "Glyph",
      "Collision", "Density", "Greedy", "FirstFit", "Decreasing", "Heuristic", "NPHard", "BinPacking",
      "Dilation", "Mask", "Shift", "And", "Or", "Empty", "Mixed", "Full", "Subdivide", "Root", "Leaf",
      "Visit", "Node", "Seed", "Jitter", "Rotate", "Ninety", "Canvas", "Rectangle", "Archimedes",
      "Wordle", "Feinberg", "FormattedText", "RenderTarget", "Alpha", "Threshold", "TextBlock",
      "FontSize", "Binding", "ItemsControl", "DataTemplate", "ObservableCollection", "MVVM", "WPF",
      "LINQ", "Lambda", "CSharp5", "DotNet45", "VS2012", "Mono", "Console", "Verify", "Overlap",
      "Zero", "Refuse", "Reason", "Craft", "Museum", "Fountain", "July2013", "Transposed", "Broadcast",
      "Amnesia", "Restored", "September2026", "Tessera"
    };

    public static IList<Tag> Build()
    {
      var tags = new List<Tag>();
      for (int i = 0; i < WORDS.Length; i++)
      {
        double weight = 1000.0 / Math.Pow(i + 1, 0.7);
        tags.Add(new Tag(WORDS[i], weight));
      }
      return tags;
    }
  }
}
