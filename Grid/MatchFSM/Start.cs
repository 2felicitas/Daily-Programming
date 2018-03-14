using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public class StartState : IState
  {
    private Lineup attTeam;
    private Lineup defTeam;
    private State type;
    public State Type() {return type;}

    public StartState(Lineup _attTeam, Lineup _defTeam)
    {
      attTeam = _attTeam;
      defTeam = _defTeam;
      type = State.START;
    }
    public IState Play()
    {
      return new NoMomentsState(attTeam, defTeam);
    }
  }
}
