using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schedule.Classes
{
  public class Goal
  {
    string playerScored;
    string playerAssisted;
    string minute;
    bool ownGoal;
    bool home;
    bool penalty;
    public string PlayerScored 
    { get 
      { 
        return string.Format("{0}{1}", playerScored, ownGoal? "(og)" : ""); 
      }
    }
    //$"{playerScored}{ownGoal == "og"? "(og)" : ""}"
    public string PlayerAssisted { get { return playerAssisted; } }
    public bool Home { get { return home; } }
    public bool Penalty { get { return penalty; } }
    public bool OG { get { return ownGoal; } }
    public Goal(string m, Player ps, bool h, bool p, bool og, Player pa)
    {
      playerScored = ps.Name;
      minute = m;
      ownGoal = og;
      penalty = p;
      playerAssisted = pa == null? "" : pa.Name;
      home = h;
    }
    public override string ToString()
    {
      StringBuilder s = new StringBuilder();
      if (home)
        s.AppendFormat("{0}{1} {2}'",
                        penalty? "(p) " : ownGoal? "(og) " : "", 
                        playerScored, 
                        minute.ToString().PadLeft(2));
      //s.Append($"{penalty? "(p) " : ""}{ownGoal? "(og) " : ""}{playerScored} {minute}'");
      else
        s.AppendFormat("{0}' {1}{2}",
                        minute.ToString().PadRight(2), 
                        playerScored, 
                        penalty ? " (p)" : ownGoal ? " (og)" : "");
      //s.Append($"{minute}' {playerScored}{penalty? " (p)" : ""}{ownGoal? " (og)" : ""}");
      return s.ToString();
    }
  }

  public class MatchStats
  {
    private int homeGoals;
    private int awayGoals;
    private List<Goal> goals;
    public List<Goal> Goals { get { return goals; } }
    public int HomeGoals { get { return homeGoals; } }
    public int AwayGoals { get { return awayGoals; } }

    public MatchStats(Lineup h, Lineup a)
    {
      homeGoals = 0;
      awayGoals = 0;
      goals = new List<Goal>();
    }

    public void AddGoal(string m, Player ps, bool home, string parameter, Player pa)
    {
      goals.Add(new Goal(m, ps, home, parameter == "p", parameter == "og", pa));
      homeGoals += home ? 1 : 0;
      awayGoals += home ? 0 : 1;
    }
    public List<string> ToString(int n)
    {
      List<string> ss = new List<string>();
      int h = HomeGoals;
      int a = AwayGoals;
      var gh = goals.Where(x => x.Home).ToList();
      var ga = goals.Where(x => !x.Home).ToList();
      for (int i = 0; i < Math.Max(HomeGoals, AwayGoals); i++)
      {
        StringBuilder s = new StringBuilder();
        if (a > 0 && h > 0)
        {
          s.Append(gh[i].ToString().PadLeft(n + 16));
          s.Append(new string(' ', 3));
          s.Append(ga[i].ToString());
          a--;
          h--;
        }
        else if (h > 0)
        {
          s.Append(gh[i].ToString().PadLeft(n + 16));
          h--;
        }
        else
        {
          s.Append(new string(' ', n + 19));
          s.Append(ga[i].ToString());
          a--;
        }
        ss.Add(s.ToString());
      }
      return ss;
    }
  }
}
