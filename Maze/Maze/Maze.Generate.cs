using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mazes
{
  partial class Maze
  {
    public void genPlayer()
    {
      int x, y;
      do
      {
        x = StaticRandom.Next(1, map.Height - 1);
        y = StaticRandom.Next(1, map.Width - 1);
      } while (!map.isFree(x, y));
      pl = new Player(x, y);
      map[x, y] = Player.cursor[0];
    }

    public void genTrolls(int n)
    {
      trolls = new List<Troll>();
      for (int i = 0; i < n; i++)
      {
        int x, y;
        do
        {
          x = StaticRandom.Next(1, map.Height - 1);
          y = StaticRandom.Next(1, map.Width - 1);
        } while (!map.isFree(x, y));
        trolls.Add(new Troll(x, y));
        map[x, y] = Troll.calmCursor;
      }
    }
  }
}
