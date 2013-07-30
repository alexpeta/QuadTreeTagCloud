using Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataStructures
{
  public class QuadTreeNode
  {
    public Rectangle Surface { get; private set; }
    public QuadTreeNode[] Children { get; private set; }

    #region Constructors
    public QuadTreeNode(Rectangle surface) 
        : this(surface,null)
    {
    }
    public QuadTreeNode(Rectangle surface, params Rectangle[] children)
    {
        Surface = surface;
        Children = new QuadTreeNode[Core.Enums.GlobalConstants.TREE_CHILDREN_COUNT];

        if (children == null || children.Count() != 4)
        {
            return;
        }

        for (int i = 0; i < Core.Enums.GlobalConstants.TREE_CHILDREN_COUNT; i++)
        {
            Children[i] = new QuadTreeNode(children[i]);
        }
    }
    #endregion Constructors

    #region Public Methods
    public void Insert(QuadTreeNode nodeToInsert)
    {
      for (int i = 0; i < Core.Enums.GlobalConstants.TREE_CHILDREN_COUNT; i++)
      {       
        if (Children[i] == null)
        {
          Children[i] = nodeToInsert;
          return;
        }
      }

      for (int i = 0; i < Core.Enums.GlobalConstants.TREE_CHILDREN_COUNT; i++)
      {
        if (Children[i].HasChildrenFull())
        {
          continue;
        }
        else
        {
          Children[i].Insert(nodeToInsert);
          break;
        }
      }
    }
    public void Visit()
    {
      Console.WriteLine(string.Format("w:{0} h:{1} a:{2}",this.Surface.Width, this.Surface.Height, this.Surface.Area));

      for (int i = 0; i < Core.Enums.GlobalConstants.TREE_CHILDREN_COUNT; i++)
      {
        if (Children[i] != null)
        {
          Children[i].Visit();
        }
      }
    }
    #endregion Public Methods

    #region Private Methods
    private bool HasChildrenFull()
    {
      return Children.Count(c => c != null && c.Surface != null) == Core.Enums.GlobalConstants.TREE_CHILDREN_COUNT;
    }
    #endregion Private Methods


    
  }
}
