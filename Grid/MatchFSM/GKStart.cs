using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class GKStartState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player goalkeeper;
    private State type;
    public State Type() {return type;}

    public GKStartState(Lineup _attTeam, Lineup _defTeam)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      type = State.GK_START;
      goalkeeper = attTeam.Any(Position.GK);
    }
    public IState Play()
    {
      double accProb = goalkeeper.LPS / 20.1;

      if (StaticRandom.RandDouble() < accProb)
        switch (StaticRandom.Rand(4))
        {
          case 0:
            return new AttackCentreState(attTeam, defTeam, "counter");
          case 1:
            return new AttackSideState(attTeam, defTeam, 'l', "counter");
          case 2:
            return new AttackSideState(attTeam, defTeam, 'r', "counter");
          case 3:
            return new CentrePitchHeaderState(attTeam, defTeam);
          default:
            return null;
        }
      else
        return new NoMomentsState(attTeam, defTeam);
    }
  }
}
