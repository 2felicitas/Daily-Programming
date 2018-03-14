using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class ShotState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private Player goalkeeper;
    private Player assist;
    private State type;
    public State Type() {return type;}

    public ShotState(Lineup _attTeam, Lineup _defTeam, Player _attacker, string shotType, Player _assist = null)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      if (shotType == "inbox")
        type = State.INBOX_SHOT;
      else if (shotType == "outbox")
        type = State.OUTBOX_SHOT;
      else if (shotType == "header")
        type = State.HEADER;
      attacker = _attacker;
      assist = _assist;
      goalkeeper = defTeam.Any(Position.GK);
    }
    public IState Play()
    {
      double y;
      double shotProb;
      double goalProb;
      if (type == State.INBOX_SHOT)
      {
        y = attacker.FIN;
        shotProb = (-1.0 / 12.0 * y * y + 39.0 / 12.0 * y + 30.0) / 100.0;
        goalProb = y / (goalkeeper.GK * 13.0 / 6.0 + 5.0);
      }
      else if (type == State.OUTBOX_SHOT)
      {
        y = attacker.FIN;
        shotProb = (1.0 / 12.0 * y * y + 13.0 / 12.0 * y + 15.0) / 100.0;
        goalProb = y / (goalkeeper.GK * 33.0 / 6.0 + 5.0);
      }
      else if (type == State.HEADER)
      {
        y = (attacker.FIN + attacker.HEA) / 2.0;
        shotProb = (1.0 / 12.0 * y * y + 13.0 / 12.0 * y + 15.0) / 100.0;
        goalProb = y / (goalkeeper.GK * 27.0 / 6.0 + 5.0);
      }
      else
        return null;
      if (StaticRandom.RandDouble() > shotProb)
        /***Miss***/
        return new GoalKickState(defTeam, attTeam);
      else
      {
        if (StaticRandom.RandDouble() < goalProb)
        /***Goal***/
          return new GoalState(attTeam, defTeam, attacker, assist);
        else
          /***GK saves***/
          switch (StaticRandom.Rand(4))
          {
            case 0:
            case 1:
              return new CornerState(attTeam, defTeam);
            case 2:
              return new GKStartState(defTeam, attTeam);
            case 3:
              return new InboxReboundState(attTeam, defTeam);
            default:
              return null;
          }
      }
    }
  }
}
