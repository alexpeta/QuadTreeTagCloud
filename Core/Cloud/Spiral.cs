using System;
using System.Collections.Generic;

namespace Core.Cloud
{
  public struct SpiralPoint
  {
    public readonly int X;
    public readonly int Y;

    public SpiralPoint(int x, int y)
    {
      X = x;
      Y = y;
    }
  }

  /// <summary>
  /// Where to try. An Archimedean spiral r = pitch * theta walked outward from a centre,
  /// stepped so consecutive candidates are about <c>step</c> pixels apart. It is nothing
  /// more than a cheap enumerator of "the closest untried spot to the middle", and that
  /// ordering is the whole aesthetic: big words inward, edges that look grown.
  /// </summary>
  public static class Spiral
  {
    public static IEnumerable<SpiralPoint> Walk(int centerX, int centerY, double pitch, double step, double maxRadius)
    {
      if (pitch <= 0 || step <= 0 || maxRadius <= 0)
      {
        throw new ArgumentOutOfRangeException("pitch, step and maxRadius must all be positive");
      }

      double theta = 0;
      int lastX = int.MinValue;
      int lastY = int.MinValue;

      while (true)
      {
        double r = pitch * theta;
        if (r > maxRadius)
        {
          yield break;
        }
        int x = (int)Math.Round(centerX + r * Math.Cos(theta));
        int y = (int)Math.Round(centerY + r * Math.Sin(theta));
        if (x != lastX || y != lastY)
        {
          lastX = x;
          lastY = y;
          yield return new SpiralPoint(x, y);
        }
        theta += step / Math.Max(r, step);
      }
    }
  }
}
