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
    public bool IsOccupied { get; private set; }
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
        IsOccupied = false;
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
      if (this.Surface.Area > nodeToInsert.Surface.InflatedArea)
      {
        this.CreateChildren();
        for (int i = 0; i < Core.Enums.GlobalConstants.TREE_CHILDREN_COUNT; i++)
        {
          Children[i].Insert(nodeToInsert);
        }
      }
      else
      {
        this.CreateChildren();
        this.AddNodeAsChild(nodeToInsert);
      }
    }

    public void Visit(Action<QuadTreeNode> action)
    {

      action(this);

      for (int i = 0; i < Core.Enums.GlobalConstants.TREE_CHILDREN_COUNT; i++)
      {
        if (Children[i] != null)
        {
          Children[i].Visit(action);
        }
      }
    }
    #endregion Public Methods

    #region Private Methods
    private void AddNodeAsChild(QuadTreeNode nodeToInsert)
    {
      for (int i = 0; i < Core.Enums.GlobalConstants.TREE_CHILDREN_COUNT; i++)
      {
        if (Children[i].IsOccupied)
        {
          continue;
        }
        else
        {
          nodeToInsert.Surface.Top = Children[i].Surface.Top;
          nodeToInsert.Surface.Left = Children[i].Surface.Left;
          nodeToInsert.IsOccupied = true;
          Children[i] = nodeToInsert;
          break;
        }
      }
    }
    private void CreateChildren()
    {
      // the smallest subnode has an area 
      if ((Surface.Area) <= 100)
      {
        return;
      }

      int halfWidth = (int)(Surface.Width / 2f);
      int halfHeight = (int)(Surface.Height / 2f);

      Children = new QuadTreeNode[4];
      Children[0] = new QuadTreeNode(new Rectangle(Surface.Top, Surface.Left, halfWidth, halfHeight));
      Children[1] = new QuadTreeNode(new Rectangle(Surface.Left, Surface.Top + halfHeight, halfWidth, halfHeight));
      Children[2] = new QuadTreeNode(new Rectangle(Surface.Left + halfWidth, Surface.Top, halfWidth, halfHeight));
      Children[3] = new QuadTreeNode(new Rectangle(Surface.Left + halfWidth, Surface.Top + halfHeight, halfWidth, halfHeight));
    }
    #endregion Private Methods


    
  }
}
