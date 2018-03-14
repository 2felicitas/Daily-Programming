using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule
{
  class MatchDateCompare : IComparer<Match>
  {
    public int Compare(Match x, Match y)
    {
      if (x.Date > y.Date)
        return 1;
      else if (x.Date < y.Date)
        return -1;
      else
        return 0;
    }
  }
  class PositionComparer : IComparer<PlayerPositionPair>
  {
    public int Compare(PlayerPositionPair p1, PlayerPositionPair p2)
    {
      var compared = p1.Position.CompareTo(p2.Position);
      if (compared != 0)
        return compared;
      else
        return p1.Player.Name.CompareTo(p2.Player.Name);
    }
  }
}
