using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Schedule.Classes;

namespace Schedule.FSM
{
  public enum State
  {
    ATTACK_CENTRE, ATTACK_SIDE,
    COUNTER_ATTACK_CENTRE, COUNTER_ATTACK_SIDE,
    BOX_CROSS_ATTEMPT, BOX_CROSS,
    LOW_CROSS_ATTEMPT, LOW_CROSS,
    PITCH_CROSS, THROW_IN,
    CENTRE_PASS, CENTRE_PASS_COUNTER,
    DEFENDER_RUN,
    BOX_DRIBBLING, THROUGH_PASS,
    FLANK_PASS, FLANK_PASS_COUNTER,
    FREE_KICK, FREE_KICK_CROSS, FREE_KICK_SHOT,
    PENALTY, CORNER,
    GOAL_KICK, GK_START,
    CENTRE_PITCH_HEADER,
    INBOX_SHOT_ATTEMPT, INBOX_SHOT,
    OUTBOX_SHOT_ATTEMPT, OUTBOX_SHOT,
    HEADER, OWN_SHOT, GOAL,
    INBOX_REBOUND, OUTBOX_REBOUND,
    CLEARENCE,
    NO_MOMENTS,
    START,
    NULL
  }

  partial class MatchFSM
  {
    private Lineup homeTeam;
    private Lineup awayTeam;
    private MatchStats ms;
    private IState state;
    private double minute;
    private bool isHomeTeamAttacking;
    private bool sechalf;

    public MatchFSM(Lineup home, Lineup away)
    {
      homeTeam = home;
      awayTeam = away;
      minute = 0;
      state = new StartState(homeTeam, awayTeam);
      isHomeTeamAttacking = true;
      ms = new MatchStats(homeTeam, awayTeam);
    }

    public MatchStats Start()
    {
      eventHandler();
      return ms;
    }

    private void setState(IState p)
    {
      switch (p.Type())
      {
        case State.COUNTER_ATTACK_SIDE:
          if (state.Type() != State.FLANK_PASS_COUNTER && 
              state.Type() != State.GK_START)
            SwapTeams();
          break;
        case State.COUNTER_ATTACK_CENTRE:
          if (state.Type() != State.CENTRE_PASS_COUNTER &&
              state.Type() != State.GK_START)
            SwapTeams();
          break;
        case State.ATTACK_SIDE:
          if (state.Type() == State.FLANK_PASS_COUNTER)
            SwapTeams();
          break;
        case State.ATTACK_CENTRE:
          if (state.Type() == State.BOX_DRIBBLING || 
              state.Type() == State.THROUGH_PASS ||
              state.Type() == State.CENTRE_PASS_COUNTER)
            SwapTeams();
          break;
        case State.GOAL_KICK:
        case State.CLEARENCE:
        case State.GK_START:
          SwapTeams();
          break;
        case State.NO_MOMENTS:
          if ((p as NoMomentsState).HaveChanged)
            SwapTeams();
          break;
        case State.GOAL:
          ms.AddGoal((minute > 45 && !sechalf) ? "45+" : Math.Ceiling(minute).ToString(),
                     (p as GoalState).Scorer, 
                     isHomeTeamAttacking,
                     state is PenaltyState ? "p" : state is OwnShotState ? "og" : "",
                     (p as GoalState).Assist);
          break;
        default:
          break;
      }
      state = p;
    }
    private void eventHandler()
    {
      bool end = false;
      sechalf = false;
      int firstHalfEnd = StaticRandom.Rand(45, 49);
      int matchEnd = StaticRandom.Rand(90, 97);
      while (!end)
      {
        setState(state.Play());
        switch (state.Type())
        {
          case State.ATTACK_CENTRE:
          case State.ATTACK_SIDE:
          case State.FREE_KICK:
          case State.CORNER:
          case State.PENALTY:
          case State.GOAL:
            minute += 1;
            break;
          case State.COUNTER_ATTACK_CENTRE:
          case State.COUNTER_ATTACK_SIDE:
            minute += 0.5;
            break;
          case State.NO_MOMENTS:
            minute += StaticRandom.RandDouble(1, 4);
            break;
          default:
            break;
        }
        if (minute >= firstHalfEnd && !sechalf)
        {
          sechalf = true;
          minute = 45;
          setState(new StartState(awayTeam, homeTeam));
          isHomeTeamAttacking = false;
        }
        if (minute >= matchEnd)
          end = true;
      }
    }

    private void SwapTeams()
    {
      isHomeTeamAttacking = !isHomeTeamAttacking;
    }
  }
}
