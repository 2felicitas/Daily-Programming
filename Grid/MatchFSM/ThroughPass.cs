using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class ThroughPassState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private State type;
    bool counter;
    public State Type() {return type;}

    public ThroughPassState(Lineup _attTeam, Lineup _defTeam, Player _attacker, bool _counter)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      type = State.THROUGH_PASS;
      attacker = _attacker;
      counter = _counter;
    }
    public IState Play()
    {
      var defenders = defTeam.All(Position.CB, Position.DM);
      var attackers = attTeam.All(Position.CF, Position.AM, Position.LM, Position.RM)
                             .Where(x => x != attacker);
      double successProb = (2.25 * attacker.SPS - 10 * (1 + defenders.Count() - attackers.Count())) / 100.0; 
      if (counter)
        successProb += 2.25 * (20 - defenders.Average(x => x.DEF) / 2) / 100.0;
      else
        successProb += 2.25 * (20 - defenders.Average(x => x.DEF)) / 100.0;
      if (StaticRandom.RandDouble() > successProb)
        /***Pass is intercepted or too long***/
        switch (StaticRandom.Rand(4))
        {
          case 0:
            return new OutboxReboundState(attTeam, defTeam);
          case 1:
            return new AttackCentreState(defTeam, attTeam);
          case 2:
            return new ClearenceState(defTeam, attTeam);
          case 3:
            return new GoalKickState(defTeam, attTeam);
          default:
            return null;
        }
      else
      /***Not intercepted***/
      {
        Player newAttacker = attackers
                             .OrderByDescending(x => x.PAC + x.FIN)
                             .GetRandomElementWithWeights();
        return new InboxShotAttemptState(attTeam, defTeam, newAttacker, defTeam.Any(Position.CB), attacker);
      }
    }
  }
}
