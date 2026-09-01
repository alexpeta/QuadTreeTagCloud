using Core.Cloud;
using Core.DataStructures;
using Core.Geometry;
using Core.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleTests
{
  using GlbCnst = Core.Enums.GlobalConstants;

  /// <summary>
  /// The console proves the algorithm before the window is allowed to draw it. Three
  /// self-checks pin the primitives against naive pixel loops, then two full clouds are
  /// laid out and every placement is re-verified with code that shares nothing with the
  /// placer's own collision path. Exit code 0 means every check passed.
  /// </summary>
  class Program
  {
    static int Main(string[] args)
    {
      int failures = 0;
      failures += SelfCheckIntersects();
      failures += SelfCheckDilate();
      failures += SelfCheckQuadTree();
      failures += RunScenario("solid boxes, quadtree on", new BoxRasterizer(), "cloud-boxes.svg", true, true);
      failures += RunScenario("solid boxes, quadtree off", new BoxRasterizer(), "cloud-boxes-no-tree.svg", true, false);
      failures += RunScenario("block letters, quadtree on", new BlockLetterRasterizer(), "cloud-letters.svg", false, true);
      failures += RunScenario("block letters, quadtree off", new BlockLetterRasterizer(), "cloud-letters-no-tree.svg", false, false);

      Console.WriteLine();
      Console.WriteLine(failures == 0 ? "ALL CHECKS PASSED" : string.Format("{0} CHECK(S) FAILED", failures));
      if (!Console.IsInputRedirected)
      {
        Console.ReadKey();
      }
      return failures == 0 ? 0 : 1;
    }

    #region Self-checks of the primitives
    static int SelfCheckIntersects()
    {
      var random = new Random(7);
      int failures = 0;
      const int TRIALS = 3000;
      for (int trial = 0; trial < TRIALS; trial++)
      {
        var board = RandomMask(random, random.Next(1, 200), random.Next(1, 40), 0.10);
        var sprite = RandomMask(random, random.Next(1, 150), random.Next(1, 30), 0.30);
        int left = random.Next(-5, board.Width + 5);
        int top = random.Next(-5, board.Height + 5);
        bool expected = NaiveIntersects(board, sprite, left, top);
        bool actual = board.Intersects(sprite, left, top);
        if (expected != actual)
        {
          failures++;
          if (failures <= 5)
          {
            Console.WriteLine("  Intersects disagrees with the pixel loop: board {0} sprite {1} at ({2},{3}) expected {4} got {5}", board, sprite, left, top, expected, actual);
          }
        }
      }
      Report("Mask.Intersects vs naive pixel loop", TRIALS, failures);
      return failures;
    }

    static int SelfCheckDilate()
    {
      var random = new Random(11);
      int failures = 0;
      const int TRIALS = 400;
      for (int trial = 0; trial < TRIALS; trial++)
      {
        var mask = RandomMask(random, random.Next(1, 40), random.Next(1, 40), 0.15);
        int radius = random.Next(0, 4);
        var dilated = mask.Dilate(radius);
        bool ok = dilated.Width == mask.Width + 2 * radius && dilated.Height == mask.Height + 2 * radius;
        for (int y = 0; ok && y < dilated.Height; y++)
        {
          for (int x = 0; ok && x < dilated.Width; x++)
          {
            bool expected = false;
            for (int dy = -radius; dy <= radius && !expected; dy++)
            {
              for (int dx = -radius; dx <= radius && !expected; dx++)
              {
                expected = mask.Get(x - radius + dx, y - radius + dy);
              }
            }
            ok = expected == dilated.Get(x, y);
          }
        }
        if (!ok)
        {
          failures++;
        }
      }
      Report("Mask.Dilate vs Chebyshev neighbourhood", TRIALS, failures);
      return failures;
    }

    /// <summary>
    /// The tree may be pessimistic (a Mixed leaf sends a candidate to the mask for space
    /// that was fine) but it must never be optimistic in either direction: an Empty verdict
    /// over ink would let words overlap, and a Full verdict over a hole would refuse a spot
    /// that was free. Both are checked against the mask itself; completeness is only reported.
    /// </summary>
    static int SelfCheckQuadTree()
    {
      var random = new Random(13);
      int failures = 0;
      int trulyEmpty = 0;
      int calledEmpty = 0;
      int trulyFull = 0;
      int calledFull = 0;
      const int TRIALS = 5000;

      var board = new OccupancyBoard(300, 200);
      for (int i = 0; i < 40; i++)
      {
        var sprite = RandomMask(random, random.Next(1, 60), random.Next(1, 40), 0.6);
        if (sprite.CountSetBits() == 0)
        {
          continue;
        }
        board.Occupy(sprite, sprite.Dilate(2), random.Next(0, 240), random.Next(0, 160), 2);
      }
      for (int trial = 0; trial < TRIALS; trial++)
      {
        var query = new Rectangle(random.Next(0, 200), random.Next(0, 300), random.Next(1, 50), random.Next(1, 30));
        int keepOutPixels = 0;
        int pixels = 0;
        for (int y = query.Top; y < query.Bottom; y++)
        {
          for (int x = query.Left; x < query.Right; x++)
          {
            if (x >= board.Width || y >= board.Height)
            {
              continue;
            }
            pixels++;
            if (board.KeepOut.Get(x, y))
            {
              keepOutPixels++;
            }
          }
        }
        bool taken = keepOutPixels > 0;
        bool solid = pixels > 0 && keepOutPixels == pixels;
        bool empty = board.Index.IsEmpty(query);
        bool full = board.Index.IsFull(query);
        if (taken && empty)
        {
          failures++;
        }
        if (!solid && full)
        {
          failures++;
        }
        if (!taken)
        {
          trulyEmpty++;
          if (empty)
          {
            calledEmpty++;
          }
        }
        if (solid)
        {
          trulyFull++;
          if (full)
          {
            calledFull++;
          }
        }
      }
      Report("QuadTree never says Empty over ink nor Full over a hole", TRIALS, failures);
      Console.WriteLine("  (completeness, not a check: {0} of {1} empty queries and {2} of {3} solid queries answered by the tree alone; {4} nodes)", calledEmpty, trulyEmpty, calledFull, trulyFull, board.Index.CountNodes());
      return failures;
    }
    #endregion Self-checks of the primitives

    #region Scenarios
    static int RunScenario(string name, IGlyphRasterizer rasterizer, string svgPath, bool inkIsBox, bool useQuadTree)
    {
      Console.WriteLine();
      Console.WriteLine("== {0}: {1}x{2} canvas, gutter {3}, fonts {4}-{5}, seed 2013", name, GlbCnst.MAX_WIDTH, GlbCnst.MAX_HEIGHT, GlbCnst.DEFAULT_GUTTER, 10, 64);
      var placer = new WordCloudPlacer(rasterizer, GlbCnst.MAX_WIDTH, GlbCnst.MAX_HEIGHT, GlbCnst.DEFAULT_GUTTER, 10, 64, 0.25, 2013, useQuadTree);
      var tags = SampleTags.Build();
      var result = placer.Place(tags);

      Console.WriteLine("  {0} placed, {1} refused, {2} ms", result.Placed.Count, result.Unplaced.Count, result.ElapsedMilliseconds);
      foreach (var refusal in result.Unplaced)
      {
        Console.WriteLine("  refused: {0}", refusal);
      }
      if (result.Board.Index != null)
      {
        Console.WriteLine("  quadtree: {0} nodes; candidates accepted on the tree alone: {1}, refused on the tree alone: {2}; bitwise tests: {3}", result.Board.Index.CountNodes(), result.Board.FastPathHits, result.Board.FastPathRejects, result.Board.BitwiseTests);
      }
      else
      {
        Console.WriteLine("  no quadtree; bitwise tests: {0}", result.Board.BitwiseTests);
      }
      Console.WriteLine("  spiral: {0} candidates walked in total, {1} per placed word on average, worst {2}", result.Placed.Sum(p => p.Attempts), result.Placed.Count == 0 ? 0 : result.Placed.Sum(p => p.Attempts) / result.Placed.Count, result.Placed.Count == 0 ? 0 : result.Placed.Max(p => p.Attempts));

      int ink = result.Board.Ink.CountSetBits();
      if (result.Placed.Count > 0)
      {
        int hullLeft = result.Placed.Min(p => p.Left);
        int hullTop = result.Placed.Min(p => p.Top);
        int hullRight = result.Placed.Max(p => p.Left + p.Width);
        int hullBottom = result.Placed.Max(p => p.Top + p.Height);
        long hull = (long)(hullRight - hullLeft) * (hullBottom - hullTop);
        Console.WriteLine("  density: ink {0} px = {1:0.0}% of the cloud's hull ({2}x{3}), {4:0.0}% of the canvas", ink, 100.0 * ink / hull, hullRight - hullLeft, hullBottom - hullTop, 100.0 * ink / ((long)GlbCnst.MAX_WIDTH * GlbCnst.MAX_HEIGHT));
      }

      int failures = 0;
      failures += VerifyInsideCanvas(result);
      failures += VerifyGutterPixelwise(result, rasterizer, GlbCnst.DEFAULT_GUTTER);
      if (inkIsBox)
      {
        failures += VerifyGutterByRectangles(result, GlbCnst.DEFAULT_GUTTER);
      }
      failures += VerifyEveryTagAccountedFor(tags, result);
      WriteSvg(result, rasterizer, svgPath);
      Console.WriteLine("  picture: {0}", Path.GetFullPath(svgPath));
      return failures;
    }

    static int VerifyInsideCanvas(PlacementResult result)
    {
      int failures = result.Placed.Count(p => p.Left < 0 || p.Top < 0 || p.Left + p.Width > GlbCnst.MAX_WIDTH || p.Top + p.Height > GlbCnst.MAX_HEIGHT);
      Report("every placed word lies inside the canvas", result.Placed.Count, failures);
      return failures;
    }

    /// <summary>
    /// Re-rasterize every placed word and paint it onto an ownership grid. Any pixel
    /// painted twice is an overlap; any ink with foreign ink inside its gutter square is a
    /// gutter violation. Uses Mask.Get on the sprites and nothing else from Mask.
    /// </summary>
    static int VerifyGutterPixelwise(PlacementResult result, IGlyphRasterizer rasterizer, int gutter)
    {
      int width = GlbCnst.MAX_WIDTH;
      int height = GlbCnst.MAX_HEIGHT;
      var owner = new int[width, height];
      for (int x = 0; x < width; x++)
      {
        for (int y = 0; y < height; y++)
        {
          owner[x, y] = -1;
        }
      }
      var sprites = new List<Mask>();
      int overlaps = 0;
      for (int i = 0; i < result.Placed.Count; i++)
      {
        var word = result.Placed[i];
        var sprite = rasterizer.Rasterize(word.Text, word.FontSize);
        if (word.Rotated)
        {
          sprite = sprite.RotateClockwise();
        }
        sprites.Add(sprite);
        for (int y = 0; y < sprite.Height; y++)
        {
          for (int x = 0; x < sprite.Width; x++)
          {
            if (!sprite.Get(x, y))
            {
              continue;
            }
            int bx = word.Left + x;
            int by = word.Top + y;
            if (owner[bx, by] != -1)
            {
              overlaps++;
            }
            owner[bx, by] = i;
          }
        }
      }
      int gutterViolations = 0;
      for (int i = 0; i < result.Placed.Count; i++)
      {
        var word = result.Placed[i];
        var sprite = sprites[i];
        for (int y = 0; y < sprite.Height; y++)
        {
          for (int x = 0; x < sprite.Width; x++)
          {
            if (!sprite.Get(x, y))
            {
              continue;
            }
            for (int dy = -gutter; dy <= gutter; dy++)
            {
              for (int dx = -gutter; dx <= gutter; dx++)
              {
                int bx = word.Left + x + dx;
                int by = word.Top + y + dy;
                if (bx < 0 || by < 0 || bx >= width || by >= height)
                {
                  continue;
                }
                if (owner[bx, by] != -1 && owner[bx, by] != i)
                {
                  gutterViolations++;
                }
              }
            }
          }
        }
      }
      Report("no ink pixel is painted twice", result.Placed.Count, overlaps);
      Report(string.Format("no foreign ink within {0} px of any ink pixel", gutter), result.Placed.Count, gutterViolations);
      return (overlaps > 0 ? 1 : 0) + (gutterViolations > 0 ? 1 : 0);
    }

    /// <summary>Pure rectangle arithmetic; valid only when the ink is the whole box.</summary>
    static int VerifyGutterByRectangles(PlacementResult result, int gutter)
    {
      int failures = 0;
      var words = result.Placed;
      for (int i = 0; i < words.Count; i++)
      {
        for (int j = i + 1; j < words.Count; j++)
        {
          var a = words[i];
          var b = words[j];
          bool apart = a.Left + a.Width + gutter <= b.Left
                    || b.Left + b.Width + gutter <= a.Left
                    || a.Top + a.Height + gutter <= b.Top
                    || b.Top + b.Height + gutter <= a.Top;
          if (!apart)
          {
            failures++;
          }
        }
      }
      Report("every pair of boxes is at least the gutter apart (rectangle arithmetic)", words.Count * (words.Count - 1) / 2, failures);
      return failures > 0 ? 1 : 0;
    }

    static int VerifyEveryTagAccountedFor(IList<Tag> tags, PlacementResult result)
    {
      var seen = new HashSet<string>(result.Placed.Select(p => p.Text).Concat(result.Unplaced.Select(u => u.Tag.Text)));
      int missing = tags.Count(t => !seen.Contains(t.Text));
      int duplicates = result.Placed.Count + result.Unplaced.Count - seen.Count;
      Report("every tag is either placed or refused with a reason, exactly once", tags.Count, missing + duplicates);
      return missing + duplicates > 0 ? 1 : 0;
    }
    #endregion Scenarios

    #region Helpers
    static void Report(string what, int trials, int failures)
    {
      Console.WriteLine("  [{0}] {1} ({2} checked{3})", failures == 0 ? "PASS" : "FAIL", what, trials, failures == 0 ? "" : string.Format(", {0} failed", failures));
    }

    static Mask RandomMask(Random random, int width, int height, double density)
    {
      var mask = new Mask(width, height);
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          if (random.NextDouble() < density)
          {
            mask.Set(x, y);
          }
        }
      }
      return mask;
    }

    static bool NaiveIntersects(Mask board, Mask sprite, int left, int top)
    {
      if (left < 0 || top < 0 || left + sprite.Width > board.Width || top + sprite.Height > board.Height)
      {
        return true;
      }
      for (int y = 0; y < sprite.Height; y++)
      {
        for (int x = 0; x < sprite.Width; x++)
        {
          if (sprite.Get(x, y) && board.Get(left + x, top + y))
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>One path per word, built from horizontal ink runs, so the picture is the placement and not an artist's impression of it.</summary>
    static void WriteSvg(PlacementResult result, IGlyphRasterizer rasterizer, string path)
    {
      var svg = new StringBuilder();
      svg.AppendFormat(CultureInfo.InvariantCulture, "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{0}\" height=\"{1}\" viewBox=\"0 0 {0} {1}\">\n", GlbCnst.MAX_WIDTH, GlbCnst.MAX_HEIGHT);
      svg.AppendFormat(CultureInfo.InvariantCulture, "<rect width=\"{0}\" height=\"{1}\" fill=\"#d3d3d3\"/>\n", GlbCnst.MAX_WIDTH, GlbCnst.MAX_HEIGHT);
      string[] palette = new[] { "#1b263b", "#415a77", "#b23a48", "#2a9d8f", "#6a4c93", "#e76f51" };
      for (int i = 0; i < result.Placed.Count; i++)
      {
        var word = result.Placed[i];
        var sprite = rasterizer.Rasterize(word.Text, word.FontSize);
        if (word.Rotated)
        {
          sprite = sprite.RotateClockwise();
        }
        var d = new StringBuilder();
        for (int y = 0; y < sprite.Height; y++)
        {
          int x = 0;
          while (x < sprite.Width)
          {
            if (!sprite.Get(x, y))
            {
              x++;
              continue;
            }
            int run = 0;
            while (x + run < sprite.Width && sprite.Get(x + run, y))
            {
              run++;
            }
            d.AppendFormat(CultureInfo.InvariantCulture, "M{0} {1}h{2}v1h-{2}z", word.Left + x, word.Top + y, run);
            x += run;
          }
        }
        svg.AppendFormat(CultureInfo.InvariantCulture, "<path d=\"{0}\" fill=\"{1}\"><title>{2}</title></path>\n", d, palette[i % palette.Length], word.Text);
      }
      svg.Append("</svg>\n");
      File.WriteAllText(path, svg.ToString());
    }
    #endregion Helpers
  }
}
