using Core.DataStructures;
using Core.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Core.Cloud
{
  /// <summary>
  /// The placer. Three organs, kept apart on purpose: the spiral says where to try, the
  /// mask says whether it is free, the board remembers what is taken. Words go in
  /// heaviest first while space is plentiful; the small ones are the mortar.
  ///
  /// Seeded throughout: the same tags on the same canvas give the same cloud every time.
  /// </summary>
  public class WordCloudPlacer
  {
    private const double SPIRAL_PITCH = 1.5;   // pixels of radius per radian
    private const double SPIRAL_STEP = 2.0;    // pixels between consecutive candidates
    private const int CENTRE_JITTER = 4;       // pixels; keeps the biggest word from always sitting dead centre

    private readonly IGlyphRasterizer _rasterizer;
    private readonly Random _random;

    public int CanvasWidth { get; private set; }
    public int CanvasHeight { get; private set; }
    public int Gutter { get; private set; }
    public double MinFontSize { get; private set; }
    public double MaxFontSize { get; private set; }
    public double RotationProbability { get; private set; }
    /// <summary>Whether the board keeps its quadtree. Off, every candidate goes to the bitwise test.</summary>
    public bool UseQuadTree { get; private set; }

    #region Constructors
    public WordCloudPlacer(IGlyphRasterizer rasterizer, int canvasWidth, int canvasHeight, int gutter, double minFontSize, double maxFontSize, double rotationProbability, int seed)
      : this(rasterizer, canvasWidth, canvasHeight, gutter, minFontSize, maxFontSize, rotationProbability, seed, true)
    {
    }

    public WordCloudPlacer(IGlyphRasterizer rasterizer, int canvasWidth, int canvasHeight, int gutter, double minFontSize, double maxFontSize, double rotationProbability, int seed, bool useQuadTree)
    {
      if (rasterizer == null)
      {
        throw new ArgumentNullException("rasterizer");
      }
      if (canvasWidth <= 0 || canvasHeight <= 0)
      {
        throw new ArgumentOutOfRangeException("the canvas needs a positive width and height");
      }
      if (gutter < 0)
      {
        throw new ArgumentOutOfRangeException("gutter", "the gutter cannot be negative");
      }
      if (minFontSize <= 0 || maxFontSize < minFontSize)
      {
        throw new ArgumentOutOfRangeException("font sizes must be positive and max must not be below min");
      }
      if (rotationProbability < 0 || rotationProbability > 1)
      {
        throw new ArgumentOutOfRangeException("rotationProbability", "must be between 0 and 1");
      }
      _rasterizer = rasterizer;
      _random = new Random(seed);
      CanvasWidth = canvasWidth;
      CanvasHeight = canvasHeight;
      Gutter = gutter;
      MinFontSize = minFontSize;
      MaxFontSize = maxFontSize;
      RotationProbability = rotationProbability;
      UseQuadTree = useQuadTree;
    }
    #endregion Constructors

    #region Public Methods
    public PlacementResult Place(IEnumerable<Tag> tags)
    {
      if (tags == null)
      {
        throw new ArgumentNullException("tags");
      }
      var ordered = tags.OrderByDescending(t => t.Weight).ThenBy(t => t.Text, StringComparer.Ordinal).ToList();
      var board = new OccupancyBoard(CanvasWidth, CanvasHeight, UseQuadTree);
      var placed = new List<PlacedWord>();
      var unplaced = new List<UnplacedWord>();
      var watch = Stopwatch.StartNew();

      if (ordered.Count == 0)
      {
        return new PlacementResult(placed, unplaced, board, watch.ElapsedMilliseconds);
      }

      double minWeight = ordered.Min(t => t.Weight);
      double maxWeight = ordered.Max(t => t.Weight);
      double maxRadius = Math.Sqrt((double)CanvasWidth * CanvasWidth + (double)CanvasHeight * CanvasHeight);

      foreach (var tag in ordered)
      {
        double fontSize = this.ScaleFont(tag.Weight, minWeight, maxWeight);
        bool rotate = _random.NextDouble() < RotationProbability;
        int jitterX = _random.Next(-CENTRE_JITTER, CENTRE_JITTER + 1);
        int jitterY = _random.Next(-CENTRE_JITTER, CENTRE_JITTER + 1);

        Mask sprite = _rasterizer.Rasterize(tag.Text, fontSize);
        if (sprite == null || sprite.Width == 0 || sprite.Height == 0 || sprite.CountSetBits() == 0)
        {
          unplaced.Add(new UnplacedWord(tag, fontSize, "the rasterizer produced no ink for this word", 0));
          continue;
        }
        if (rotate)
        {
          sprite = sprite.RotateClockwise();
        }
        if (sprite.Width > CanvasWidth || sprite.Height > CanvasHeight)
        {
          unplaced.Add(new UnplacedWord(tag, fontSize, string.Format("a {0}x{1} sprite cannot fit a {2}x{3} canvas at any position", sprite.Width, sprite.Height, CanvasWidth, CanvasHeight), 0));
          continue;
        }
        Mask keepOut = sprite.Dilate(Gutter);

        int attempts = 0;
        PlacedWord result = null;
        foreach (var point in Spiral.Walk(CanvasWidth / 2 + jitterX, CanvasHeight / 2 + jitterY, SPIRAL_PITCH, SPIRAL_STEP, maxRadius))
        {
          attempts++;
          int left = point.X - sprite.Width / 2;
          int top = point.Y - sprite.Height / 2;
          if (board.IsFree(sprite, left, top))
          {
            board.Occupy(sprite, keepOut, left, top, Gutter);
            result = new PlacedWord(tag, fontSize, left, top, sprite.Width, sprite.Height, rotate, attempts);
            break;
          }
        }

        if (result != null)
        {
          placed.Add(result);
        }
        else
        {
          unplaced.Add(new UnplacedWord(tag, fontSize, string.Format("no free position for a {0}x{1} sprite after {2} candidates: the canvas is full at this size", sprite.Width, sprite.Height, attempts), attempts));
        }
      }

      watch.Stop();
      return new PlacementResult(placed, unplaced, board, watch.ElapsedMilliseconds);
    }
    #endregion Public Methods

    #region Private Methods
    /// <summary>
    /// Square-root scaling: linear lets the top word eat the canvas. Equal weights all
    /// land halfway between the two sizes.
    /// </summary>
    private double ScaleFont(double weight, double minWeight, double maxWeight)
    {
      double span = maxWeight - minWeight;
      double t = span <= 0 ? 0.5 : Math.Sqrt((weight - minWeight) / span);
      return MinFontSize + (MaxFontSize - MinFontSize) * t;
    }
    #endregion Private Methods
  }
}
