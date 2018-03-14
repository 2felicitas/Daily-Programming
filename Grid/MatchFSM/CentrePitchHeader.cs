using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class CentrePitchHeaderState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private Player defender;
    private State type;
    public State Type() { return type; }

    public CentrePitchHeaderState(Lineup _attTeam, Lineup _defTeam)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      type = State.CENTRE_PITCH_HEADER;
      attacker = attTeam.Any(Position.CF, Position.AM);
      defender = defTeam.Any(Position.CB, Position.DM);
    }

    public IState Play()
    {
      double successProb = 0.5 * (attacker.HEA + attacker.STR) / (defender.STR + defender.HEA);
      if (StaticRandom.RandDouble() < successProb)
        return new AttackCentreState(attTeam, defTeam);
      else
        return new NoMomentsState(attTeam, defTeam);
    }
  }
}
