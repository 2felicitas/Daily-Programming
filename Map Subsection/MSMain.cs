using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Map_Subsection
{
  static class MSMain
  {
    static char[] sep = {',', '(', ')', '[', ']', ' '};

    static void Main(string[] args)
    {
      string[] input = File.ReadAllLines("input.txt");
      foreach (string line in input)
      {
        Console.WriteLine(MakeSubsection(line));
        Console.WriteLine();
      }
      Console.ReadKey();
    }

    static string MakeSubsection(string line)
    {
      string[] inputs = line.Split(sep, StringSplitOptions.RemoveEmptyEntries);
      int[] points = Array.ConvertAll(inputs, int.Parse);
      int minLeft   = points.GetNth(1, 2).Min();
      int maxRight  = points.GetNth(1, 2).Max();
      int maxTop    = points.GetNth(2, 2).Max();
      int minBottom = points.GetNth(2, 2).Min();
      int bigSize = points[0];
      int margin = 30;
      int smallSize = Math.Max(maxTop - minBottom, maxRight - minLeft);
      int leftCornerX = minLeft;
      int leftCornerY = minBottom;

      if (smallSize + 2 * margin >= bigSize || smallSize + 2 * margin >= bigSize)
        return string.Format("({0}, {1}), {2}", 0, 0, bigSize);
      leftCornerX = AdjustTheBorders(leftCornerX, smallSize, margin, bigSize);
      leftCornerY = AdjustTheBorders(leftCornerY, smallSize, margin, bigSize);
      smallSize += 2 * margin;

      int centerX = minLeft + (maxRight - minLeft) / 2;
      int centerY = minBottom + (maxTop - minBottom) / 2;
      if (leftCornerX + smallSize / 2 > centerX)
        leftCornerX = Math.Max(centerX - smallSize / 2, 0);
      else
        if (centerX + smallSize / 2 < bigSize)
          leftCornerX = centerX + smallSize / 2;
        else
          leftCornerX = bigSize - smallSize;
      if (leftCornerY + smallSize / 2 > centerY)
        leftCornerY = Math.Max(centerY - smallSize / 2, 0);
      else
        if (centerY + smallSize / 2 < bigSize)
          leftCornerY = centerY + smallSize / 2;
        else
          leftCornerY = bigSize - smallSize;

      return string.Format("({0}, {1}), {2}", leftCornerX, leftCornerY, smallSize);
    }

    static int AdjustTheBorders (int initialCoord, int size, int margin, int border)
    {
      if (border == 0)
        return (initialCoord - margin < border)? border: initialCoord;
      else
        return (initialCoord + size + margin > border)? border - size - 2 * margin: initialCoord;
    }

    static IEnumerable<T> GetNth<T>(this IEnumerable<T> list, int startPoint, int n)
    {
      for (int i = startPoint; i < list.Count(); i += n)
        yield return list.ElementAt(i);
    }
  }
}
