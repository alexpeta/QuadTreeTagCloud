using System;

namespace Core.Enums
{
  /// <summary>
  /// What a region of the canvas holds, as far as the quadtree knows.
  /// Empty is the only state that earns money: a candidate over an Empty region
  /// needs no bitwise test at all.
  /// </summary>
  public enum NodeState
  {
    Empty = 0,
    Mixed = 1,
    Full = 2
  }
}
