using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class LowCrossAttemptState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private Player defender;
    private State type;
    private bool counter;
    private bool left;
    public Player Attacker { get { return attacker; } }
    public State Type() {return type;}

    public LowCrossAttemptState(Lineup _attTeam, Lineup _defTeam, Player _attacker, Player _defender, bool _left, bool _counter)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      attacker = _attacker;
      defender = _defender;
      counter = _counter;
      left = _left;
      type = State.LOW_CROSS_ATTEMPT;
    }

    public IState Play()
    {
      double successProb = (2.25 * attacker.LPS + 5 + 2.25 * 10) / 100.0;
      if (!counter)
        successProb += 2.25 * (10 - defender.DEF) / 100;
      if (StaticRandom.RandDouble() > successProb)
        /***Cross is blocked***/
        switch (StaticRandom.Rand(3))
        {
          case 0:
            if (left)
              return new ThrowInState(attTeam, defTeam, 'l');
            else
              return new ThrowInState(attTeam, defTeam, 'r');
          case 1:
            return new ClearenceState(defTeam, attTeam);
          case 2:
            return new CornerState(attTeam, defTeam);
          default:
            return null;
        }
      else
      /***Not blocked - cross***/
        return new LowCrossState(attTeam, defTeam, attacker);
    }
  }
}
