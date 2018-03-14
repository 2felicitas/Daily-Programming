using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class LowCrossState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private Player defender;
    private Player assist;
    private State type;
    public State Type() {return type;}

    public LowCrossState(Lineup _attTeam, Lineup _defTeam, Player _assist)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      assist = _assist;
      type = State.LOW_CROSS;
      attacker = attTeam.Any(Position.CF, Position.AM);
      defender = defTeam.Any(Position.CB, Position.RB, Position.LB, Position.DM);
    }
    public IState Play()
    {
      double shotProb = 1.0;
      double clearProb = 1.0;
      double ownShotProb = 0.05;

      shotProb += (attacker.PAC + attacker.FIN - 20) / 20.0;
      clearProb += (defender.DEF + defender.PAC - 20) / 20.0;

      clearProb += shotProb;
      ownShotProb += clearProb;

      double p = StaticRandom.RandDouble(0, ownShotProb);
      if (p < shotProb)
        return new InboxShotAttemptState(attTeam, defTeam, attacker, defTeam.Any(Position.CB), assist);
      else if (p < clearProb)
        return new ClearenceState(defTeam, attTeam);
      else
        return new OwnShotState(attTeam, defTeam, defender);
    }
  }
}
