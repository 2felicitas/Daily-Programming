using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class BoxCrossState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private Player defender;
    private Player goalkeeper;
    private Player assist;
    private State type;
    public State Type() { return type; }

    public BoxCrossState(Lineup _attTeam, Lineup _defTeam, Player _assist)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      assist = _assist;
      type = State.BOX_CROSS;
      attacker = attTeam.All(Position.AM, Position.CF)
                        .OrderByDescending(x => x.HEA)
                        .GetRandomElementWithWeights();
      defender = defTeam.Any(Position.CB);
      goalkeeper = defTeam.Any(Position.GK);
    }

    public IState Play()
    {
      if (StaticRandom.RandDouble() < goalkeeper.HEA / 150.0)
        /***GK interceptes/cleares/to the corner***/
        switch (StaticRandom.Rand(3))
        {
          case 0:
            return new GKStartState(defTeam, attTeam);
          case 1:
            return new ClearenceState(defTeam, attTeam);
          case 2:
            return new CornerState(attTeam, defTeam);
          default:
            return null;
        }
      double shotProb = 2.0;
      double clearProb = 1.0;
      double ownShotProb = 0.05;

      shotProb += (attacker.HEA + attacker.STR - 20) / 20.0;
      clearProb += (defender.HEA + defender.STR - 20) / 20.0;

      clearProb += shotProb;
      ownShotProb += clearProb;

      double p = StaticRandom.RandDouble(0, ownShotProb);
      if (p < shotProb)
        /***Header**/
        return new ShotState(attTeam, defTeam, attacker, "header", assist);
      else if (p < clearProb)
        /***Defender cleares***/
        return new ClearenceState(defTeam, attTeam);
      else
        /***Own shot***/
        return new OwnShotState(attTeam, defTeam, defender);
    }
  }
}
