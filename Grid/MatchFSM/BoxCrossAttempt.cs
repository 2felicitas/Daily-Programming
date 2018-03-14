using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;


namespace Schedule.FSM
{
  public class BoxCrossAttemptState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private Player defender;
    private State type;
    private bool counter;
    private bool left;
    public State Type() { return type; }

    public BoxCrossAttemptState(Lineup _attTeam, Lineup _defTeam, Player _attacker, Player _defender, bool _left, bool _counter)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      attacker = _attacker;
      defender = _defender;
      counter = _counter;
      left = _left;
      type = State.BOX_CROSS_ATTEMPT;
    }

    public IState Play()
    {
      double successProb = (2.25 * attacker.LPS + 15 + 1.125 * 20) / 100.0;
      if (!counter)
        successProb += 1.125 * (20 - defender.DEF - defender.HEA) / 100.0;
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
      /***not blocked - cross***/
        return new BoxCrossState(attTeam, defTeam, attacker);
    }
  }
}
