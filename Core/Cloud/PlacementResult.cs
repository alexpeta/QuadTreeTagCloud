using Core.DataStructures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.Cloud
{
  public class PlacementResult
  {
    public ReadOnlyCollection<PlacedWord> Placed { get; private set; }
    public ReadOnlyCollection<UnplacedWord> Unplaced { get; private set; }
    public OccupancyBoard Board { get; private set; }
    public long ElapsedMilliseconds { get; private set; }

    public PlacementResult(IList<PlacedWord> placed, IList<UnplacedWord> unplaced, OccupancyBoard board, long elapsedMilliseconds)
    {
      if (placed == null || unplaced == null || board == null)
      {
        throw new ArgumentNullException("a result needs its placed list, its refusals and its board");
      }
      Placed = new ReadOnlyCollection<PlacedWord>(placed);
      Unplaced = new ReadOnlyCollection<UnplacedWord>(unplaced);
      Board = board;
      ElapsedMilliseconds = elapsedMilliseconds;
    }
  }
}
