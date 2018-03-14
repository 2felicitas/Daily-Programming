using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PascalTriangle
{
  class PTMain
  {
    static int[] pads;

    static void Main(string[] args)
    {
      while (Console.ReadKey().Key == ConsoleKey.Enter)
      {
        Console.Clear();
        Console.WriteLine("Количество строк:");
        int n = int.Parse(Console.ReadLine());
        Console.Clear();
        List<List<ulong>> triangle = new List<List<ulong>>();
        pads = new int[n];
        triangle = GetTriangle(n);
        for (int j=0; j<n; j++)
        {
          for (int i = 0; i < triangle[j].Count; i++)
            Console.Write(triangle[j][i].ToString().PadLeft((i == 0) ? (pads[n - 1] - pads[j]) / 2 + 1 : triangle[j][i].ToString().Length + 1));
          Console.WriteLine();
        }
      }
    }

    static List<List<ulong>> GetTriangle(int n)
    {
      List<List<ulong>> triangle = new List<List<ulong>>();
      for (int i = 0; i < n; i++)
      {
        triangle.Add(new List<ulong>());
        triangle[i].Add(1);
        if (i == 0)
        {
          pads[0] = 1;
          continue;
        }
        pads[i] = 3;
        if (i > 1)
          for (int j = 0; j < triangle[i - 1].Count - 1; j++)
          {
            triangle[i].Add(triangle[i - 1][j] + triangle[i - 1][j + 1]);
            pads[i] += triangle[i].Last().ToString().Length + 1;
          }
        triangle[i].Add(1);
      }
      return triangle;
    }
  }
}
