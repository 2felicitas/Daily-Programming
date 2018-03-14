using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class BoxDribblingState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private State type;
    public State Type() { return type; }

    public BoxDribblingState(Lineup _attTeam, Lineup _defTeam, Player _attacker)
    {
      type = State.BOX_DRIBBLING;
      attacker = _attacker;
      attTeam = _attTeam;
      defTeam = _defTeam;
    }

    public IState Play()
    {
      double successProb = (2.25 * attacker.DRB +
                            2.25 * (15 - defTeam.All(Position.CB)
                                                .Sum(x => x.DEF) / 3) / 100.0);
      if (StaticRandom.RandDouble() > successProb)
        /***Defender tackles***/
        switch (StaticRandom.Rand(2))
        {
          case 0:
            return new ClearenceState(defTeam, attTeam);
          case 1:
            return new AttackCentreState(defTeam, attTeam);
          default:
            return null;
        }
      else
        /***Free kick/pass/shot***/
        if (StaticRandom.RandDouble() < 0.1)
          return new FreeKickState(attTeam, defTeam);
        else
        {
          double y = attacker.FIN;
          double shotProb = 0.0085 * y * y + 0.03 * y - 1;
          y = attacker.SPS;
          double passProb = shotProb + 0.0085 * y * y + 0.03 * y - 1;
          if (StaticRandom.RandDouble(0, passProb) < shotProb)
          {
            Player newAttacker = attTeam.All(Position.CF, Position.AM, Position.RM, Position.LM)
                                        .Where(x => x != attacker)
                                        .GetRandomElement();
            return new InboxShotAttemptState(attTeam, defTeam, newAttacker, defTeam.Any(Position.CB), attacker);
          }
          else
            return new InboxShotAttemptState(attTeam, defTeam, attacker, defTeam.Any(Position.CB));
        }
    }
  }
}
