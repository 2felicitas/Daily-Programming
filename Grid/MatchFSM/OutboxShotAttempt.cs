using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class OutboxShotAttemptState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private Player defender;
    private State type;
    bool counter;
    public Player Attacker { get { return attacker; } }
    public State Type() {return type;}

    public OutboxShotAttemptState(Lineup _attTeam, Lineup _defTeam, Player _attacker, Player _defender, bool _counter = false)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      type = State.OUTBOX_SHOT_ATTEMPT;
      attacker = _attacker;
      defender = _defender;
      counter = _counter;
    }
    public IState Play()
    {

      double y = attacker.FIN - defender.DEF + 7;
      double shotProb = (-0.0125 * y * y + 3 * y + 50) / 100;
      if (counter)
        shotProb = 1.5 * shotProb / (shotProb + 0.5);
      if (StaticRandom.RandDouble() > shotProb)
      /***Shot is blocked***/
      {
        switch (StaticRandom.Rand(3))
        {
          case 0:
            return new CornerState(attTeam, defTeam);
          case 1:
            return new ClearenceState(defTeam, attTeam);
          case 2:
            return new OutboxReboundState(attTeam, defTeam);
          default:
            return null;
        }
      }
      else
        /***Not blocked - shot***/
        if (StaticRandom.RandDouble() < 0.16)
          return new FreeKickState(attTeam, defTeam);
        else
          return new ShotState(attTeam, defTeam, attacker, "outbox");
    }
  }
}
