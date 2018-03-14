using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;


namespace Schedule.FSM
{
  public class AttackCentreState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    bool counter;
    private Player attacker; 
    private Player defender;
    private State type;
    public State Type() {return type;}
    public AttackCentreState(Lineup _attTeam, Lineup _defTeam, string _counter = "")
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      if (_counter == "counter")
      {
        counter = true;
        type = State.COUNTER_ATTACK_CENTRE;
        attacker = attTeam.Any(Position.CM, Position.AM, Position.CF);
      }
      else
      {
        counter = false;
        type = State.ATTACK_CENTRE;
        if (attTeam.All(Position.CF).Count == 3)
          attacker = attTeam.Any(Position.DM, Position.CM, Position.AM, Position.CF);
        else
          attacker = attTeam.Any(Position.DM, Position.CM, Position.AM);
      }
      defender = defTeam.Any(Position.DM, Position.CB);
    }
    public IState Play()
    {
      double throughBallProb = 1.0;
      double shotProb = 1.0;
      double flankPassProb = 1.0;
      double dribblingProb = 1.0;

      double y = (attacker.SPS + attTeam.All(Position.LM, Position.RM, Position.AM, Position.CF)
                                        .Where(x => x != attacker)
                                        .Max(x => x.PAC)) / 2.0;
      throughBallProb += 0.0085 * y * y + 0.03 * y - 1;                                  //0~20 -> -1~3
      y = attacker.FIN;
      shotProb += 0.0085 * y * y + 0.03 * y - 1;
      y = (attacker.SPS + attacker.LPS) / 2.0;
      flankPassProb += 0.0085 * y * y + 0.03 * y - 1;
      if (!defTeam.IsTherePosition(Position.LB))
        flankPassProb += 1.0;
      y = attacker.DRB;
      dribblingProb += 0.000001 * Math.Pow(y, 4) + 0.0071 * y * y - 1;                   //0~20 -> -1~2

      shotProb += throughBallProb;
      flankPassProb += shotProb;
      dribblingProb += flankPassProb;

      double p = StaticRandom.RandDouble(0, dribblingProb);
      if (p < throughBallProb)
        return new ThroughPassState(attTeam, defTeam, attacker, counter);
      else if (p < shotProb)
        return new OutboxShotAttemptState(attTeam, defTeam, attacker, defender, counter);
      else if (p < flankPassProb)
        return new FlankPassState(attTeam, defTeam, attacker, counter);
      else
        return new BoxDribblingState(attTeam, defTeam, attacker);
    }
  }
}
