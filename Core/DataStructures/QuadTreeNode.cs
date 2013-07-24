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
      Children = new QuadTreeNode[4];
      Value = value;
    }

    #region Public Methods
    public void Insert(QuadTreeNode nodeToInsert)
    {
      for (int i = 0; i < 4; i++)
      {       
        if (Children[i] == null)
        {
          Children[i] = nodeToInsert;
          return;
        }
      }

      for (int i = 0; i < 4; i++)
      {
        if (Children[i].IsFull())
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

    private bool IsFull()
    {
      return Children.Count(c => c!= null && c.Value != null) == 4;
    }

  }
}
