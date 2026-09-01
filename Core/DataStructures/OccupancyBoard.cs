using Core.Enums;
using Core.Geometry;
using System;

namespace Core.DataStructures
{
  /// <summary>
  /// What's taken. Two rasters and an index: Ink is the letters themselves (kept for the
  /// density figure), KeepOut is that ink grown by the gutter (what a candidate must not
  /// touch), and the quadtree indexes the parts of the canvas nothing has reached yet.
  /// The board records; it neither searches nor decides where a word wants to be.
  /// </summary>
  public class OccupancyBoard
  {
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Mask Ink { get; private set; }
    public Mask KeepOut { get; private set; }
    /// <summary>Null when the board runs on the masks alone; the console measures both ways.</summary>
    public QuadTree Index { get; private set; }

    /// <summary>Candidates accepted on the quadtree's word alone.</summary>
    public int FastPathHits { get; private set; }
    /// <summary>Candidates refused on the quadtree's word alone.</summary>
    public int FastPathRejects { get; private set; }
    /// <summary>Candidates that had to be tested bit by bit.</summary>
    public int BitwiseTests { get; private set; }
    public int Placements { get; private set; }

    #region Constructors
    public OccupancyBoard(int width, int height) : this(width, height, true)
    {
    }

    public OccupancyBoard(int width, int height, bool indexed)
    {
      if (width <= 0 || height <= 0)
      {
        throw new ArgumentOutOfRangeException("a board needs a positive width and height");
      }
      Width = width;
      Height = height;
      Ink = new Mask(width, height);
      KeepOut = new Mask(width, height);
      Index = indexed ? new QuadTree(new Rectangle(0, 0, width, height)) : null;
    }
    #endregion Constructors

    #region Public Methods
    /// <summary>
    /// May <paramref name="sprite"/> sit with its top-left at (left, top)? Off the canvas
    /// is never free. Over untouched space the quadtree answers alone, and over space that
    /// is keep-out edge to edge it refuses alone; anywhere else the raw sprite is tested
    /// against everyone else's grown ink.
    /// </summary>
    public bool IsFree(Mask sprite, int left, int top)
    {
      if (sprite == null)
      {
        throw new ArgumentNullException("sprite");
      }
      if (left < 0 || top < 0 || left + sprite.Width > Width || top + sprite.Height > Height)
      {
        return false;
      }
      if (Index != null)
      {
        NodeState? verdict = Index.Classify(new Rectangle(top, left, sprite.Width, sprite.Height));
        if (verdict == NodeState.Empty)
        {
          FastPathHits++;
          return true;
        }
        if (verdict == NodeState.Full)
        {
          FastPathRejects++;
          return false;
        }
      }
      BitwiseTests++;
      return !KeepOut.Intersects(sprite, left, top);
    }

    /// <summary>
    /// Record a placement. <paramref name="keepOut"/> must be <paramref name="sprite"/>
    /// dilated by <paramref name="gutter"/>, so its origin sits gutter pixels up and left
    /// of the sprite's.
    /// </summary>
    public void Occupy(Mask sprite, Mask keepOut, int left, int top, int gutter)
    {
      if (sprite == null || keepOut == null)
      {
        throw new ArgumentNullException("sprite and keepOut are both required");
      }
      if (keepOut.Width != sprite.Width + 2 * gutter || keepOut.Height != sprite.Height + 2 * gutter)
      {
        throw new ArgumentException("keepOut is not this sprite dilated by the gutter");
      }
      Ink.Or(sprite, left, top);
      KeepOut.Or(keepOut, left - gutter, top - gutter);
      if (Index != null)
      {
        Index.Mark(this.Clip(new Rectangle(top - gutter, left - gutter, keepOut.Width, keepOut.Height)), this.IsCoveredByKeepOut);
      }
      Placements++;
    }
    #endregion Public Methods

    #region Private Methods
    /// <summary>Every pixel of <paramref name="cell"/> is keep-out. Cells are small; a plain scan is honest and cheap.</summary>
    private bool IsCoveredByKeepOut(Rectangle cell)
    {
      for (int y = cell.Top; y < cell.Bottom; y++)
      {
        for (int x = cell.Left; x < cell.Right; x++)
        {
          if (!KeepOut.Get(x, y))
          {
            return false;
          }
        }
      }
      return cell.Area > 0;
    }

    private Rectangle Clip(Rectangle region)
    {
      int top = Math.Max(0, region.Top);
      int left = Math.Max(0, region.Left);
      int bottom = Math.Min(Height, region.Bottom);
      int right = Math.Min(Width, region.Right);
      return new Rectangle(top, left, Math.Max(0, right - left), Math.Max(0, bottom - top));
    }
    #endregion Private Methods
  }
}
