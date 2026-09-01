using System;

namespace Core.Cloud
{
  /// <summary>
  /// A word the placer refused, with the reason. Nothing gets shrunk or squeezed to fit:
  /// a cloud that lies about its own weights is worse than one with a word missing.
  /// </summary>
  public class UnplacedWord
  {
    public Tag Tag { get; private set; }
    public double FontSize { get; private set; }
    public string Reason { get; private set; }
    public int Attempts { get; private set; }

    public UnplacedWord(Tag tag, double fontSize, string reason, int attempts)
    {
      if (tag == null)
      {
        throw new ArgumentNullException("tag");
      }
      if (string.IsNullOrWhiteSpace(reason))
      {
        throw new ArgumentException("a refusal needs a reason", "reason");
      }
      Tag = tag;
      FontSize = fontSize;
      Reason = reason;
      Attempts = attempts;
    }

    public override string ToString()
    {
      return string.Format("{0} ({1:0.#}pt): {2}", Tag.Text, FontSize, Reason);
    }
  }
}
