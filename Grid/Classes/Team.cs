using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Schedule.Classes
{
  using Pos = Position;

  public class Squad : IEnumerable
  {
    private List<Player> list;

    public Squad()
    {
      list = new List<Player>();
    }
    public Player this[int i] { get { return list[i]; } }
    public List<Player> FieldPlayers
    {
      get { return list.Where(x => !x.Positions.Contains(Pos.GK)).ToList(); }
    }
    public List<Player> Goalkeepers
    {
      get { return list.Where(x => x.Positions.Contains(Pos.GK)).ToList(); }
    }
    public void Add(Player p) { list.Add(p); }
    public Player Last { get { return list.Last(); } }
    public int Count() { return list.Count; }
    public int Count(Func<Player, bool> pred) { return list.Count(pred); }
    public double Power { get { return list.Average(x => x.BestOVR); } }

    public IEnumerator GetEnumerator()
    {
      return list.GetEnumerator();
    }
  }

  public class Team
  {
    #region Formations
    static List<List<Pos>> Formations = new List<List<Pos>>()
    {
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.CM, Pos.CF, Pos.CF}, //5-2-1-2   2DM 1CM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.CM, Pos.CM, Pos.CF, Pos.CF}, //5-1-2-2   1DM 2CM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.AM, Pos.CF, Pos.CF}, //5-2-1-2   2DM 1AM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.CM, Pos.CM, Pos.CM, Pos.CF, Pos.CF}, //5-3-2     Narrow
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.CM, Pos.CM, Pos.AM, Pos.CF, Pos.CF}, //5-2-1-2   2CM 1AM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.RM, Pos.LM, Pos.CF, Pos.CF}, //5-1-2-2   Wide
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.CM, Pos.RM, Pos.LM, Pos.CF, Pos.CF}, //5-3-2     Wide
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.RM, Pos.LM, Pos.CF}, //5-2-2-1   Wide
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.CM, Pos.CM, Pos.RM, Pos.LM, Pos.CF}, //5-4-1
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.CM, Pos.CM, Pos.CF}, //5-2-2-1   2DM 2CM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.AM, Pos.AM, Pos.CF}, //5-2-2-1   2DM 2AM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.CM, Pos.CM, Pos.AM, Pos.CF}, //5-4-1     Narrow diamond
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.RM, Pos.LM, Pos.AM, Pos.CF}, //5-4-1     Wide diamond
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.CM, Pos.RM, Pos.LM, Pos.AM, Pos.CF}, //5-3-1-1   Wide
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.CM, Pos.RM, Pos.LM, Pos.CF}, //5-1-3-1   Wide
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.CM, Pos.CM, Pos.CM, Pos.CF}, //5-1-3-1   Narrow
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.CM, Pos.CM, Pos.CF, Pos.CF, Pos.CF}, //5-2-3     2CM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.CF, Pos.CF, Pos.CF}, //5-2-3     2DM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.CM, Pos.CM, Pos.RM, Pos.LM, Pos.CF, Pos.CF}, //4-4-2
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.RM, Pos.LM, Pos.CF, Pos.CF}, //4-2-2-2   Wide
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.CM, Pos.CM, Pos.AM, Pos.CF, Pos.CF}, //4-4-2     Narrow diamond
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.RM, Pos.LM, Pos.AM, Pos.CF, Pos.CF}, //4-4-2     Wide diamond
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.CM, Pos.CM, Pos.CF, Pos.CF}, //4-2-2-2   Narrow
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.CM, Pos.RM, Pos.LM, Pos.CF, Pos.CF}, //4-1-3-2
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.CM, Pos.RM, Pos.LM, Pos.AM, Pos.CF, Pos.CF}, //4-3-1-2
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.CM, Pos.CM, Pos.CM, Pos.RM, Pos.LM, Pos.CF}, //4-5-1
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.RM, Pos.LM, Pos.AM, Pos.CF}, //4-2-3-1   2DM 1AM              
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.RM, Pos.LM, Pos.AM, Pos.AM, Pos.CF}, //4-1-2-2-1 Wide
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.DM, Pos.RM, Pos.LM, Pos.CF}, //4-3-2-1   Wide 3DM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.CM, Pos.CM, Pos.CM, Pos.AM, Pos.AM, Pos.CF}, //4-3-2-1   3CM 2AM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.CM, Pos.AM, Pos.AM, Pos.CF}, //4-3-2-1   2DM 2AM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.CM, Pos.CM, Pos.RM, Pos.LM, Pos.CF}, //4-1-4-1   1DM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.CM, Pos.RM, Pos.LM, Pos.CF}, //4-2-3-1   2DM 1CM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.DM, Pos.CM, Pos.CM, Pos.CF}, //4-3-2-1   3DM 2CM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.DM, Pos.AM, Pos.AM, Pos.CF}, //4-3-2-1   3DM 2AM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.CM, Pos.CM, Pos.DM, Pos.CF, Pos.CF, Pos.CF}, //4-3-3     Narrow
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.CM, Pos.RM, Pos.LM, Pos.CF, Pos.CF, Pos.CF}, //4-3-3     Wide
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.CM, Pos.CM, Pos.CF, Pos.CF, Pos.CF}, //4-1-2-3   Narrow
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.RM, Pos.LM, Pos.CF, Pos.CF, Pos.CF}, //4-1-2-3   Wide
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.CM, Pos.CF, Pos.CF, Pos.CF}, //4-2-1-3   2DM 1CM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.CM, Pos.RM, Pos.LM, Pos.AM, Pos.AM}, //4-1-3-2-0
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.CM, Pos.CM, Pos.RM, Pos.LM, Pos.AM}, //4-1-4-1-0
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.DM, Pos.CM, Pos.CM, Pos.AM}, //4-3-2-1-0 Narrow
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.DM, Pos.RM, Pos.LM, Pos.AM}, //4-3-2-1-0 Wide
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.CM, Pos.RM, Pos.LM, Pos.AM}, //4-2-3-1-0
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.RM, Pos.LM, Pos.AM, Pos.AM}, //4-2-2-2-0 Wide
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.CM, Pos.CM, Pos.AM, Pos.AM}, //4-2-2-2-0 Narrow
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.DM, Pos.DM, Pos.CM, Pos.RM, Pos.LM, Pos.CF, Pos.CF}, //3-2-3-2   Wide
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.CM, Pos.CM, Pos.CM, Pos.RM, Pos.LM, Pos.CF, Pos.CF}, //3-5-2
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.DM, Pos.DM, Pos.RM, Pos.LM, Pos.AM, Pos.CF, Pos.CF}, //3-2-2-1-2
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.CM, Pos.CM, Pos.RM, Pos.LM, Pos.AM, Pos.CF, Pos.CF}, //3-4-1-2  2CM
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.DM, Pos.CM, Pos.RM, Pos.LM, Pos.AM, Pos.CF, Pos.CF}, //3-1-3-1-2
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.DM, Pos.DM, Pos.CM, Pos.RM, Pos.LM, Pos.AM, Pos.CF}, //3-2-3-1-1
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.CM, Pos.CM, Pos.RM, Pos.LM, Pos.AM, Pos.AM, Pos.CF}, //3-4-2-1
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.DM, Pos.DM, Pos.RM, Pos.LM, Pos.AM, Pos.AM, Pos.CF}, //3-2-2-2-1
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.DM, Pos.CM, Pos.RM, Pos.LM, Pos.AM, Pos.AM, Pos.CF}, //3-1-3-2-1
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.DM, Pos.CM, Pos.CM, Pos.CM, Pos.RM, Pos.LM, Pos.CF}, //3-1-5-1
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.DM, Pos.CM, Pos.CM, Pos.RM, Pos.LM, Pos.CF, Pos.CF}, //3-1-4-2
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.DM, Pos.CM, Pos.CM, Pos.RM, Pos.LM, Pos.AM, Pos.CF}, //3-1-4-1-1
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.CM, Pos.CM, Pos.RM, Pos.LM, Pos.CF, Pos.CF, Pos.CF}, //3-4-3
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.DM, Pos.CM, Pos.RM, Pos.LM, Pos.CF, Pos.CF, Pos.CF}, //3-4-3     Narrow diamond
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.DM, Pos.RM, Pos.LM, Pos.AM, Pos.CF, Pos.CF, Pos.CF}, //3-4-3     Wide diamond
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.DM, Pos.DM, Pos.RM, Pos.LM, Pos.CF, Pos.CF, Pos.CF}, //3-2-2-3   Wide
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.DM, Pos.CM, Pos.RM, Pos.LM, Pos.CF, Pos.CF, Pos.CF}, //3-1-3-3
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.CM, Pos.CM, Pos.AM}, //5-2-2-1-0 Narrow
     new List<Pos>(){Pos.GK, Pos.CB, Pos.CB, Pos.CB, Pos.RB, Pos.LB, Pos.DM, Pos.DM, Pos.RM, Pos.LM, Pos.AM}  //5-2-2-1-0 Wide
    };
    #endregion

    public string Name { get; set; }
    public string Town { get; set; }
    public Squad Squad;

    public Team(string name, string town)
    {
      Name = name;
      Town = town;
      Squad = new Squad();
    }
    public Team(string name)
    {
      Name = name;
      Squad = new Squad();
    }
    public double Power { get { return Math.Round(Squad.Power, 2); } }

    private double Style()
    {
      var def = Squad.FieldPlayers.Where(x => x.Positions.Contains(Pos.CB) || 
                                              x.Positions.Contains(Pos.LB) || 
                                              x.Positions.Contains(Pos.RB) || 
                                              x.Positions.Contains(Pos.DM))
                                  .OrderBy(x => x.BestOVR)
                                  .Take(6)
                                  .Average(x => x.Positions.Max(y => x.OVRByPosition(y))); 
      var att = Squad.FieldPlayers.Where(x => x.Positions.Contains(Pos.LM) ||
                                              x.Positions.Contains(Pos.RM) ||
                                              x.Positions.Contains(Pos.CF) ||
                                              x.Positions.Contains(Pos.AM))
                                  .OrderBy(x => x.BestOVR)
                                  .Take(6)
                                  .Average(x => x.Positions.Max(y => x.OVRByPosition(y)));
      return def / att;
    }

    public void AddPlayer(Player p)
    {
      Squad.Add(p);
    }

    public Lineup ChooseLineupHungarian()
    {
      if (Squad.Count() < 11)
        return null;
      Lineup eleven = new Lineup();
      double minSum = double.MaxValue;
      List<Position> bestFormation = new List<Position>();
      int[] bestLineup = new int[11];

      foreach (var formation in Formations)
      {
        double[,] m = new double[Squad.Count(), Squad.Count()];
        //Creating the matrix
        int i = 0;
        foreach (Player p in Squad)
        {
          for (int j = 0; j < 11; j++)
            m[i, j] = 20 - p.OVRByPosition(formation[j]);
          for (int j = 11; j < Math.Max(formation.Count, Squad.Count()); j++)
            m[i, j] = 20;
          i++;
        }
        //Solving the system
        int[] answer = new HungarianAlgorithm(m).Solve().Take(11).ToArray();
        double sum = 0;
        for (int j = 0; j < 11; j++)
          sum += m[answer[j], j];

        var newsum = sum;
        //If team has more skilled attacking players, offensive formations are in priority and vice versa
        if (Style() < 1 &&
            formation.Count(x => x == Pos.CB || x == Pos.DM) > 3 &&
            formation.Count(x => x == Pos.CF || x == Pos.AM) < 3) 
          sum *= 1.005;
        if (Style() > 1 &&
            formation.Count(x => x == Pos.CF || x == Pos.AM) > 2)
          sum *= 1.005;
        //Updating the best formation if necessary
        if (sum < minSum)
        {
          minSum = sum;
          bestFormation = new List<Position>(formation);
          bestLineup = answer;  
        }
      }
      for (int i = 0; i < 11; i++)
        eleven.Add(Squad[bestLineup[i]], bestFormation[i]);
      return eleven;
    }
    public override string ToString()
    {
      StringBuilder s = new StringBuilder();
      foreach (Player pl in Squad)
      {
        s.AppendFormat("{0:f0}  {1}", Math.Round(pl.BestOVR * 5), pl.Name.PadRight(17));
        foreach (var pos in pl.Positions)
          s.AppendFormat(" {0}", pos);
        s.Append('\n');
      }
      return s.ToString();
    }
  }
}