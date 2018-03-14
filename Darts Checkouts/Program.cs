using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Darts_Checkouts
{
  static class Points
  {
    static public SortedSet<int> sectors = new SortedSet<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 25 };
    static public SortedSet<int> doubles = new SortedSet<int> { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 36, 38, 40, 50 };
    static public SortedSet<int> triples = new SortedSet<int> { 3, 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36, 39, 42, 45, 48, 51, 54, 57, 60 };
  }
  class Program
  {
    static void Main(string[] args)
    {
      while(true)
      {
        Console.WriteLine("Впишите оставшееся количество очков");
        int sum = int.Parse(Console.ReadLine());
        List<Three> a = Three(sum);
        if (a != null && a.Any())
          foreach (var checkout in a)
            foreach (var line in checkout.ToString())
              Console.WriteLine(line);
        else
          Console.WriteLine("Нет окончания");
        Console.WriteLine();
      }
    }
    static List<Three> Three(int sum)
    {
      if (sum > 170 || sum < 2)
        return null;
      List<Three> answer = new List<Three>();
      List<int> allpoints = Points.sectors.Union(Points.doubles).Union(Points.triples).OrderByDescending(x => x).ToList();
      bool end = false;
      for (int i = allpoints.FindIndex(x => x < sum); i < allpoints.Count && !end; i++)
      {
        if (Points.doubles.Contains(sum))
        {
          answer.Add(new Three(sum));
          break;
        }
        var first = allpoints.ElementAt(i);
        if (sum - first > 110)
          break;
        for (int j = allpoints.FindIndex(x => x <= sum - first); j < allpoints.Count && !end; j++)
        {
          if (Points.doubles.Contains(sum - first))
          {
            answer.Add(new Three(first, sum - first));
            break;
          }
          var second = allpoints.ElementAt(j);
          if (sum - first - second > 50)
            break;
          var third = sum - first - second;
          if (!answer.Any(x => x.Third == -1) && 
              !answer.Any(x => x.First == second && x.Second == first) && 
              Points.doubles.Contains(third))
            answer.Add(new Three(first, second, third));
        }
      }
      if (answer.Any(x => x.Third == -1))
        return answer.Where(x => x.Third == -1).ToList();
      return answer;
    }
  }

  class Three
  {
    private int first = -1;
    private int second = -1;
    private int third = -1;

    public int First {get {return first;}}
    public int Second { get { return second; } }
    public int Third { get { return third; } }

    public Three(int f, int s = -1, int t = -1)
    {
      first = f;
      second = s;
      third = t;
    }
    new public List<string> ToString()
    {
      List<string> answer = new List<string>();
      if (second == -1)
        return Point2Str(first, true);
      var firsts = Point2Str(first);
      if (third == -1)
        return firsts.SelectMany(x => Point2Str(second, true), (y, z) => {return y + ' ' + z;}).ToList();
      var seconds = firsts.SelectMany(x => Point2Str(second), (y, z) => { return y + ' ' + z; }).ToList();
      var thirds = Point2Str(third, true);
      return seconds.SelectMany(x => thirds, (y, z) => { return y + ' ' + z; }).ToList();
    }

    List<string> Point2Str(int i, bool lastdouble = false)
    {
      if (i == 50)
        return new List<string> { "B" };
      List<string> answer = new List<string>();
      if (!lastdouble && Points.sectors.Contains(i))
        answer.Add(i.ToString());
      if (Points.doubles.Contains(i))
        answer.Add((i / 2).ToString() + 'D');
      if (!lastdouble && Points.triples.Contains(i))
        answer.Add((i / 3).ToString() + 'T');
      return answer;
    }
  }
}
