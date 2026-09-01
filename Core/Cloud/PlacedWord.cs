using Core.Geometry;
using System;

namespace Core.Cloud
{
  /// <summary>A word that found its place. Left/Top are the top-left of its ink box.</summary>
  public class PlacedWord
  {
    public Tag Tag { get; private set; }
    public double FontSize { get; private set; }
    public int Left { get; private set; }
    public int Top { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public bool Rotated { get; private set; }
    /// <summary>Spiral candidates tried before this one was accepted.</summary>
    public int Attempts { get; private set; }

    public string Text
    {
      get
      {
        return Tag.Text;
      }
    }

    /// <summary>Degrees clockwise, ready for a RotateTransform.</summary>
    public double Angle
    {
      get
      {
        return Rotated ? 90 : 0;
      }
    }

    public Rectangle Bounds
    {
      get
      {
        return new Rectangle(Top, Left, Width, Height);
      }
    }

    public PlacedWord(Tag tag, double fontSize, int left, int top, int width, int height, bool rotated, int attempts)
    {
      if (tag == null)
      {
        throw new ArgumentNullException("tag");
      }
      Tag = tag;
      FontSize = fontSize;
      Left = left;
      Top = top;
      Width = width;
      Height = height;
      Rotated = rotated;
      Attempts = attempts;
    }

    public override string ToString()
    {
      return string.Format("{0} @({1},{2}) {3}x{4} {5:0.#}pt{6}", Text, Left, Top, Width, Height, FontSize, Rotated ? " rotated" : "");
    }
  }
}
