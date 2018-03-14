using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class ClearenceState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private State type;
    public State Type() { return type; }

    public ClearenceState(Lineup _attTeam, Lineup _defTeam)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      type = State.CLEARENCE;
    }

    public IState Play()
    {
      double accProb = 0.33;

      if (StaticRandom.RandDouble() < accProb)
        return new CentrePitchHeaderState(attTeam, defTeam);
      else
        return new NoMomentsState(attTeam, defTeam);
    }
  }
}
