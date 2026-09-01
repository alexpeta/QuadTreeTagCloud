using System;

namespace Core.Geometry
{
  /// <summary>
  /// A 1-bit raster. Rows are packed into 64-bit words so the collision test between a
  /// word's ink and the board costs a few shifts and ANDs per row instead of a pixel loop.
  /// Bit k of word j in a row is the pixel at x = 64 * j + k.
  ///
  /// Only Intersects is on the hot path and works on whole words. Or and Dilate touch
  /// sprite-sized masks once per word and stay as plain pixel loops on purpose: the
  /// console harness checks the word-level test against exactly this kind of loop.
  /// </summary>
  public class Mask
  {
    private readonly ulong[] _bits;

    public int Width { get; private set; }
    public int Height { get; private set; }
    /// <summary>Words per row.</summary>
    public int Stride { get; private set; }

    #region Constructors
    public Mask(int width, int height)
    {
      if (width < 0 || height < 0)
      {
        throw new ArgumentOutOfRangeException("width and height must not be negative");
      }
      Width = width;
      Height = height;
      Stride = (width + 63) / 64;
      _bits = new ulong[Stride * height];
    }
    #endregion Constructors

    #region Pixels
    /// <summary>Outside the raster is not ink.</summary>
    public bool Get(int x, int y)
    {
      if (x < 0 || y < 0 || x >= Width || y >= Height)
      {
        return false;
      }
      return (_bits[y * Stride + (x >> 6)] & (1UL << (x & 63))) != 0;
    }

    public void Set(int x, int y)
    {
      if (x < 0 || y < 0 || x >= Width || y >= Height)
      {
        throw new ArgumentOutOfRangeException(string.Format("({0},{1}) is outside a {2}x{3} mask", x, y, Width, Height));
      }
      _bits[y * Stride + (x >> 6)] |= 1UL << (x & 63);
    }

    public int CountSetBits()
    {
      int count = 0;
      for (int i = 0; i < _bits.Length; i++)
      {
        ulong w = _bits[i];
        while (w != 0)
        {
          w &= w - 1;
          count++;
        }
      }
      return count;
    }
    #endregion Pixels

    #region Public Methods
    /// <summary>
    /// True if any ink of <paramref name="sprite"/>, laid with its top-left corner at
    /// (left, top) on this mask, lands on ink already here. A sprite that would hang
    /// over the edge counts as a collision: the edge is not free space.
    /// </summary>
    public bool Intersects(Mask sprite, int left, int top)
    {
      if (sprite == null)
      {
        throw new ArgumentNullException("sprite");
      }
      if (left < 0 || top < 0 || left + sprite.Width > Width || top + sprite.Height > Height)
      {
        return true;
      }

      int wordShift = left >> 6;
      int bitShift = left & 63;

      for (int r = 0; r < sprite.Height; r++)
      {
        int spriteBase = r * sprite.Stride;
        int boardBase = (top + r) * Stride;

        for (int i = 0; i < sprite.Stride; i++)
        {
          ulong w = sprite._bits[spriteBase + i];
          if (w == 0)
          {
            continue;
          }
          int j = i + wordShift;
          if (j < Stride && (_bits[boardBase + j] & (w << bitShift)) != 0)
          {
            return true;
          }
          // a shift of 64 is a no-op in C#, so the spill word only exists when bitShift > 0
          if (bitShift != 0 && j + 1 < Stride && (_bits[boardBase + j + 1] & (w >> (64 - bitShift))) != 0)
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// OR <paramref name="sprite"/> into this mask at (left, top). Parts that fall outside
    /// this mask are dropped: nothing can ever be placed there anyway.
    /// </summary>
    public void Or(Mask sprite, int left, int top)
    {
      if (sprite == null)
      {
        throw new ArgumentNullException("sprite");
      }
      for (int y = 0; y < sprite.Height; y++)
      {
        int by = top + y;
        if (by < 0 || by >= Height)
        {
          continue;
        }
        for (int x = 0; x < sprite.Width; x++)
        {
          int bx = left + x;
          if (bx < 0 || bx >= Width)
          {
            continue;
          }
          if (sprite.Get(x, y))
          {
            Set(bx, by);
          }
        }
      }
    }

    /// <summary>
    /// Grow every ink pixel into a (2r+1)-square. The result is 2r wider and 2r taller
    /// and the original ink sits at offset (r, r). This is how the gutter between words
    /// is made: a word is tested raw against a board of everyone else's ink grown by r,
    /// so no two letters ever come closer than r empty pixels.
    /// </summary>
    public Mask Dilate(int radius)
    {
      if (radius < 0)
      {
        throw new ArgumentOutOfRangeException("radius");
      }
      var horizontal = new Mask(Width + 2 * radius, Height + 2 * radius);
      for (int y = 0; y < Height; y++)
      {
        for (int x = 0; x < Width; x++)
        {
          if (!Get(x, y))
          {
            continue;
          }
          for (int d = 0; d <= 2 * radius; d++)
          {
            horizontal.Set(x + d, y + radius);
          }
        }
      }
      var result = new Mask(horizontal.Width, horizontal.Height);
      for (int y = 0; y < horizontal.Height; y++)
      {
        for (int x = 0; x < horizontal.Width; x++)
        {
          if (!horizontal.Get(x, y))
          {
            continue;
          }
          for (int d = -radius; d <= radius; d++)
          {
            int ty = y + d;
            if (ty >= 0 && ty < result.Height)
            {
              result.Set(x, ty);
            }
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Rotate 90 degrees clockwise: (x, y) becomes (Height - 1 - y, x). This is the same
    /// direction WPF's RotateTransform(90) turns, y pointing down.
    /// </summary>
    public Mask RotateClockwise()
    {
      var rotated = new Mask(Height, Width);
      for (int y = 0; y < Height; y++)
      {
        for (int x = 0; x < Width; x++)
        {
          if (Get(x, y))
          {
            rotated.Set(Height - 1 - y, x);
          }
        }
      }
      return rotated;
    }
    #endregion Public Methods

    public override string ToString()
    {
      return string.Format("mask {0}x{1} ink:{2}", Width, Height, CountSetBits());
    }
  }
}
