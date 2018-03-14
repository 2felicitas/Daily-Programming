using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;
using Schedule.FSM;

namespace Schedule.Classes
{

  public class Match
  {
    private DateTime date;
    private Team homeTeam;
    private Team awayTeam;
    private Lineup homeLineup;
    private Lineup awayLineup;
    private bool hasPlayed;
    private MatchStats Stats = null;
    public DateTime Date { get { return date; } }
    public Team HomeTeam { get { return homeTeam; } }
    public Team AwayTeam { get { return awayTeam; } }
    public Lineup HomeLineup { get { return homeLineup; } }
    public Lineup AwayLineup { get { return awayLineup; } }
    public bool HasPlayed { get { return  hasPlayed; } }
    public int HomeGoals { get { return Stats == null ? 0 : Stats.HomeGoals; } }
    public int AwayGoals { get { return Stats == null ? 0 : Stats.AwayGoals; } }
    public List<Goal> Goals { get { return Stats == null ? null : Stats.Goals; } }

    public Match(Team h, Team a, DateTime d)
    {
      hasPlayed = false;
      homeTeam = h;
      awayTeam = a;
      date = d;
    }
    public void SwapHome()
    {
      var temp = HomeTeam;
      homeTeam = AwayTeam;
      awayTeam = temp;
    }
    public bool isTeams(Team t1, Team t2)
    {
      return (t1 == HomeTeam && t2 == AwayTeam) || (t1 == AwayTeam && t2 == HomeTeam);
    }
    public void Play()
    {
      homeLineup = HomeTeam.ChooseLineupHungarian();
      awayLineup = AwayTeam.ChooseLineupHungarian();
      Stats = new MatchFSM(HomeLineup, AwayLineup).Start();
      hasPlayed = true;
    }
    public string ToString(int n)
    {
      return string.Format("{0}  {1} {2} : {3} {4}", 
                            date.ForMatch(),
                            homeTeam.Name.PadLeft(n),
                            Stats == null  ? "  " : Stats.HomeGoals.ToString().PadLeft(2),
                            Stats == null  ? "  " : Stats.AwayGoals.ToString().PadRight(2),
                            awayTeam.Name);
    }
    public string ToStringExtra(int n)
    {
      if (Stats == null)
        return ToString(n);
      else
      {
        StringBuilder s = new StringBuilder();
        s.AppendLine(this.ToString(n));
        foreach (var line in Stats.ToString(n))
          s.AppendLine(line);
        return s.ToString();
      }
    }
  }
}
