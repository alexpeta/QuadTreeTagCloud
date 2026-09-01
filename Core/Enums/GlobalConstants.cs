using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums
{
  public sealed class GlobalConstants
  {

    public static readonly int TREE_CHILDREN_COUNT = 4; //captain obvious
    public static readonly int MAX_WIDTH = 900;
    public static readonly int MAX_HEIGHT = 600;

    // the quadtree stops subdividing at this side length; a smaller cell that is
    // partly taken is a Mixed leaf and simply sends the candidate to the bitwise test
    public static readonly int MIN_QUADTREE_CELL = 8;
    // empty pixels kept between the ink of any two words
    public static readonly int DEFAULT_GUTTER = 2;



    #region Constructors
    private GlobalConstants()
    {
    }
    static GlobalConstants()
    {
    }
    #endregion Constructors

  }
}
