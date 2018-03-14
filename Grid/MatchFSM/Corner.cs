using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class CornerState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private State type;
    public State Type() { return type; }

    public CornerState(Lineup _attTeam, Lineup _defTeam)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      type = State.CORNER;
      attacker = attTeam.All().ByMax(x => x.SP);
    }
    public IState Play()
    {
      double mistakeProb = (20 - attacker.SP) / 45.0;

      if (StaticRandom.RandDouble() < mistakeProb)
        /***Mistake***/
        return new GoalKickState(defTeam, attTeam);
      else
        return new FreeKickCrossState(attTeam, defTeam, attacker);
    }
  }
}
