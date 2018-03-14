using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schedule.Classes
{
  public class Season
  {
    public class Row
    {
      public int W;
      public int D;
      public int L;
      public int Points;
      public int ScoredGoals;
      public int ConcededGoals;
    }

    public class GoalsStats
    {
      private int goals;
      private int penalties;
      public int Goals { get { return goals; } }
      public int Penalties { get { return penalties; } }
      public GoalsStats(Goal g)
      {
        goals = 1;
        penalties = g.Penalty ? 1 : 0;
      }
      public void Add(Goal g)
      {
        goals++;
        penalties += g.Penalty ? 1 : 0;
      }
    }

    private int longestTeamName;
    public Round[] schedule;
    private Dictionary<string, Row> table;
    private Dictionary<string, GoalsStats> scorers;
    private Dictionary<string, int> assisters;
    public int LongestTeamName { get { return longestTeamName; } }

    public void MakeTheSchedule(List<Team> teams)
    {
      DateTime date = new DateTime(2017, 8, 12);
      longestTeamName = teams.Max(x => x.Name.Length);
      int nOfRounds = (teams.Count - 1) * 2;
      int nOfTeams = teams.Count;
      int[] consecutiveHomeGames = Enumerable.Repeat(0, nOfTeams).ToArray();
      schedule = new Round[nOfRounds];
      int[] teamNs = Enumerable.Range(0, nOfTeams).ToArray();

      for (int i = 0; i < nOfRounds / 2; i++)
      {
        schedule[i] = new Round(i + 1);
        schedule[nOfRounds - 1 - i] = new Round(nOfRounds - i);
        for (int j = 0; j < nOfTeams / 2; j++)
        {
          schedule[i].Add(teams[teamNs[j]], teams[teamNs[nOfTeams - 1 - j]], date);
          if (consecutiveHomeGames[teamNs[j]] >= 1 &&
              consecutiveHomeGames[teamNs[nOfTeams - 1 - j]] <= 1)
          {
            schedule[i][j].SwapHome();
            consecutiveHomeGames[teamNs[j]] = 0;
            consecutiveHomeGames[teamNs[nOfTeams - 1 - j]]++;
          }
          else
          {
            consecutiveHomeGames[teamNs[j]]++;
            consecutiveHomeGames[teamNs[nOfTeams - 1 - j]] = 0;
          }
        }
        schedule[i].matches.Sort(new MatchDateCompare());
        do
          date = date.AddDays(7);
        while (IsRedZone(date));


        var temp = teamNs.Last();
        for (int j = nOfTeams - 1; j > 1; j--)
          teamNs[j] = teamNs[j - 1];
        teamNs[1] = temp;
      }
      for (int i = nOfRounds / 2; i < nOfRounds; i++)
      {
        for (int j = 0; j < nOfTeams / 2; j++)
          schedule[i].Add(schedule[i - nOfRounds / 2][j].AwayTeam,
                          schedule[i - nOfRounds / 2][j].HomeTeam, date);
        schedule[i].matches.Sort(new MatchDateCompare());
        do
          date = date.AddDays(7);
        while (IsRedZone(date));
      }
    }

    public void MakeTheTable()
    {
      scorers = new Dictionary<string, GoalsStats>();
      assisters = new Dictionary<string, int>();
      table = new Dictionary<string, Row>();
      foreach (var r in schedule[0].matches)
      {
        table.Add(r.HomeTeam.Name, new Row());
        table.Add(r.AwayTeam.Name, new Row());
      }
      foreach (var r in schedule)
      {
        if (r.matches[0].Goals == null)
          break;
        foreach (var m in r.matches)
        {
          foreach (var goal in m.Goals)
          {
            if (scorers.ContainsKey(goal.PlayerScored))
              scorers[goal.PlayerScored].Add(goal);
            else
              scorers.Add(goal.PlayerScored, new GoalsStats(goal));
            if (goal.PlayerAssisted != "")
            {
              if (assisters.ContainsKey(goal.PlayerAssisted))
                assisters[goal.PlayerAssisted]++;
              else
                assisters.Add(goal.PlayerAssisted, 1);
            }
          }
          table[m.HomeTeam.Name].ScoredGoals += m.HomeGoals;
          table[m.AwayTeam.Name].ScoredGoals += m.AwayGoals;
          table[m.HomeTeam.Name].ConcededGoals += m.AwayGoals;
          table[m.AwayTeam.Name].ConcededGoals += m.HomeGoals;
          if (m.HomeGoals > m.AwayGoals)
          {
            table[m.HomeTeam.Name].W++;
            table[m.AwayTeam.Name].L++;
            table[m.HomeTeam.Name].Points += 3;
          }
          else if (m.HomeGoals < m.AwayGoals)
          {
            table[m.AwayTeam.Name].W++;
            table[m.HomeTeam.Name].L++;
            table[m.AwayTeam.Name].Points += 3;
          }
          else
          {
            table[m.HomeTeam.Name].D++;
            table[m.AwayTeam.Name].D++;
            table[m.HomeTeam.Name].Points++;
            table[m.AwayTeam.Name].Points++;
          }
        }
      }
      table = table.OrderByDescending(x => x.Value.Points)
                   .ThenByDescending(x => x.Value.W)
                   .ThenByDescending(x => x.Value.ScoredGoals)
                   .ThenBy(x => x.Value.ConcededGoals)
                   .ToDictionary(x => x.Key, x => x.Value);
    }

    public string PrintTable(string highlightTeam = "")
    {
      StringBuilder s = new StringBuilder();
      int n = longestTeamName + 5;
      for (int i = 0; i < table.Count; i++)
      {
        s.Append((i+1).ToString().PadLeft(2));
        s.Append(". ");
        s.Append(table.ElementAt(i).Key.PadRight(n));
        s.Append(table.ElementAt(i).Value.W.ToString().PadLeft(3));
        s.Append(table.ElementAt(i).Value.D.ToString().PadLeft(3));
        s.Append(table.ElementAt(i).Value.L.ToString().PadLeft(3));
        s.Append(table.ElementAt(i).Value.ScoredGoals.ToString().PadLeft(4));
        s.Append(table.ElementAt(i).Value.ConcededGoals.ToString().PadLeft(4));
        s.Append(table.ElementAt(i).Value.Points.ToString().PadLeft(4));
        if (table.ElementAt(i).Key == highlightTeam)
          s.Append("<");
        s.Append('\n');
      }
      return s.ToString();
    }

    public string PrintTopScorers()
    {
      StringBuilder s = new StringBuilder();
      var top5 = scorers.OrderByDescending(x => x.Value.Goals).ToList();
      top5 = top5.TakeWhile(x => x.Value.Goals >= top5[4].Value.Goals).ToList();
      int n = top5.Max(x => x.Key.Length) + 2;
      foreach (var line in top5)
      {
        s.Append(line.Key.PadRight(n));
        s.Append(line.Value.Goals.ToString().PadLeft(2));
        s.AppendFormat(" ({0})", line.Value.Penalties);
        s.Append('\n');
      }
      return s.ToString();
    }

    public string PrintTopAssisters()
    {
      StringBuilder s = new StringBuilder();
      var top5 = assisters.OrderByDescending(x => x.Value).ToList();
      top5 = top5.TakeWhile(x => x.Value >= top5[4].Value).ToList();
      int n = top5.Max(x => x.Key.Length);
      foreach (var line in top5)
      {
        s.Append(line.Key.PadRight(n + 2));
        s.Append(line.Value.ToString().PadLeft(2));
        s.Append('\n');
      }
      return s.ToString();
    }

    public string PrintStats()
    {
      StringBuilder s = new StringBuilder(PrintTable());
      s.Append("\n");
      s.Append(PrintTopScorers());
      s.Append("\n");
      s.Append(PrintTopAssisters());
      return s.ToString();
    }

    bool IsRedZone(DateTime d)
    {
      return (d.Month == 12 && d.Day >= 10) ||
             (d.Month == 1) || 
             (d.Month == 2 && d.Day < 20);
    }
  }
}
