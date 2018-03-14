using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class ThrowInState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private State type;
    bool left;
    public State Type() {return type;}

    public ThrowInState(Lineup _attTeam, Lineup _defTeam, char _side)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      type = State.THROW_IN;
      if (_side == 'l')
      {
        left = true;
        if (attTeam.IsTherePosition(Position.LB))
          attacker = attTeam.Any(Position.LB);
        else
          attacker = attTeam.Any(Position.LM);
      }
      else
      {
        left = false;
        if (attTeam.IsTherePosition(Position.RB))
          attacker = attTeam.Any(Position.RB);
        else
          attacker = attTeam.Any(Position.RM);
      }
    }
    public IState Play()
    {

      double shortThrow = 1.0;
      double longThrow = 0.3 + (attacker.SP - 12) / 12.0;

      double p = StaticRandom.RandDouble(0, longThrow);
      if (p < shortThrow)
        if (left)
          return new AttackSideState(attTeam, defTeam, 'l');
        else
          return new AttackSideState(attTeam, defTeam, 'r');
      else
        return new BoxCrossState(attTeam, defTeam, attacker);
    }
  }
}
