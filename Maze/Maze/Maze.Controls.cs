using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Mazes
{
  partial class Maze
  {
    public void ManualControl()
    {
      while (true)
      {
        if (ArrowDown)
          if (KeyDown == Key.A)
          {
            AutoControl();
            break;
          }
          else
            ManualMove(KeyDown);
      }
    }

    private void ManualMove(Key key)
    {
      bool isexit = false;
      if (key == Key.Up)
        pl.Dir = new Point(-1, 0);
      else if (key == Key.Down)
        pl.Dir = new Point(1, 0);
      else if (key == Key.Left)
        pl.Dir = new Point(0, -1);
      else if (key == Key.Right)
        pl.Dir = new Point(0, 1);
      else
        return;

      if (map.isFree(pl.DesirePosition) || map.isExit(pl.DesirePosition) ||
          Player.arrows.Contains(map[pl.DesirePosition]))
      {
        map[pl.Position] = Player.arrows[Math.Abs(pl.Dir.X) * (pl.Dir.X + 1) +
                                         Math.Abs(pl.Dir.Y) * (pl.Dir.Y + 2)];
        pl.Position = pl.DesirePosition;
        map[pl.Position] = Player.cursor[Math.Abs(pl.Dir.X) * (pl.Dir.X + 1) +
                                         Math.Abs(pl.Dir.Y) * (pl.Dir.Y + 2)];
        if (map.isExit(pl.Position))
          isexit = true;
      }
      ArrowDown = false;
      MoveTrolls();
      PrintMap = map.Print();
      if (isexit)
        MessageBox.Show("Congratulations");
    }

    private void AutoControl()
    {
      /*List<Point> path = PathFinding.AStar(pl.Position, map.Exit, map.neighbours);
      for (int i = 0; i < path.Count; i++)
      {
        if (i < path.Count - 1)
          pl.Dir = path[i + 1] - path[i];

        if (map.isFree(pl.DesirePosition) || map.isExit(pl.DesirePosition))
        {
          map[pl.Position] = Player.arrows[Math.Abs(pl.Dir.X) * (pl.Dir.X + 1) +
                                           Math.Abs(pl.Dir.Y) * (pl.Dir.Y + 2)];
          pl.Position = pl.DesirePosition;
          map[pl.Position] = Player.cursor[Math.Abs(pl.Dir.X) * (pl.Dir.X + 1) +
                                           Math.Abs(pl.Dir.Y) * (pl.Dir.Y + 2)];
        }
        MoveTrolls();
        PrintMap = map.Print(0, 0);
        Thread.Sleep(300);
      }*/
      List<Point> path = new List<Point>();
      path.Add(pl.Position);
      while (pl.Position != map.Exit)
      {
        path.Add(PathFinding.AStar(pl.Position, map.Exit, map.neighbours).First(x => x != pl.Position));
        pl.Dir = path[path.Count - 1] - path[path.Count - 2];
        if (map.isFree(pl.DesirePosition) || map.isExit(pl.DesirePosition))
        {
          map[pl.Position] = Player.arrows[Math.Abs(pl.Dir.X) * (pl.Dir.X + 1) +
                                           Math.Abs(pl.Dir.Y) * (pl.Dir.Y + 2)];
          pl.Position = pl.DesirePosition;
          map[pl.Position] = Player.cursor[Math.Abs(pl.Dir.X) * (pl.Dir.X + 1) +
                                           Math.Abs(pl.Dir.Y) * (pl.Dir.Y + 2)];
        }
        MoveTrolls();
        PrintMap = map.Print();
        Thread.Sleep(300);
      }
    }
  }
}
