using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class InboxShotAttemptState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private Player defender;
    private Player assist;
    private State type;
    public State Type() {return type;}

    public InboxShotAttemptState(Lineup _attTeam, Lineup _defTeam, Player _attacker, Player _defender, Player _assist = null)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      attacker = _attacker;
      defender = _defender;
      assist = _assist;
      type = State.INBOX_SHOT_ATTEMPT;
    }
    public IState Play()
    {
      double defProb;
      double y = attacker.FIN - defender.DEF + 7;
      defProb = (-0.0125 * y * y + 3 * y + 50) / 100;
      if (StaticRandom.RandDouble() > defProb)
      /***Shot is blocked***/
      {
        switch (StaticRandom.Rand(4))
        {
          case 0:
            return new CornerState(attTeam, defTeam);
          case 1:
            return new ClearenceState(defTeam, attTeam);
          case 2:
            return new OutboxReboundState(attTeam, defTeam);
          case 3:
            return new InboxReboundState(attTeam, defTeam);
          default:
            return null;
        }
      }
      else
        /***Not blocked - penalty/shot***/
        if (StaticRandom.RandDouble() < 0.05)
          return new PenaltyState(attTeam, defTeam);
        else
          return new ShotState(attTeam, defTeam, attacker, "inbox", assist);
    }
  }
}
