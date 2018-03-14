using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mazes
{
  partial class Maze
  {
    private void MoveTrolls()
    {
     for (int i = 0; i < trolls.Count; i++)
       if (trolls[i].Chase || CheckForPlayer(trolls[i]))
         TrollChaseMode(trolls[i]);
       else
         TrollCalmMode(trolls[i]);
     PrintMap = map.Print();
    }

    private void TrollChaseMode(Troll t)
    {
      map[t.Position] = MazeMap.Empty;
      t.Position = t.DesirePosition;
      if (Player.cursor.Contains(map[t.Position]))
      {
        map[t.Position] = Troll.angryCursor;
        Eat();
      }
      else
        if (t.Position == t.ChaseGoal && !CheckForPlayer(t))
        {
          map[t.Position] = Troll.calmCursor;
          t.Chase = false;
        }
        else
          map[t.Position] = Troll.angryCursor;
    }

    private void TrollCalmMode(Troll t)
    {
      map[t.Position] = MazeMap.Empty;
      if (StaticRandom.Next(6) > 1 && map.isFree(t.DesirePosition))
        t.Position += t.Dir;
      else
      {
        double j = t.Dir.X == 0 ? Math.Acos(t.Dir.Y) : -Math.Asin(t.Dir.X);
        for (double k = j + Math.PI/2; k > double.MinValue; k += Math.PI / 2)
        {
          t.Dir = Extensions.Direction(k);
          if (StaticRandom.Next(3) > 1 && (map.isFree(t.DesirePosition) ||
              Player.arrows.Contains(map[t.DesirePosition])))
            break;
        }
        t.Position = t.DesirePosition;
      }
      map[t.Position] = Troll.calmCursor;
      CheckForPlayer(t);
    }

    private bool CheckForPlayer(Troll t)
    {
      for (double j = 0; !t.Chase && j != 2 * Math.PI; j += Math.PI / 2)
      {
        Point n = t.Position + Extensions.Direction(j);
        while (!map.isWall(n))
        {
          if (t.Check(map[n]))
          {
            t.Dir = Extensions.Direction(j);
            t.ChaseGoal = n;
            return true;
          }
          n += Extensions.Direction(j);
        }
      }
      return false;
    }
  }
}
