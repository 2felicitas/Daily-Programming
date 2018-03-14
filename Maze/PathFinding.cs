using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Priority_Queue;

namespace Mazes
{
  public static class PathFinding
  {
    public static List<Point> AStar(Point start, Point goal, Func<Point, List<Point>> neighbours)
    {
      SimplePriorityQueue<Point, double> open =  new SimplePriorityQueue<Point, double>();
      Dictionary<Point, Point> parent = new Dictionary<Point, Point>();
      Dictionary<Point, double> cost = new Dictionary<Point, double>();
      open.EnqueueWithoutDuplicates(start, 0);
      parent[start] = null;
      cost[start] = 0;
      while (open.Count > 0)
      {
        Point current = open.Dequeue();
        if (current == goal)
          break;
        foreach (var next in neighbours(current))
        {
          double newcost = cost[current] + 1;
          if (!cost.ContainsKey(next) || newcost < cost[next])
          {
            cost[next] = newcost;
            open.Enqueue(next, Heuristic(next, goal) + newcost);
            parent[next] = current;
          }
        }
      }
      List<Point> result = new List<Point>();
      Point end = goal;
      while (true)
      {
        result.Add(end);
        if (parent[end] != null)
          end = parent[end];
        else
          break;
      }
      result.Reverse();
      return result;
    }

    private static double Heuristic(Point p, Point goal)
    {
      return (Math.Abs(p.X - goal.X) + Math.Abs(p.Y - goal.Y)) * 1.01;
    }
  }
}
