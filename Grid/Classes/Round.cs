using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schedule.Classes
{
  public class Round
  {
    private static TimeSpan[] possibleStartTimes = 
      {
        new TimeSpan(19, 0, 0),
        new TimeSpan(16, 0, 0),
        new TimeSpan(14, 0, 0),
        new TimeSpan(21, 0, 0) 
      };
    public int number;
    public List<Match> matches;

    public Round(int n)
    {
      number = n;
      matches = new List<Match>();
    }
    public Round(int n, List<Match> l)
    {
      number = n;
      matches = new List<Match>(l);
    }

    public string DatesRange
    {
      get
      {
        return string.Format("{0:dd.MM.yyyy} - {1:dd.MM.yyyy}",
                              matches.Min(x => x.Date.Date).Date,
                              matches.Max(x => x.Date.Date).Date);
      }
    }

    public bool IsDateFree(DateTime d)
    {
      return matches.Count(x => x.Date.Date == d) < 4;
    }

    public void Add(Match m, DateTime d)
    {
      matches.Add(m);
    }
    public void Add(Team a, Team b, DateTime d)
    {
      DateTime m = d.AddDays(StaticRandom.Rand(-1, 3));
      while (!IsDateFree(m))
        m = d.AddDays(StaticRandom.Rand(-1, 3));
      d = m;
      int i = 0;
      m = d.AddHours(possibleStartTimes[i].Hours);
      while (matches.Any(x => x.Date == m))
        m = d.AddHours(possibleStartTimes[++i].Hours);
      matches.Add(new Match(a, b, m));
    }
    public Match this[int n]
    {
      get { return matches[n]; }
    }
    public string ToString(int n)
    {
      StringBuilder res = new StringBuilder();
      res.AppendFormat("{0} Тур   {1}\n", number, DatesRange);
      foreach (var m in matches)
        res.AppendLine(m.ToStringExtra(n));
      return res.AppendLine().ToString();
    }
  }
}
