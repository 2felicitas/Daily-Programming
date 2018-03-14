using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;
using System.IO;

namespace Schedule
{
  public static class Stats
  {
    public static double fk_goal=0;
    public static double fks=0;
    public static double fk_saves=0;
  }
  class SCHMain
  {
    static void Main(string[] args)
    {
      /*while (true)
      {
        string[] a = Console.ReadLine().Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries);
        double[] b = new double[a.Length];
        for (int i = 0; i < a.Length; i++)
          if (double.TryParse(a[i], out b[i]))
            b[i] /= 5.0;
          else
            break;
        if (b[0] > 20 && b.Length > 1)
          if (b.Length == 2)
            Console.WriteLine(b[0] + b[1] / 2.0 - 29.0);
          else
            Console.WriteLine((b[0] + b[1] / 2.0 - 29.0 + 2.0 * b[2]) / 3.0);
        else if (b.Sum(x => Math.Abs(x - b.Average())) >= 1 * b.Count())
        {
          double min = (b.Min() + b.Average()) / 2.0;
          double max = (b.Max() + b.Average()) / 2.0;
      
          Console.WriteLine((-min + 2.0 - max) / 4.0);
        }
        else
          Console.WriteLine(b.Average());
      }*/

      List<Team> teams = new List<Team>();
      string[] lines = File.ReadAllLines("BundesligaTeams.txt");
      //string[] lines = File.ReadAllLines("RFPLTeams.txt");
      int i = 1;
      int playerId = 0;
      while (i < lines.Count())
      {
        string teamName = lines[i++];
        teams.Add(new Team(teamName));
        while (i < lines.Count() && lines[i].Length > 0)
        {
          string[] player = lines[i].Split('|');
          teams.Last().AddPlayer(new Player(playerId++, player[0].Trim(), player[1].Trim()));
          i++;
        }
        while (i < lines.Count() && lines[i].Length == 0)
          i++;
      }
      teams = teams.OrderByDescending(x => x.Power).ToList();
      /*foreach (var team in teams)
      {
        Console.WriteLine("{0}  {1:f2}({2})", team.Name.PadRight(15),
                                              team.Power,
                                              team.Squad.Count());
        Console.WriteLine(team);
        Console.WriteLine();
        Console.ReadKey();
      }
      Console.ReadKey();

      foreach (var team in teams)
      {
        Lineup lineup = team.ChooseLineupHungarian();
        Console.WriteLine("{0}  {1:f2}/{2:f2}({3})", team.Name.PadRight(15),
                                                     lineup.Power, team.Power, 
                                                     team.Squad.Count());
        Console.WriteLine(lineup);
        Console.WriteLine();
        Console.ReadKey();
      }
      Console.ReadKey();*/


      teams.Shuffle();
      Season s = PlaySeason(teams);
      s.MakeTheTable();
      Console.WriteLine(s.PrintStats());

      //foreach (var r in s.schedule)
      // Console.WriteLine(r.ToString(teams.Max(x => x.Name.Length) + 1));
      while (true)
      {
        Console.WriteLine("\nНазвание команды для всех её матчей и статистики, Enter для выхода, all для всей таблицы, new для нового сезона, цифры для тура");
        string teamName = Console.ReadLine();
        if (teamName == "")
          break;
        else if (teamName.IsNumber())
        {
          Console.WriteLine(s.schedule[int.Parse(teamName)].ToString(s.LongestTeamName + 1));
          continue;
        }
        else if (teamName == "all")
        {
          Console.WriteLine(s.PrintStats());
          continue;
        }
        else if (teamName == "new")
        {
          s = PlaySeason(teams);
          s.MakeTheTable();
          Console.WriteLine(s.PrintStats());
          continue;
        }
        else if (!teams.Select(x => x.Name).Contains(teamName))
          continue;
        Console.WriteLine(teams.First(x => x.Name == teamName).ChooseLineupHungarian());
        foreach (var round in s.schedule)
          foreach (var match in round.matches)
            if (match.HomeTeam.Name == teamName || match.AwayTeam.Name == teamName)
            {
              Console.WriteLine(match.ToStringExtra(s.LongestTeamName + 1));
              Console.WriteLine();
              continue;
            }
        Console.WriteLine(s.PrintTable(teamName));
        Console.WriteLine();
      }
      
    }

    static Season PlaySeason(List<Team> teams)
    {
      Season s = new Season();
      s.MakeTheSchedule(teams);
      foreach (var round in s.schedule)
      {
        foreach (var match in round.matches)
          match.Play();
        Console.Clear();
        Console.WriteLine("Тур {0} из {1}", round.number, (teams.Count - 1) * 2);
      }
      Console.Clear();
      return s;
    }
   
  }
}
