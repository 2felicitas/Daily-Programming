using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class OutboxReboundState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private Player defender;
    private State type;
    public State Type() {return type;}

    public OutboxReboundState(Lineup _attTeam, Lineup _defTeam)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      type = State.OUTBOX_REBOUND;
      attacker = attTeam.Any(Position.DM, Position.CM, Position.AM);
      defender = defTeam.Any(Position.DM, Position.CM);
    }
    public IState Play()
    {

      double reboundProb = attacker.PAC / (defender.PAC + 20.0);
      if (StaticRandom.RandDouble() < reboundProb)
        return new OutboxShotAttemptState(attTeam, defTeam, attacker, defender);
      else
        if (StaticRandom.RandDouble() < 0.5)
          return new AttackCentreState(defTeam, attTeam, "counter");
        else
          return new ClearenceState(defTeam, attTeam);
    }
  }
}
