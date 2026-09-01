using System;

namespace Core.Cloud
{
  /// <summary>A word and how much it matters. Weight is relative; the placer scales it.</summary>
  public class Tag
  {
    public string Text { get; private set; }
    public double Weight { get; private set; }

    public Tag(string text, double weight)
    {
      if (string.IsNullOrWhiteSpace(text))
      {
        throw new ArgumentException("a tag needs text", "text");
      }
      if (weight < 0 || double.IsNaN(weight) || double.IsInfinity(weight))
      {
        throw new ArgumentOutOfRangeException("weight", "weight must be a finite non-negative number");
      }
      Text = text;
      Weight = weight;
    }

    public override string ToString()
    {
      return string.Format("{0} ({1})", Text, Weight);
    }
  }
}
