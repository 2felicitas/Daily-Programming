using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class CentrePassState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private State type;
    private bool counter;
    public State Type() { return type; }

    public CentrePassState(Lineup _attTeam, Lineup _defTeam, Player _attacker, bool _counter)
    {
      attTeam = _attTeam;
      attacker = _attacker;
      defTeam = _defTeam;
      counter = _counter;
      if (counter)
        type = State.CENTRE_PASS_COUNTER;
      else
        type = State.CENTRE_PASS;
    }

    public IState Play()
    {
      double successProb = (2.25 * attacker.SPS + 5) / 100.0;
      if (counter)
        successProb += 2.25 * (20 - defTeam.Any(Position.CM, Position.DM).DEF / 6.0) / 100.0;
      else
        successProb += 2.25 * (20 - defTeam.Any(Position.CM, Position.DM).DEF / 4.0) / 100.0;

      if (StaticRandom.RandDouble() > successProb)
        /***Pass is intercepted***/
        switch (StaticRandom.Rand(3))
        {
          case 0:
          case 1:
            if (counter)
              return new AttackCentreState(defTeam, attTeam);
            else
              return new AttackCentreState(defTeam, attTeam, "counter");
          case 2:
            return new ClearenceState(defTeam, attTeam);
          default:
            return null;
        }
      else
        /***Not intercepted - pass***/
        if (counter)
          return new AttackCentreState(attTeam, defTeam, "counter");
        else
          return new AttackCentreState(attTeam, defTeam);
    }
  }
}
