using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class FreeKickShotState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private Player goalkeeper;
    private State type;
    public State Type() {return type;}

    public FreeKickShotState(Lineup _attTeam, Lineup _defTeam, Player _attacker)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      attacker = _attacker;
      type = State.FREE_KICK_SHOT;
      goalkeeper = defTeam.Any(Position.GK);
    }
    public IState Play()
    {
      Stats.fks++;
      double shotProb = attacker.SP / 25.0;
      double goalProb = (attacker.SP - goalkeeper.GK + 3) / 22.0;
      if (StaticRandom.RandDouble() > shotProb)
        /***Miss or shot into the wall***/
        switch (StaticRandom.Rand(3))
        {
          case 0:
            return new ClearenceState(defTeam, attTeam);
          case 1:
            return new OutboxReboundState(attTeam, defTeam);
          case 2:
            return new GoalKickState(defTeam, attTeam);
          default:
            return null;
        }
      if (StaticRandom.RandDouble() < goalProb)
      /***Shot on goal and GK doesn't save***/
      {
        Stats.fk_goal++;
        return new GoalState(attTeam, defTeam, attacker);
      }
      else
        /***GK saves***/
        Stats.fk_saves++;
        switch (StaticRandom.Rand(3))
        {
          case 0:
            return new GKStartState(defTeam, attTeam);
          case 1:
            return new InboxReboundState(attTeam, defTeam);
          case 2:
            return new CornerState(attTeam, defTeam);
          default:
            return null;
        }
    }
  }
}
