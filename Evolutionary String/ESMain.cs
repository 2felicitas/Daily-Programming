using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Evolutionary_String
{
  class ESMain
  {
    static int popSize = 500;
    static int maxGen = 4000;
    static double range = 0.1;

    static void Main(string[] args)
    {
      int m = '҈';
      Console.OutputEncoding = Encoding.UTF8;
      for (int i = 0; i < 1; i++)
      {
        if (Console.ReadKey().Key == ConsoleKey.Enter)
          Evolve("*************************************************************************\n" +
                 "Этот алгоритм определяет исходную фразу с помощью генетического алгоритма\n" +
                 "*************************************************************************");
        else
          if (Console.ReadKey().Key == ConsoleKey.N)
            Evolve("҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈\n" +
                   "҈ @@@ @@@ @  @ @@@    @  @ @  @ @@@  @@@ @@@ ҈\n" +
                   "҈ @   @   @@ @ @  @   @@ @ @  @ @  @ @   @   ҈\n" +
                   "҈ @@@ @@@ @@@@ @  @   @@@@ @  @ @  @ @@@ @@@ ҈\n" +
                   "҈   @ @   @ @@ @  @   @ @@ @  @ @  @ @     @ ҈\n" +
                   "҈ @@@ @@@ @  @ @@@    @  @  @@  @@@  @@@ @@@ ҈\n" +
                   "҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈҈");  
          else
            Evolve("Этот алгоритм определяет исходную фразу с помощью генетического алгоритма");
        //Console.Clear();
        //Console.WriteLine(new string('@', 50 - (200 - i) / 4) + new string('░', (200 - i) / 4));
      }
      Console.ReadKey();
    }

    static int Fitness(string first, string second)
    {
      int fit = 0;
      for (int i = 0; i < first.Length; i++)
      {
        fit += Math.Abs((int)first[i] - (int)second[i]);
      }
      return fit;
    }

    static char RandChar(int min, int max)
    {
      return (char)StaticRandom.Next(min, max);
    }

    static string RandString(int length)
    {
      StringBuilder a = new StringBuilder();
      for (int i = 0; i < length; i++)
        a.Append(RandChar(32, 1500));
      return a.ToString();
    }

    static string Child(string first, string second)
    {
      StringBuilder child = new StringBuilder();
      for (int i = 0; i < first.Length; i++)
      {
        if (StaticRandom.NextDouble() < range)
          child.Append(RandChar(Math.Min((int)first[i],(int)second[i]) - 30, 
                                Math.Max((int)first[i],(int)second[i]) + 30));
        else
          child.Append((first[i].ToString() + 
                       second[i].ToString())[StaticRandom.Next(0, 2)]);
      }
      return child.ToString();
    }

    static void Evolve(string target)
    {
      range = 1.0/target.Length;
      List<string> population = new List<string>();
      for (int i = 0; i < popSize; i++)
      {
        population.Add(RandString(target.Length));
      }
      int lowfit = int.MaxValue;
      int genNumber = 1;
      while (genNumber < maxGen)
      {
        List<string> leaders = population.OrderBy(x => Fitness(x, target)).Take(4).ToList();
        lowfit = Math.Min(Fitness(leaders[0], target),lowfit);
        string g = String.Format("Gen: " + "{0}" + "| Fitness:" + "{1}" + "| {2}\r\n",
                                 genNumber.ToString().PadRight(3),
                                 Fitness(leaders[0], target).ToString().PadLeft(5),
                                 leaders[0]);
        //Console.Write(g);
        Console.Clear();
        Console.WriteLine(leaders[0] + "\n\n" + Fitness(leaders[0], target).ToString() + "   " + lowfit.ToString());
        if (Fitness(leaders[0], target) == 0)
        {
          Console.WriteLine("\n\nSUCCESS\n\n{0}\n\nGeneration: {1}", leaders[0], genNumber);
          break;
        }
        else
        {
          for (int i = 0; i < popSize; i += 4)
          {
            population[i]     = Child(leaders[0], leaders[1]);
            population[i + 1] = Child(leaders[0], leaders[2]);
            population[i + 2] = Child(leaders[0], leaders[3]);
            population[i + 3] = Child(leaders[1], leaders[2]);
          }
        }
        genNumber++;
        //Thread.Sleep(10);
      }
    }
    public static class StaticRandom
    {
      static int seed = Environment.TickCount;
      static readonly ThreadLocal<Random> random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));
      static double prevGauss = 0;
      static bool   isStored = false;
      public static int Next()
      {
        return random.Value.Next();
      }
      public static int Next(int maxValue)
      {
        return random.Value.Next(maxValue);
      }
      public static int Next(int minValue, int maxValue)
      {
        return random.Value.Next(minValue, maxValue);
      }
      public static double NextDouble()
      {
        return random.Value.NextDouble();
      }
      public static double NextDouble(double minValue, double maxValue)
      {
        return random.Value.NextDouble() * (maxValue - minValue) + minValue;
      }
      public static double NextGaussian(double mu = 0, double sigma = 1)
      {
        double x, u, v, s;
        if (isStored)
        {
          isStored = !isStored;
          return prevGauss * sigma + mu;
        }
        do
        {
          u = 2 * StaticRandom.NextDouble() - 1;
          v = 2 * StaticRandom.NextDouble() - 1;
          s = u * u + v * v;
        } while (s >= 1 || s == 0);
        x = Math.Sqrt(-2 * Math.Log(s) / s);
        prevGauss = x * u;
        isStored = !isStored;
        return x * v * sigma + mu;
      }
      public static double NextGaussian(double mu, double sigma, double minValue, double maxValue)
      {
        double x;
        do
        {
          x = NextGaussian(mu, sigma);
        } while (x > maxValue || x < minValue);
        return x;
      }
    }
  }
}
