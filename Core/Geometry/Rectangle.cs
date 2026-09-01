using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Geometry
{
  public class Rectangle
  {
    #region Public Properties
    public int Top { get; private set; }
    public int Left { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    /// <summary>Exclusive: the first column NOT inside.</summary>
    public int Right
    {
      get
      {
        return Left + Width;
      }
    }
    /// <summary>Exclusive: the first row NOT inside.</summary>
    public int Bottom
    {
      get
      {
        return Top + Height;
      }
    }
    public int Area
    {
      get
      {
        return Width * Height;
      }
    }
    #endregion Public Properties
    
    #region Constructor
    public Rectangle(): this(0,0,0,0)
    {
    }
    public Rectangle(int top, int left, int width, int height)
    {
      if (width < 0 || height < 0)
      {
        throw new ArgumentOutOfRangeException("width and height must not be negative");
      }
      Top = top;
      Left = left;
      Width = width;
      Height = height;
    }

    #endregion Constructor

    #region Public Methods
    /// <summary>
    /// Half-open on both axes: two rectangles that share an edge do not intersect,
    /// and a rectangle with no area intersects nothing.
    /// </summary>
    public bool Intersects(Rectangle other)
    {
      if (other == null)
      {
        throw new ArgumentNullException("other");
      }
      return other.Left < Right && Left < other.Right && other.Top < Bottom && Top < other.Bottom;
    }

    public bool Contains(Rectangle other)
    {
      if (other == null)
      {
        throw new ArgumentNullException("other");
      }
      return other.Left >= Left && other.Right <= Right && other.Top >= Top && other.Bottom <= Bottom;
    }
    #endregion Public Methods

    public override string ToString()
    {
      return string.Format("w:{0} h:{1} a:{2}", this.Width, this.Height, this.Area);
    }

  }
}
