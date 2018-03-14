using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class NoMomentsState : IState
  {
    private Lineup team1;
    private Lineup team2;
    private State type;
    public State Type() {return type;}
    private bool haveChanged;
    public bool HaveChanged { get { return haveChanged; } }
    public NoMomentsState(Lineup _team1, Lineup _team2)
    {
      team1 = _team1;
      team2 = _team2;
      type = State.NO_MOMENTS;
      haveChanged = false;
      double att1Prob;                                                                   //First team
      double x = team1.Power / team2.Power;                                              //attack probability
      att1Prob = x >= 1 ? 1.0 / Math.PI * Math.Atan(0.6 * (x - 1.0)) + 0.5 :
                          1.0 - 1.0 / Math.PI * Math.Atan(0.6 * (1.0 / x - 1.0)) - 0.5;
      att1Prob = Math.Pow(att1Prob + 0.5, 12) - 0.5;
      if (StaticRandom.RandDouble() > att1Prob)
      {
        team1 = _team2;
        team2 = _team1;
        haveChanged = true;
      }
    }

    public IState Play()
    {
      double[] weights = {team1.All(Position.LB, Position.LM)
                               .Average(x => x.BestOVR),
                          team1.All(Position.CM, Position.AM, Position.CF)
                               .Average(x => x.BestOVR),
                          team1.All(Position.RB, Position.RM)
                               .Average(x => x.BestOVR)};
      char side = new char[] { 'l', 'c', 'r' }.GetRandomElementWithWeights(weights);
      if (side == 'c')
        return new AttackCentreState(team1, team2);
      else
        return new AttackSideState(team1, team2, side);
    }
  }
}
