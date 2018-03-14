using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mazes
{
  public class Character
  {
    private Point position;
    public Point Position
    {
      get {return position;}
      set {position = value;}
    }   
    private Point dir;
    public Point Dir
    {
      get { return dir; }
      set { dir = value; }
    }
    public Point DesirePosition
    { get {return position + dir;} }
    public bool isDead { get; set; }

    public int x
    { get { return position.X; } }
    public int y
    { get { return position.Y; } }
  }

  public class Player : Character
  {
    public static char[] cursor = { '▲', '◄', '▼', '►' };
    public static char[] arrows = { '↑', '←', '↓', '→' };

    public Player(int x, int y)
    {
      Position = new Point(x, y);
      Dir = new Point(-1, 0);
      isDead = false;
    }
  }

  public class Troll : Character
  {
    public const char calmCursor = 'T';
    public const char angryCursor = 'W';
    public bool Chase { get; set; }
    public Point ChaseGoal { get; set; }

    public Troll(int x, int y)
    {
      Position = new Point(x, y);
      Dir = new Point(-1, 0);
      Chase = false;
      isDead = false;
    }

    public bool Check(char c)
    {
      if (Player.cursor.Contains(c))
      {
        Chase = true;
        return true;
      }
      else
        return false;
    }
  }
}
