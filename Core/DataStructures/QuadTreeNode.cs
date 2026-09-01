using Core.Enums;
using Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataStructures
{
  /// <summary>
  /// A region quadtree over the occupancy board. It does not place anything and it does
  /// not hold words: it remembers which parts of the canvas are still Empty, so that a
  /// candidate over untouched space is accepted without a bitwise test, and which parts
  /// are keep-out edge to edge, so a candidate buried in the dense core is refused without
  /// one either. Everything in between is sent to the mask. Both verdicts are sound:
  /// the tree never says Empty over ink, and never says Full over a hole.
  /// </summary>
  public class QuadTreeNode
  {
    public Rectangle Surface { get; private set; }
    public NodeState State { get; private set; }
    /// <summary>Null until the node is subdivided; dropped again when it becomes Full.</summary>
    public QuadTreeNode[] Children { get; private set; }

    public bool IsLeaf
    {
      get
      {
        return Children == null;
      }
    }

    #region Constructors
    public QuadTreeNode(Rectangle surface)
    {
      if (surface == null)
      {
        throw new ArgumentNullException("surface");
      }
      Surface = surface;
      State = NodeState.Empty;
      Children = null;
    }
    #endregion Constructors

    #region Public Methods
    /// <summary>
    /// Record that <paramref name="region"/> has ink in it. <paramref name="isCovered"/>
    /// answers whether a given cell is keep-out from edge to edge; a node is only ever
    /// marked Full on its word, never on the bounding box alone, so a Full verdict can
    /// be trusted as much as an Empty one.
    /// </summary>
    public void Mark(Rectangle region, Func<Rectangle, bool> isCovered)
    {
      if (isCovered == null)
      {
        throw new ArgumentNullException("isCovered");
      }
      if (!Surface.Intersects(region) || State == NodeState.Full)
      {
        return;
      }
      if (region.Contains(Surface) && isCovered(Surface))
      {
        State = NodeState.Full;
        Children = null;
        return;
      }
      if (Surface.Width <= GlobalConstants.MIN_QUADTREE_CELL || Surface.Height <= GlobalConstants.MIN_QUADTREE_CELL)
      {
        State = isCovered(Surface) ? NodeState.Full : NodeState.Mixed;
        return;
      }
      if (Children == null)
      {
        this.Subdivide();
      }
      bool allFull = true;
      for (int i = 0; i < GlobalConstants.TREE_CHILDREN_COUNT; i++)
      {
        Children[i].Mark(region, isCovered);
        if (Children[i].State != NodeState.Full)
        {
          allFull = false;
        }
      }
      if (allFull)
      {
        State = NodeState.Full;
        Children = null;
      }
      else
      {
        State = NodeState.Mixed;
      }
    }

    /// <summary>
    /// One traversal, three answers. Empty: every part of <paramref name="region"/> inside
    /// this node is untouched. Full: every part is keep-out edge to edge. Mixed: anything
    /// else, including a leaf too small to know better. Null when the region misses this
    /// node entirely, so a parent can combine children without counting the absent ones.
    /// </summary>
    public NodeState? Classify(Rectangle region)
    {
      if (!Surface.Intersects(region))
      {
        return null;
      }
      if (State == NodeState.Empty || State == NodeState.Full || Children == null)
      {
        return State;
      }
      NodeState? verdict = null;
      for (int i = 0; i < GlobalConstants.TREE_CHILDREN_COUNT; i++)
      {
        NodeState? child = Children[i].Classify(region);
        if (child == null)
        {
          continue;
        }
        if (child == NodeState.Mixed)
        {
          return NodeState.Mixed;
        }
        if (verdict == null)
        {
          verdict = child;
        }
        else if (verdict != child)
        {
          return NodeState.Mixed;
        }
      }
      return verdict ?? NodeState.Mixed;
    }

    /// <summary>True only when every part of <paramref name="region"/> inside this node is known to be untouched.</summary>
    public bool IsEmpty(Rectangle region)
    {
      NodeState? verdict = this.Classify(region);
      return verdict == null || verdict == NodeState.Empty;
    }

    /// <summary>True only when every part of <paramref name="region"/> inside this node is known to be keep-out from edge to edge.</summary>
    public bool IsFull(Rectangle region)
    {
      NodeState? verdict = this.Classify(region);
      return verdict == null || verdict == NodeState.Full;
    }

    public void Visit(Action<QuadTreeNode> action)
    {

      action(this);

      if (Children == null)
      {
        return;
      }
      for (int i = 0; i < GlobalConstants.TREE_CHILDREN_COUNT; i++)
      {
        Children[i].Visit(action);
      }
    }
    #endregion Public Methods

    #region Private Methods
    private void Subdivide()
    {
      // Rectangle takes (top, left, width, height); odd sizes give the remainder to the
      // right and bottom halves so the four children tile the surface exactly
      int halfWidth = Surface.Width / 2;
      int halfHeight = Surface.Height / 2;

      Children = new QuadTreeNode[GlobalConstants.TREE_CHILDREN_COUNT];
      Children[0] = new QuadTreeNode(new Rectangle(Surface.Top,              Surface.Left,             halfWidth,                 halfHeight));
      Children[1] = new QuadTreeNode(new Rectangle(Surface.Top,              Surface.Left + halfWidth, Surface.Width - halfWidth, halfHeight));
      Children[2] = new QuadTreeNode(new Rectangle(Surface.Top + halfHeight, Surface.Left,             halfWidth,                 Surface.Height - halfHeight));
      Children[3] = new QuadTreeNode(new Rectangle(Surface.Top + halfHeight, Surface.Left + halfWidth, Surface.Width - halfWidth, Surface.Height - halfHeight));
    }
    #endregion Private Methods

  }
}
