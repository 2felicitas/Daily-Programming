using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mazes
{
  public class Point
  {
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int _x = 0, int _y = 0)
    {
      this.X = _x;
      this.Y = _y;
    }

    public Point Normalize()
    {
      return this / Math.Abs(this.X + this.Y);
    }
    public static Point operator -(Point point1, Point point2)
    {
      return new Point(point1.X - point2.X, point1.Y - point2.Y);
    }
    public static Point operator +(Point point1, Point point2)
    {
      return new Point(point1.X + point2.X, point1.Y + point2.Y);
    }
    public static Point operator /(Point point, int i)
    {
      return new Point(point.X / i, point.Y / i);
    }
    public static Point operator *(Point point, int i)
    {
      return new Point(point.X * i, point.Y * i);
    }
    public static bool operator !=(Point point1, Point point2)
    {
      return !(point1 == point2);
    }
    public static bool operator ==(Point point1, Point point2)
    {
      if (System.Object.ReferenceEquals(point1, point2))
        return true;
      if ((object)point1 == null || (object)point2 == null)
        return false;
      return point1.X == point2.X && point1.Y == point2.Y;
    }
    public override bool Equals(object o)
    {
      if (o == null)
        return false;
      Point p = o as Point;
      return (object)p != null && p.X == this.X && p.Y == this.Y;
    }
    public bool Equals(Point p)
    {
      return (object)p != null && p.X == this.X && p.Y == this.Y;
    }
    public override int GetHashCode()
    {
      int hash = 9769;
      hash = hash * 8641 ^ X.GetHashCode();
      hash = hash * 8641 ^ Y.GetHashCode();
      return hash;
    }
    public override string ToString()
    {
      return string.Format("({0};{1})", X, Y);
    }
  }

  public class Map
  {
    public List<StringBuilder> map;
    public int Height { get; set; }
    public int Width { get; set; }
    public StringBuilder printMap;

    public Map(int height, int width)
    {
      Height = height;
      Width = width;
      map = new List<StringBuilder>();
      for (int i = 0; i < height; i++)
        map.Add(new StringBuilder(new string(' ', width)));
    }

    public bool isWithin(Point p, bool borders)
    {
      if (borders)
        return p.X < Height - 1 && p.X > 0 && p.Y < Width - 1 && p.Y > 0;
      else
        return p.X < Height && p.X >= 0 && p.Y < Width && p.Y >= 0;
    }

    public char this[Point p]
    {
      get { return map[p.X][p.Y]; }
      set { map[p.X][p.Y] = value; }
    }

    public char this[int x, int y]
    {
      get { return map[x][y]; }
      set { map[x][y] = value; }
    }
  }

  public class MazeMap : Map
  {
    public static char Wall = '#';
    public static char Empty = ' ';
    private Point exit;
    private HashSet<Point> unhogged = new HashSet<Point>();

    public Point Exit
    {
      get { return exit; }
    }

    public MazeMap(int height, int width, MazeMode mode)
      : base(height, width)
    {
      BuildByDivide(0, 0, height, width, ChooseOrientation(height, width), mode);
      Transform();
      PutExit();
    }

    public bool isWall(Point p)
    {
      return map[p.X][p.Y] == Wall;
    }
    public bool isWall(int x, int y)
    {
      return map[x][y] == Wall;
    }
    public bool isExit(Point p)
    {
      return p == exit;
    }
    public bool isExit(int x, int y)
    {
      return new Point(x, y) == exit;
    }
    public bool isFree(Point p)
    {
      return map[p.X][p.Y] == Empty;
    }
    public bool isFree(int x, int y)
    {
      return map[x][y] == Empty;
    }

    public bool areNeighboursWalls(int x, int y, params Sides[] list)
    {
      for (int i = 0; i < list.Count(); i++)
      {
        switch (list[i])
        {
          case Sides.up:
            if (!isWall(x - 1, y))
              return false;
            break;
          case Sides.left:
            if (!isWall(x, y - 1))
              return false;
            break;
          case Sides.down:
            if (!isWall(x + 1, y))
              return false;
            break;
          case Sides.right:
            if (!isWall(x, y + 1))
              return false;
            break;
        }
      }
      return true;
    }

    public List<Point> neighbours(Point p)
    {
      List<Point> result = new List<Point>();
      for (double i = 0; i != 2 * Math.PI; i += Math.PI / 2)
      {
        Point n = p + Extensions.Direction(i);
        if (isFree(n) || isExit(n))
          result.Add(n);
      }
      return result;
    }

    public HashSet<Point> Visibles(Player pl)
    {
      HashSet<Point> visibles = new HashSet<Point>();
      if (pl == null)
      {
        for (int i = 0; i < Height; i++)
          for (int j = 0; j < Width; j++)
            visibles.Add(new Point(i, j));
        return visibles;
      }
      int l = int.MaxValue, r = int.MaxValue;
      double k = pl.Dir.X == 0 ? Math.Acos(pl.Dir.Y) : -Math.Asin(pl.Dir.X);
      for (int i = 0; ; i++)
      {
        Point p = pl.Position + Extensions.Direction(k) * i;
        if (!isWall(p) && !isExit(p))
        {
          visibles.Add(p);
          visibles.Add(p + Extensions.Direction(k));
          visibles.Add(p + Extensions.Direction(k - Math.PI / 2));
          visibles.Add(p + Extensions.Direction(k + Math.PI / 2));
        }
        else
          break;
        for (int j = 0; j < l; j++)
        {
          Point pp = p + Extensions.Direction(k + Math.PI / 2) * j;
          if (!isWall(pp) && !isExit(p))
          {
            visibles.Add(pp);
            visibles.Add(pp + Extensions.Direction(k));
            visibles.Add(pp + Extensions.Direction(k + Math.PI / 2));
            visibles.Add(pp + Extensions.Direction(k + Math.PI));
          }
          else
          {
            l = j;
            break;
          }
        }
        for (int j = 0; j < r; j++)
        {
          Point pp = p + Extensions.Direction(k - Math.PI / 2) * j;
          if (!isWall(pp) && !isExit(p))
          {
            visibles.Add(pp);
            visibles.Add(pp + Extensions.Direction(k));
            visibles.Add(pp + Extensions.Direction(k - Math.PI / 2));
            visibles.Add(pp + Extensions.Direction(k - Math.PI));
          }
          else
          {
            r = j;
            break;
          }
        }
      }
      return visibles;
    }

    public string Print(Player pl = null)
    {
      //Console.CursorVisible = false;
      //if (sy == 0 && sx == 0)
      //{
        printMap = new StringBuilder(new string('█',(Width + 1) * Height));
        //map.ForEach(x => { printMap.Append(x.ToString()); printMap.Append("\n"); });
        for (int i = 0; i < Height; i++)
          printMap[i * (Width + 1) + Width] = '\n';
        //PrintMap = printMapBulider.ToString();
        //return;
      //}
        if (pl == null)
          unhogged = Visibles(pl);
        else
          unhogged.UnionWith(Visibles(pl));
      /*int si = Math.Max(sx - 3, 0);
      int ei = sx == 0 ? Height - 1 : Math.Min(sx + 3, Height - 1);
      for (int i = si; i <= ei; i++)
      {
        int sj = sy == 0 ? sy : Math.Max(sy - 3 + Math.Abs(sx - i), 0);
        int ej = sy == 0 ? Width - 1 : Math.Min(sy + 3 - Math.Abs(sx - i), Width - 1);
        for (int j = sj; j <= ej; j++)
          visibles.Add(new Point(i, j));
      }*/
      foreach (var p in unhogged)
      {
        int pos = p.X * (Width + 1) + p.Y;
        if (isWall(p.X, p.Y))
          if (p.X > 0 && p.Y > 0 && p.X < map.Count - 1 && p.Y < map[0].Length - 1 &&
              areNeighboursWalls(p.X, p.Y, Sides.up, Sides.right, Sides.left, Sides.down))
            //Console.Write('╬');
            printMap[pos] = '╬';
          else if (p.Y > 0 && p.X < map.Count - 1 && p.Y < map[0].Length - 1 &&
              areNeighboursWalls(p.X, p.Y, Sides.right, Sides.left, Sides.down))
            //Console.Write('╦');
            printMap[pos] = '╦';
          else if (p.X > 0 && p.Y > 0 && p.Y < map[0].Length - 1 &&
              areNeighboursWalls(p.X, p.Y, Sides.up, Sides.right, Sides.left))
            //Console.Write('╩');
            printMap[pos] = '╩';
          else if (p.X > 0 && p.X < map.Count - 1 && p.Y < map[0].Length - 1 &&
              areNeighboursWalls(p.X, p.Y, Sides.up, Sides.right, Sides.down))
            //Console.Write('╠');
            printMap[pos] = '╠';
          else if (p.X > 0 && p.Y > 0 && p.X < map.Count - 1 &&
              areNeighboursWalls(p.X, p.Y, Sides.up, Sides.left, Sides.down))
            //Console.Write('╣');
            printMap[pos] = '╣';
          else if (p.X < map.Count - 1 && p.Y < map[0].Length - 1 &&
              areNeighboursWalls(p.X, p.Y, Sides.right, Sides.down))
            //Console.Write('╔');
            printMap[pos] = '╔';
          else if (p.Y > 0 && p.X < map.Count - 1 &&
              areNeighboursWalls(p.X, p.Y, Sides.left, Sides.down))
            //Console.Write('╗');
            printMap[pos] = '╗';
          else if (p.X > 0 && p.Y < map[0].Length - 1 &&
              areNeighboursWalls(p.X, p.Y, Sides.up, Sides.right))
            //Console.Write('╚');
            printMap[pos] = '╚';
          else if (p.X > 0 && p.Y > 0 &&
              areNeighboursWalls(p.X, p.Y, Sides.up, Sides.left))
            //Console.Write('╝');
            printMap[pos] = '╝';
          else if ((p.X > 0 && areNeighboursWalls(p.X, p.Y, Sides.up)) ||
                   (p.X < map.Count - 1 && areNeighboursWalls(p.X, p.Y, Sides.down)))
            //Console.Write('║');
            printMap[pos] = '║';
          else if ((p.Y > 0 && areNeighboursWalls(p.X, p.Y, Sides.left)) ||
                   (p.Y < map[0].Length - 1 && areNeighboursWalls(p.X, p.Y, Sides.right)))
            //Console.Write('═');
            printMap[pos] = '═';
          else
            //Console.Write('•');
            printMap[pos] = '•';
        else if (isFree(p) || isExit(p))
          //Console.Write(map[i][p.Y]);
          printMap[pos] = ' ';
        else
          //Console.Write(' ');
          printMap[pos] = map[p.X][p.Y];
      }
        //Console.WriteLine();
      return printMap.ToString();
    }

    private void BuildByDivide(int x, int y, int _height, int _width, Orientation or, MazeMode mode)
    {
      if (_width < 2 || _height < 2)
        return;
      if (_width < 3 || _height < 3)
        if (StaticRandom.Next(0, 3) > 1)
          return;
      bool hor = or == Orientation.HOR;
      int hy1, hx1, hy2, hx2, hx3, hy3;

      int wy = y + (hor ? 0 : StaticRandom.Next(_width - 2));
      int wx = x + (hor ? StaticRandom.Next(_height - 2) : 0);
      if ((mode == MazeMode.OneWay) || _height < 5 || _width < 5)
      {
        hy1 = hy2 = hy3 = wy + (hor ? StaticRandom.Next(_width) : 0);
        hx1 = hx2 = hx3 = wx + (hor ? 0 : StaticRandom.Next(_height));
      }
      else if (!hor && _width >= 5 && _width <= 15)
      {
        hy1 = hy2 = hy3 = wy;
        hx1 = wx + StaticRandom.Next(_height / 2 - 1);
        hx2 = hx3 = wx + StaticRandom.Next(_height / 2, _height);
      }
      else if (hor && _height >= 5 && _height <= 15)
      {
        hy1 = wy + StaticRandom.Next(_width / 2 - 1);
        hx1 = hx2 = hx3 = wx;
        hy2 = hy3 = wy + StaticRandom.Next(_width / 2, _width);
      }
      else
      {
        hy1 = wy + (hor ? StaticRandom.Next(_width / 3 - 1) : 0);
        hx1 = wx + (hor ? 0 : StaticRandom.Next(_height / 3));
        hy2 = wy + (hor ? StaticRandom.Next(_width / 3 + 1, 2 * _width / 3 - 1) : 0);
        hx2 = wx + (hor ? 0 : StaticRandom.Next(_height / 3 + 1, 2 * _height / 3 - 1));
        hy3 = wy + (hor ? StaticRandom.Next(2 * _width / 3 + 1, _width) : 0);
        hx3 = wx + (hor ? 0 : StaticRandom.Next(2 * _height / 3 + 1, _height));
      }

      for (int i = 0; i < (hor ? _width : _height); i++)
      {
        if (!((wx == hx1 && wy == hy1) || (wx == hx2 && wy == hy2) || (wx == hx3 && wy == hy3)))
          if (map[wx][wy] != ' ')
            map[wx][wy] = 'b';
          else
            map[wx][wy] = hor ? 'h' : 'v';
        wy += (hor ? 1 : 0);
        wx += (hor ? 0 : 1);
      }

      int newh = hor ? wx - x + 1 : _height;
      int neww = hor ? _width : wy - y + 1;
      BuildByDivide(x, y, newh, neww, ChooseOrientation(newh, neww), mode);
      newh = hor ? _height - wx + x - 1 : _height;
      neww = hor ? _width : _width - wy + y - 1;
      BuildByDivide(hor ? wx + 1 : x, hor ? y : wy + 1, newh, neww, ChooseOrientation(newh, neww), mode);
    }

    private void Transform()
    {
      map.Insert(0, new StringBuilder(new string('#', map[0].Length * 2 + 1)));
      for (int i = 1; i < map.Count - 1; i += 2)
      {
        map.Insert(i + 1, new StringBuilder(new string(' ', map[i].Length * 2 - 1)));
        map[i].Insert(0, '#');
        map[i + 1].Insert(0, '#');
        for (int j = 1; j < map[i].Length - 1; j += 2)
        {
          switch (map[i][j])
          {
            case 'h':
              map[i][j] = ' ';
              map[i].Insert(j + 1, ' ');
              map[i + 1][j] = '#';
              map[i + 1][j + 1] = '#';
              break;
            case 'b':
              map[i][j] = ' ';
              map[i].Insert(j + 1, '#');
              map[i + 1][j] = '#';
              map[i + 1][j + 1] = '#';
              break;
            case 'v':
              map[i][j] = ' ';
              map[i].Insert(j + 1, '#');
              map[i + 1][j + 1] = '#';
              break;
            case ' ':
              map[i].Insert(j + 1, ' ');
              if (map[i][j + 2] == ' ' || map[i][j + 2] == 'v')
                map[i + 1][j + 1] = ' ';
              else
                map[i + 1][j + 1] = '#';
              break;
            default:
              break;
          }
        }
        if (map[i][map[i].Length - 1] == 'h')
        {
          map[i + 1][map[i].Length - 1] = '#';
          map[i][map[i].Length - 1] = ' ';
        }
        map[i].Append('#');
        map[i + 1].Append('#');
      }
      map[map.Count - 1].Insert(0, '#');
      for (int j = 1; j < map[map.Count - 1].Length - 1; j += 2)
        if (map[map.Count - 1][j] == 'v')
        {
          map[map.Count - 1][j] = ' ';
          map[map.Count - 1].Insert(j + 1, '#');
        }
        else
        {
          map[map.Count - 1][j] = ' ';
          map[map.Count - 1].Insert(j + 1, ' ');
        }
      map[map.Count - 1].Append('#');
      map.Add(new StringBuilder(new string('#', map[0].Length)));
      Height = map.Count;
      Width = map[0].Length;
    }

    private void PutExit()
    {
      switch (StaticRandom.Next(4))
      {
        case 0:
          int k = StaticRandom.Next(1, Height - 1);
          while (isWall(k, 1))
            k = StaticRandom.Next(1, Height - 1);
          map[k][0] = 'e';
          exit = new Point(k, 0);
          break;
        case 1:
          k = StaticRandom.Next(1, Width - 1);
          while (isWall(1, k))
            k = StaticRandom.Next(1, Width - 1);
          map[0][k] = 'e';
          exit = new Point(0, k);
          break;
        case 2:
          k = StaticRandom.Next(1, Height - 1);
          while (isWall(k, Width - 2))
            k = StaticRandom.Next(1, Height - 1);
          map[k][Width - 1] = 'e';
          exit = new Point(k, Width - 1);
          break;
        case 3:
          k = StaticRandom.Next(1, Width - 1);
          while (isWall(Height - 2, k))
            k = StaticRandom.Next(1, Width - 1);
          map[Height - 1][k] = 'e';
          exit = new Point(Height - 1, k);
          break;
      }
    }

    private Orientation ChooseOrientation(int height, int width)
    {
      if (width < height)
        return Orientation.HOR;
      else if (width > height)
        return Orientation.VER;
      else
        return StaticRandom.Next(2) == 0 ? Orientation.VER : Orientation.HOR;
    }
  }
}
