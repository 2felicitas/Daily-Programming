using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class FreeKickCrossState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private Player defender;
    private Player server;
    private State type;
    public State Type() {return type;}

    public FreeKickCrossState(Lineup _attTeam, Lineup _defTeam, Player _server)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      type = State.FREE_KICK_CROSS;
      attacker = attTeam.AllFieldPlayers()
                        .Where(x => x != _server)
                        .OrderByDescending(x => x.HEA)
                        .Take(6)
                        .GetRandomElement();
      defender = defTeam.AllFieldPlayers()
                        .OrderByDescending(x => x.HEA)
                        .Take(6)
                        .GetRandomElement();
      server = _server;
    }
    public IState Play()
    {
      if (StaticRandom.RandDouble() > (server.SP + server.LPS) / 40.0)
        return new GoalKickState(defTeam, attTeam);

      double shotProb = 1.0;
      double clearProb = 2.0;
      double ownShotProb = 0.05;

      shotProb += (attacker.HEA + attacker.STR - 20) / 20.0;
      clearProb += (defender.HEA + defender.STR - 20) / 20.0;
      clearProb += shotProb;
      ownShotProb += clearProb;

      double p = StaticRandom.RandDouble(0, ownShotProb);
      if (p < shotProb)
        return new ShotState(attTeam, defTeam, attacker, "header", server);
      else if (p < clearProb)
        return new ClearenceState(defTeam, attTeam);
      else
        return new OwnShotState(attTeam, defTeam, defender);
    }
  }
}
