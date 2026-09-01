using Core.Cloud;
using Core.Geometry;
using System;
using System.Collections.Generic;

namespace ConsoleTests
{
  /// <summary>
  /// A 5x7 dot-matrix font, the kind every character LCD shipped with, scaled to the
  /// requested size. Real holes, real descender-free ugliness: enough for the console to
  /// prove that words interlock without ever touching, using no font engine at all.
  /// </summary>
  public class BlockLetterRasterizer : IGlyphRasterizer
  {
    private const int GLYPH_WIDTH = 5;
    private const int GLYPH_HEIGHT = 7;
    private const int ADVANCE = 6;

    private static readonly Dictionary<char, string[]> FONT = new Dictionary<char, string[]>
    {
      { 'A', new[] { ".###.", "#...#", "#...#", "#####", "#...#", "#...#", "#...#" } },
      { 'B', new[] { "####.", "#...#", "#...#", "####.", "#...#", "#...#", "####." } },
      { 'C', new[] { ".####", "#....", "#....", "#....", "#....", "#....", ".####" } },
      { 'D', new[] { "####.", "#...#", "#...#", "#...#", "#...#", "#...#", "####." } },
      { 'E', new[] { "#####", "#....", "#....", "####.", "#....", "#....", "#####" } },
      { 'F', new[] { "#####", "#....", "#....", "####.", "#....", "#....", "#...." } },
      { 'G', new[] { ".####", "#....", "#....", "#.###", "#...#", "#...#", ".####" } },
      { 'H', new[] { "#...#", "#...#", "#...#", "#####", "#...#", "#...#", "#...#" } },
      { 'I', new[] { "#####", "..#..", "..#..", "..#..", "..#..", "..#..", "#####" } },
      { 'J', new[] { "....#", "....#", "....#", "....#", "#...#", "#...#", ".###." } },
      { 'K', new[] { "#...#", "#..#.", "#.#..", "##...", "#.#..", "#..#.", "#...#" } },
      { 'L', new[] { "#....", "#....", "#....", "#....", "#....", "#....", "#####" } },
      { 'M', new[] { "#...#", "##.##", "#.#.#", "#...#", "#...#", "#...#", "#...#" } },
      { 'N', new[] { "#...#", "##..#", "#.#.#", "#..##", "#...#", "#...#", "#...#" } },
      { 'O', new[] { ".###.", "#...#", "#...#", "#...#", "#...#", "#...#", ".###." } },
      { 'P', new[] { "####.", "#...#", "#...#", "####.", "#....", "#....", "#...." } },
      { 'Q', new[] { ".###.", "#...#", "#...#", "#...#", "#.#.#", "#..#.", ".##.#" } },
      { 'R', new[] { "####.", "#...#", "#...#", "####.", "#.#..", "#..#.", "#...#" } },
      { 'S', new[] { ".####", "#....", "#....", ".###.", "....#", "....#", "####." } },
      { 'T', new[] { "#####", "..#..", "..#..", "..#..", "..#..", "..#..", "..#.." } },
      { 'U', new[] { "#...#", "#...#", "#...#", "#...#", "#...#", "#...#", ".###." } },
      { 'V', new[] { "#...#", "#...#", "#...#", "#...#", "#...#", ".#.#.", "..#.." } },
      { 'W', new[] { "#...#", "#...#", "#...#", "#.#.#", "#.#.#", "##.##", "#...#" } },
      { 'X', new[] { "#...#", "#...#", ".#.#.", "..#..", ".#.#.", "#...#", "#...#" } },
      { 'Y', new[] { "#...#", "#...#", ".#.#.", "..#..", "..#..", "..#..", "..#.." } },
      { 'Z', new[] { "#####", "....#", "...#.", "..#..", ".#...", "#....", "#####" } },
      { '0', new[] { ".###.", "#...#", "#..##", "#.#.#", "##..#", "#...#", ".###." } },
      { '1', new[] { "..#..", ".##..", "..#..", "..#..", "..#..", "..#..", ".###." } },
      { '2', new[] { ".###.", "#...#", "....#", "...#.", "..#..", ".#...", "#####" } },
      { '3', new[] { "#####", "...#.", "..#..", "...#.", "....#", "#...#", ".###." } },
      { '4', new[] { "...#.", "..##.", ".#.#.", "#..#.", "#####", "...#.", "...#." } },
      { '5', new[] { "#####", "#....", "####.", "....#", "....#", "#...#", ".###." } },
      { '6', new[] { "..##.", ".#...", "#....", "####.", "#...#", "#...#", ".###." } },
      { '7', new[] { "#####", "....#", "...#.", "..#..", ".#...", ".#...", ".#..." } },
      { '8', new[] { ".###.", "#...#", "#...#", ".###.", "#...#", "#...#", ".###." } },
      { '9', new[] { ".###.", "#...#", "#...#", ".####", "....#", "...#.", ".##.." } }
    };

    public Mask Rasterize(string text, double fontSize)
    {
      int pixel = Math.Max(1, (int)Math.Round(fontSize / GLYPH_HEIGHT));
      int width = (text.Length * ADVANCE - (ADVANCE - GLYPH_WIDTH)) * pixel;
      int height = GLYPH_HEIGHT * pixel;
      var mask = new Mask(width, height);

      for (int c = 0; c < text.Length; c++)
      {
        string[] glyph;
        if (!FONT.TryGetValue(char.ToUpperInvariant(text[c]), out glyph))
        {
          continue;   // unknown characters are blank, the way an LCD shows a box you can't see
        }
        for (int gy = 0; gy < GLYPH_HEIGHT; gy++)
        {
          for (int gx = 0; gx < GLYPH_WIDTH; gx++)
          {
            if (glyph[gy][gx] != '#')
            {
              continue;
            }
            int originX = (c * ADVANCE + gx) * pixel;
            int originY = gy * pixel;
            for (int py = 0; py < pixel; py++)
            {
              for (int px = 0; px < pixel; px++)
              {
                mask.Set(originX + px, originY + py);
              }
            }
          }
        }
      }
      return mask;
    }
  }
}
