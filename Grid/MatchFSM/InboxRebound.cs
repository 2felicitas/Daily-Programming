using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class InboxReboundState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private Player defender;
    private State type;
    public State Type() {return type;}

    public InboxReboundState(Lineup _attTeam, Lineup _defTeam)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      type = State.INBOX_REBOUND;
      attacker = attTeam.All(Position.CF, Position.AM, Position.RM, Position.LM)
                        .OrderByDescending(x => x.PAC + x.FIN)
                        .GetRandomElementWithWeights();
      defender = defTeam.Any(Position.CB, Position.RB, Position.LB, Position.DM);
    }
    public IState Play()
    {
      double reboundProb = attacker.PAC / (defender.PAC + 20.0);
      if (StaticRandom.RandDouble() < reboundProb)
        /***Rebound by offence***/
        return new ShotState(attTeam, defTeam, attacker, "inbox");
      else
        /***rebound by defence***/
        if (StaticRandom.RandDouble() < 0.5)
          return new CornerState(attTeam, defTeam);
        else
          return new ClearenceState(defTeam, attTeam);
    }
  }
}
