using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class FlankPassState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private State type;
    private bool counter;
    public State Type() { return type; }

    public FlankPassState(Lineup _attTeam, Lineup _defTeam, Player _attacker, bool _counter)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      attacker = _attacker;
      counter = _counter;
      if (counter)
        type = State.FLANK_PASS_COUNTER;
      else
        type = State.FLANK_PASS;
    }
    public IState Play()
    {
      double successProb = (2 * (attacker.SPS + attacker.LPS) + 10) / 100.0;
      if (StaticRandom.RandDouble() > successProb)
        /***Pass is intercepted***/
        switch (StaticRandom.Rand(3))
        {
          case 0:
            if (counter)
              return new AttackSideState(defTeam, attTeam, 'l');
            else
              return new AttackSideState(defTeam, attTeam, 'l', "counter");
          case 1:
            if (counter)
              return new AttackSideState(defTeam, attTeam, 'r');
            else
              return new AttackSideState(defTeam, attTeam, 'r', "counter");
          case 2:
            return new ClearenceState(defTeam, attTeam);
          default:
            return null;
        }
      else if (StaticRandom.RandDouble() > 0.5)
        /***Pass***/
        if (counter)
          return new AttackSideState(attTeam, defTeam, 'l', "counter");
        else
          return new AttackSideState(attTeam, defTeam, 'l');
      else
        if (counter)
          return new AttackSideState(attTeam, defTeam, 'r', "counter");
        else
          return new AttackSideState(attTeam, defTeam, 'r');
    }
  }
}
