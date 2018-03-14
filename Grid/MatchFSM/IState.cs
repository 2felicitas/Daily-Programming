using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public interface IState
  
  {
    State Type();
    IState Play();
  }
}
