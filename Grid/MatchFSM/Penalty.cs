using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class PenaltyState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private Player attacker; 
    private Player goalkeeper;
    private State type;
    public State Type() {return type;}

    public PenaltyState(Lineup _attTeam, Lineup _defTeam)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      type = State.PENALTY;
      attacker = attTeam.All().ByMax(x => x.SP + x.FIN);
      goalkeeper = defTeam.Any(Position.GK);
    }
    public IState Play()
    {
      double shooterAbility = (attacker.FIN + attacker.SP) / 2.0;
      double goalProb = shooterAbility + 2 - goalkeeper.GK;

      double[] shotWeights = { 2 / shooterAbility, 1.0, 0.5, 1.0 };                      //miss, left, centre, right
      double[] gkWeights    = { 1.0, 0.4, 1.0 };                                         //left, centre, right
      char shotDir = new char[] { 'm', 'l', 'c', 'r' }.GetRandomElementWithWeights(shotWeights);
      char gkDir;
      if (StaticRandom.RandDouble() < 0.33)                                              //GK reacts
      {
        gkDir = shotDir;
        goalProb += 1;
      }
      else
        gkDir = new char[] { 'l', 'c', 'r' }.GetRandomElementWithWeights(gkWeights);

      if (shotDir == 'm')
        return new GoalKickState(defTeam, attTeam);
      if (shotDir == gkDir)
        if (StaticRandom.RandGaussian(0, 4) < goalProb)
          return new GoalState(attTeam, defTeam, attacker);
        else
          switch (StaticRandom.Rand(3))
          {
            case 0:
              return new GoalKickState(defTeam, attTeam);
            case 1:
              return new CornerState(attTeam, defTeam);
            case 2:
              return new InboxReboundState(attTeam, defTeam);
            default:
              return null;
          }
      else
        return new GoalState(attTeam, defTeam, attacker);
    }
  }
}
