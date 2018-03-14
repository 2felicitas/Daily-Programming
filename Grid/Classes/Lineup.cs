using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Schedule.Classes
{
  public class PlayerPositionPair
  {
    public Player Player {get; set;}
    public Position Position {get; set;}

    public PlayerPositionPair(Player pl, Position p)
    {
      Player = pl;
      Position = p;
    }
  }

  public class Lineup
  {
    private SortedSet<PlayerPositionPair> list;

    public Lineup()
    {
      list = new SortedSet<PlayerPositionPair>(new PositionComparer());
    }

    public double Power
    {
      get
      {
        double sum = 0;
        foreach (var pair in list)
          sum += pair.Player.OVRByPosition(pair.Position);
        return sum / list.Count;
      }
    }
    public bool IsTherePosition(Position p)
    {
      return list.Any(x => x.Position == p);
    }
    public Player Any(params Position[] p)
    {
      var col = list.Where(x => p.Contains(x.Position))
                    .Select(x => x.Player)
                    .ToList();
      if (col.Count() == 0)
        return null;
      else if (col.Count() == 1)
        return col.First();
      else
        return col.GetRandomElement();
    }
    public List<Player> All()
    {
      return list.Select(x => x.Player).ToList();
    }
    public List<Player> All(params Position[] p)
    {
      return list.Where(x => p.Contains(x.Position))
                 .Select(x => x.Player)
                 .ToList();
    }
    public List<Player> AllFieldPlayers()
    {
      return list.Where(x => x.Position != Position.GK)
                 .Select(x => x.Player)
                 .ToList();
    }
    public Position PlayerPosition(Player p)
    {
      return list.First(x => x.Player == p).Position;
    }
    public void Add(Player pl, Position p)
    {
      list.Add(new PlayerPositionPair(pl, p));
    }

    public override string ToString()
    {
      StringBuilder s = new StringBuilder();
      foreach (var pair in list)
        s.AppendFormat("{0} {1:f0}  {2} \n", 
                        pair.Position,
                        Math.Round(pair.Player.OVRByPosition(pair.Position) * 5),
                        pair.Player.Name);
      return s.ToString();
    }
  }
}
