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
    public QuadTree(QuadTreeNode root)
    {
      Root = root;
    }
    #endregion Constructors

    #region Public Methods
    public void Insert(QuadTreeNode nodeToInsert)
    {
      Root.Insert(nodeToInsert);
    }

    public void Visit()
    {
      Root.Visit();
    }

    #endregion Public Methods
  }
}
