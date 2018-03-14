using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class FreeKickState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private State type;
    public State Type() {return type;}

    public FreeKickState(Lineup _attTeam, Lineup _defTeam)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      type = State.FREE_KICK;
      attacker = attTeam.All().ByMax(x => x.SP);
    }
    public IState Play()
    {

      double shotProb = 1.0;
      double crossProb = 1.0;
      crossProb += (attTeam.All()
                           .Select(x => x.HEA)
                           .OrderByDescending(y => y)
                           .Take(6)
                           .Average() - 14) / 10.0;

      crossProb += shotProb;
      double p = StaticRandom.RandDouble(0, crossProb);

      if (p < shotProb)
        return new FreeKickShotState(attTeam, defTeam, attacker);
      else
        return new FreeKickCrossState(attTeam, defTeam, attacker);
    }
  }
}
