using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataStructures
{
  public class QuadTreeNode
  {
    public object Value { get; private set; }
    public QuadTreeNode[] Children { get; private set; }

    public QuadTreeNode(object value)
    {
      Children = new QuadTreeNode[Core.Enums.GlobalConstants.TREE_CHILDREN_COUNT];
      Value = value;
    }

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
    #endregion Public Methods

    private bool HasChildrenFull()
    {
      return Children.Count(c => c != null && c.Value != null) == Core.Enums.GlobalConstants.TREE_CHILDREN_COUNT;
    }

  }
}
