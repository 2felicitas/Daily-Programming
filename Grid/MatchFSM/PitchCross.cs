using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class PitchCrossState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private State type;
    private bool left;
    public State Type() {return type;}

    public PitchCrossState(Lineup _attTeam, Lineup _defTeam, Player _attacker, bool _left)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      attacker = _attacker;
      left = _left;
      type = State.PITCH_CROSS;
    }

    public IState Play()
    {
      double successProb = (4 * attacker.LPS + 10) / 100.0;

      if (StaticRandom.RandDouble() < successProb)
        /***Not intercepted***/
        if (left)
          return new AttackSideState(attTeam, defTeam, 'r');
        else
          return new AttackSideState(attTeam, defTeam, 'l');
      else
        /***Switch is intercepted***/
        switch (StaticRandom.Rand(3))
        {
          case 0:
            return new NoMomentsState(attTeam, defTeam);
          case 1:
            if (left)
              return new AttackSideState(defTeam, attTeam, 'r', "counter");
            else
              return new AttackSideState(defTeam, attTeam, 'l', "counter");
          case 2:
            return new ClearenceState(defTeam, attTeam);
          default:
            return null;
        }
    }
  }
}
