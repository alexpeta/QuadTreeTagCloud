using Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataStructures
{
  public class QuadTree
  {
    public QuadTreeNode Root { get; private set; }

    #region Constructors
    public QuadTree(Rectangle surface)
    {
      Root = new QuadTreeNode(surface);
    }
    #endregion Constructors

    #region Public Methods
    public void Mark(Rectangle region, Func<Rectangle, bool> isCovered)
    {
      Root.Mark(region, isCovered);
    }

    public bool IsFull(Rectangle region)
    {
      return Root.IsFull(region);
    }

    public Core.Enums.NodeState? Classify(Rectangle region)
    {
      return Root.Classify(region);
    }

    public bool IsEmpty(Rectangle region)
    {
      return Root.IsEmpty(region);
    }

    public void Visit(Action<QuadTreeNode> action)
    {
      Root.Visit(action);
    }

    public int CountNodes()
    {
      int count = 0;
      Root.Visit(n => count++);
      return count;
    }
    #endregion Public Methods
  }
}
