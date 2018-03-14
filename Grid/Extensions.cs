using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule
{
  public static class Extensions
  {
    public static string ForMatch(this DateTime b)
    {
      return string.Format("{0:dd.MM HH:mm}", b);
    }
    public static Position String2Position(this string s)
    {
      switch (s)
      {
        case "GK":
          return Position.GK;
        case "CB":
          return Position.CB;
        case "RB":
          return Position.RB;
        case "LB":
          return Position.LB;
        case "DM":
          return Position.DM;
        case "CM":
          return Position.CM;
        case "LM":
          return Position.LM;
        case "RM":
          return Position.RM;
        case "AM":
          return Position.AM;
        case "CF":
          return Position.CF;
        default:
          return Position.GK;
      }
    }
    public static double Max(params double[] m)
    {
      return m.Max();
    }
    public static double AttributesDistribution(double a)
    {
      return a > 0 ? a : Math.Round(StaticRandom.RandDouble((-2 * a), (-2 * a + 2)), 1);
    }
    public static T GetRandomElement<T>(this IEnumerable<T> collection)
    {
      return collection.ElementAt(StaticRandom.Rand(collection.Count()));
    }
    public static T GetRandomElementWithWeights<T>(this IEnumerable<T> collection, IEnumerable<double> weights)
    {
      if (collection.Count() != weights.Count())
        return default(T);
      weights = weights.CumulativeSum();
      double prob = StaticRandom.RandDouble(0, weights.Last());
      for (int i = 0; i < weights.Count(); i++)
        if (prob < weights.ElementAt(i))
          return collection.ElementAt(i);
      return default(T);
    }
    public static T GetRandomElementWithWeights<T>(this IEnumerable<T> collection)
    {
      int n = collection.Count();
      int k = StaticRandom.Rand(n * (n + 1) / 2);
      for (int i = 0; i < n; i++)
        if (k < (i + 1) * (2 * n - i) / 2)
          return collection.ElementAt(i);
      return default(T);
    }
    public static T ByMax<T>(this IEnumerable<T> collection, Func<T, double> selector)
    {
      return collection.Aggregate((agg, Rand) => selector(agg) > selector(Rand) ? agg : Rand);
    }
    public static IEnumerable<double> CumulativeSum(this IEnumerable<double> list)
    {
      double sum = 0.0;
      foreach (var item in list)
      {
        sum += item;
        yield return sum;
      }
    }
    public static void Shuffle<T>(this IList<T> collection, int start, int end)
    {
      int n = end;
      while (n > start + 1)
      {
        n--;
        int k = StaticRandom.Rand(start, n + 1);
        T value = collection[k];
        collection[k] = collection[n];
        collection[n] = value;
      }
    }
    public static void Shuffle<T>(this IList<T> collection)
    {
      int n = collection.Count;
      while (n > 1)
      {
        n--;
        int k = StaticRandom.Rand(n + 1);
        T value = collection[k];
        collection[k] = collection[n];
        collection[n] = value;
      }
    }
    public static bool IsNumber(this string s)
    {
      int b;
      return int.TryParse(s, out b);
    }
  }
}
