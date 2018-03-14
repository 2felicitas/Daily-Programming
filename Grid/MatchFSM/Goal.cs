using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class GoalState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private State type;
    private Player scorer;
    private Player assist;
    public Player Scorer { get { return scorer; } }
    public Player Assist { get { return assist; } }
    public State Type() { return type; }

    public GoalState(Lineup _attTeam, Lineup _defTeam, Player _scorer, Player _assist = null)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      scorer = _scorer;
      assist = _assist;
      type = State.GOAL;
    }
    public IState Play()
    {
      return new StartState(attTeam, defTeam);
    }
  }
}
