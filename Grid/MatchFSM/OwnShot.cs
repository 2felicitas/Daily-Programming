using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class OwnShotState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player defender;
    private Player goalkeeper;
    private State type;
    public Player Defender { get { return defender; } }
    public State Type() {return type;}

    public OwnShotState(Lineup _attTeam, Lineup _defTeam, Player _defender)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      defender = _defender;
      type = State.OWN_SHOT;
      goalkeeper = defTeam.Any(Position.GK);
    }
    public IState Play()
    {
      double goalProb = 10.0 / (goalkeeper.GK * 13.0 / 6.0 + 5.0);
      if (StaticRandom.RandDouble() < goalProb)
        /***Goal***/
        return new GoalState(attTeam, defTeam, defender);
      else
        /***GK saves***/
        switch (StaticRandom.Rand(3))
        {
          case 0:
            return new CornerState(attTeam, defTeam);
          case 1:
            return new GKStartState(defTeam, attTeam);
          case 2:
            return new InboxReboundState(attTeam, defTeam);
          default:
            return null;
        }
    }
  }
}
