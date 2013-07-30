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
      Top = top;
      Left = left;
      Width = width;
      Height = height;
    }

    #endregion Constructor

  }
}
