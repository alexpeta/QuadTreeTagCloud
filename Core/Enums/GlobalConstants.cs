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
