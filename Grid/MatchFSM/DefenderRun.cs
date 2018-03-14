using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class DefenderRunState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    bool left;
    bool counter;
    private Player newAttacker;
    private Player defender;
    private Player newDefender;
    private State type;
    public State Type() { return type; }

    public DefenderRunState(Lineup _attTeam, Lineup _defTeam, Player _defender, bool _left, bool _counter)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      defender = _defender;
      left = _left;
      counter = _counter;
      type = State.DEFENDER_RUN;
      if (left)
        newAttacker = attTeam.Any(Position.LB);
      else
        newAttacker = attTeam.Any(Position.RB);
      if (defTeam.PlayerPosition(defender) == Position.LB ||
          defTeam.PlayerPosition(defender) == Position.RB)
        if (left)
          if (defTeam.IsTherePosition(Position.LM))
            newDefender = defTeam.Any(Position.LM);
          else
            newDefender = defTeam.Any(Position.CB);
        else
          if (defTeam.IsTherePosition(Position.RM))
            newDefender = defTeam.Any(Position.RM);
          else
            newDefender = defTeam.Any(Position.CB);
      else
        newDefender = defTeam.Any(Position.CB);
    }

    public IState Play()
    {
      double defProb = (2.25 * defender.DEF + 5) / 100;
      if (StaticRandom.RandDouble() < defProb)
        /***Pass is intercepted***/
        switch (StaticRandom.Rand(3))
        {
          case 0:
            if (left)
              return new AttackSideState(defTeam, attTeam, 'r', "counter");
            else
              return new AttackSideState(defTeam, attTeam, 'l', "counter");
          case 1:
            if (left)
              return new ThrowInState(attTeam, defTeam, 'l');
            else
              return new ThrowInState(attTeam, defTeam, 'r');
          case 2:
            return new ClearenceState(defTeam, attTeam);
          default:
            return null;
        }
      else
      /***Not intercepted***/
      {
        double y = attTeam.All(Position.CF, Position.AM).Max(x => x.HEA);
        double boxCrossProb = 1 + 0.00061 * y * y * y - 0.0024 * y * y + 0.004 * y - 1;
        double lowCrossProb = 1 + 0.00085 * y * y * y - 0.0243 * y * y - 0.054 * y + 3;
        lowCrossProb += boxCrossProb;

        /***Cross***/
        if (StaticRandom.RandDouble(0, lowCrossProb) < boxCrossProb)
          return new BoxCrossAttemptState(attTeam, defTeam, newAttacker, newDefender, left, counter);
        /***Low cross***/
        else
          return new LowCrossAttemptState(attTeam, defTeam, newAttacker, newDefender, left, counter);
      }
    }
  }
}
