using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class AttackSideState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    bool counter;
    bool left;
    private Player attacker; 
    private Player defender;
    private State type;
    public State Type() { return type; }

    public AttackSideState(Lineup _attTeam, Lineup _defTeam, char _side, string _counter = "")
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      if (_counter == "counter")
      {
        type = State.COUNTER_ATTACK_SIDE;
        counter = true;
      }
      else
      {
        type = State.ATTACK_SIDE;
        counter = false;
      }
      if (_side == 'l')
      {
        left = true;
        if (attTeam.IsTherePosition(Position.LM))
          attacker = attTeam.Any(Position.LM);
        else
          attacker = attTeam.Any(Position.LB);
        if (defTeam.IsTherePosition(Position.RB))
          defender = defTeam.Any(Position.RB);
        else
          defender = defTeam.Any(Position.RM);
      }
      else
      {
        left = false;
        if (attTeam.IsTherePosition(Position.RM))
          attacker = attTeam.Any(Position.RM);
        else
          attacker = attTeam.Any(Position.RB);
        if (defTeam.IsTherePosition(Position.LB))
          defender = defTeam.Any(Position.LB);
        else
          defender = defTeam.Any(Position.LM);
      }
    }

    public IState Play()
    {
      double boxCrossProb = 1.5;
      double lowCrossProb = 1.5;
      double pitchCrossProb = 0.5;
      double centrePassProb = 1.0;
      double shotProb = 0.5;
      double dribblingProb = 1.0;
      double defenderRunProb = 1.0;

      if (left & (!attTeam.IsTherePosition(Position.LB) ||
                  !attTeam.IsTherePosition(Position.LM)) ||
         !left & (!attTeam.IsTherePosition(Position.RB) ||
                  !attTeam.IsTherePosition(Position.RM)))
        defenderRunProb = 0;
      double y = (attTeam.All(Position.CF, Position.AM)
                         .Max(x => x.HEA) + attacker.LPS) / 2.0;
      boxCrossProb += 0.00061 * y * y * y - 0.0024 * y * y + 0.004 * y - 1;              //0~20 -> -1~3
      lowCrossProb += 0.00085 * y * y * y - 0.0243 * y * y - 0.054 * y + 3;              //0~20 -> 3~-1
      y = attacker.FIN;
      shotProb     += 0.0085 * y * y + 0.03 * y - 1;                                     //0~20 -> -1~3
      y = (attacker.SPS + attTeam.All(Position.DM, Position.AM, Position.CM)
                                 .Max(x => x.BestOVR)) / 2.0;
      centrePassProb += 0.0085 * y * y + 0.03 * y - 1;
      y = (attacker.LPS + (left ? attTeam.All(Position.RM, Position.RB)
                                         .Max(x => x.BestOVR) 
                                : attTeam.All(Position.LM, Position.LB)
                                         .Max(x => x.BestOVR))) / 2.0;
      pitchCrossProb += 0.0085 * y * y + 0.03 * y - 1;
      y = attacker.DRB;
      dribblingProb += 0.000001 * Math.Pow(y, 4) + 0.0071 * y * y - 1;                   //0~20 -> -1~2

      lowCrossProb += boxCrossProb;
      pitchCrossProb += lowCrossProb;
      centrePassProb += pitchCrossProb;
      shotProb += centrePassProb;
      dribblingProb += shotProb;
      defenderRunProb += dribblingProb;

      double p = StaticRandom.RandDouble(0, defenderRunProb);
      if (p < boxCrossProb)
        return new BoxCrossAttemptState(attTeam, defTeam, attacker, defender, left, counter);
      else if (p < lowCrossProb)
        return new LowCrossAttemptState(attTeam, defTeam, attacker, defender, left, counter);
      else if (p < pitchCrossProb)
        return new PitchCrossState(attTeam, defTeam, attacker, left);
      else if (p < centrePassProb)
        return new CentrePassState(attTeam, defTeam, attacker, counter);
      else if (p < shotProb)
        return new OutboxShotAttemptState(attTeam, defTeam, attacker, defender, counter);
      else if (p < dribblingProb)
        return new BoxDribblingState(attTeam, defTeam, attacker);
      else
        return new DefenderRunState(attTeam, defTeam, defender, left, counter);
    }
  }
}
