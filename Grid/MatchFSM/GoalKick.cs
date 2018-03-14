using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class GoalKickState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player goalkeeper;
    private State type;
    public State Type() {return type;}

    public GoalKickState(Lineup _attTeam, Lineup _defTeam)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      type = State.GOAL_KICK;
      goalkeeper = attTeam.Any(Position.GK);
    }
    public IState Play()
    {
      double accProb = goalkeeper.LPS / 20.1;

      if (StaticRandom.RandDouble() < accProb)
        return new CentrePitchHeaderState(attTeam, defTeam);
      else
        return new NoMomentsState(attTeam, defTeam);
    }
  }
}
