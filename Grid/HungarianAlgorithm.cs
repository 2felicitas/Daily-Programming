using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schedule
{
  class HungarianAlgorithm
  {
    private readonly double[,] costMatrix;
    private int n;
    private double[] minRow;
    private double[] minColumn;
    private bool[] s;
    private bool[] t;
    private int[] matchX;
    private int[] matchY;
    private int maxMatch;
    private double[] slack;
    private int[] slackx;
    private int[] prev;

    public HungarianAlgorithm(double[,] _costMatrix)
    {
      costMatrix = _costMatrix;
    }

    public int[] Solve()
    {
      n = costMatrix.GetLength(0);

      minRow = new double[n];
      minColumn = new double[n];
      s = new bool[n];
      t = new bool[n];
      matchX = new int[n];
      matchY = new int[n];
      slack = new double[n];
      slackx = new int[n];
      prev = new int[n];


      InitMatches();

      if (n != costMatrix.GetLength(1))
        return null;

      InitMins();

      maxMatch = 0;

      InitialMatching();

      var q = new Queue<int>();

      #region augment

      while (maxMatch != n)
      {
        q.Clear();

        InitSt();
        //Array.Clear(S,0,n);
        //Array.Clear(T, 0, n);

        //parameters for keeping the position of root node and two other nodes
        var root = 0;
        int x;
        var y = 0;

        //find root of the tree
        for (x = 0; x < n; x++)
        {
          if (matchX[x] != -1)
            continue;
          q.Enqueue(x);
          root = x;
          prev[x] = -2;

          s[x] = true;
          break;
        }

        //init slack
        for (var i = 0; i < n; i++)
        {
          slack[i] = costMatrix[root, i] - minRow[root] - minColumn[i];
          slackx[i] = root;
        }

        //finding augmenting path
        while (true)
        {
          while (q.Count != 0)
          {
            x = q.Dequeue();
            var lxx = minRow[x];
            for (y = 0; y < n; y++)
            {
              if (costMatrix[x, y] != lxx + minColumn[y] || t[y])
                continue;
              if (matchY[y] == -1)
                break; //augmenting path found!
              t[y] = true;
              q.Enqueue(matchY[y]);

              AddToTree(matchY[y], x);
            }
            if (y < n)
              break; //augmenting path found!
          }
          if (y < n)
            break; //augmenting path found!
          UpdateMins(); //augmenting path not found, update labels

          for (y = 0; y < n; y++)
          {
            //in this cycle we add edges that were added to the equality graph as a
            //result of improving the labeling, we add edge (slackx[y], y) to the tree if
            //and only if !T[y] &&  slack[y] == 0, also with this edge we add another one
            //(y, yx[y]) or augment the matching, if y was exposed

            if (t[y] || slack[y] != 0)
              continue;
            if (matchY[y] == -1) //found exposed vertex-augmenting path exists
            {
              x = slackx[y];
              break;
            }
            t[y] = true;
            if (s[matchY[y]])
              continue;
            q.Enqueue(matchY[y]);
            AddToTree(matchY[y], slackx[y]);
          }
          if (y < n)
            break;
        }

        maxMatch++;

        //inverse edges along the augmenting path
        int ty;
        for (int cx = x, cy = y; cx != -2; cx = prev[cx], cy = ty)
        {
          ty = matchX[cx];
          matchY[cy] = cx;
          matchX[cx] = cy;
        }
      }

      #endregion

      return matchY;
    }

    private void InitMatches()
    {
      for (var i = 0; i < n; i++)
      {
        matchX[i] = -1;
        matchY[i] = -1;
      }
    }

    private void InitMins()
    {
      for (int i = 0; i < n; i++)
      {
        var mr = costMatrix[i, 0];
        for (var j = 0; j < n; j++)
        {
          if (costMatrix[i, j] < mr)
            mr = costMatrix[i, j];
          if (mr == 0)
            break;
        }
        minRow[i] = mr;
      }
      for (int j = 0; j < n; j++)
      {
        var mc = costMatrix[0, j] - minRow[0];
        for (var i = 0; i < n; i++)
        {
          if (costMatrix[i, j] - minRow[i] < mc)
            mc = costMatrix[i, j] - minRow[i];
          if (mc == 0)
            break;
        }
        minColumn[j] = mc;
      }
    }

    private void UpdateMins()
    {
      var delta = double.MaxValue;
      for (var i = 0; i < n; i++)
        if (!t[i])
          if (delta > slack[i])
            delta = slack[i];
      for (var i = 0; i < n; i++)
      {
        if (s[i])
          minRow[i] = minRow[i] + delta;
        if (t[i])
          minColumn[i] = minColumn[i] - delta;
        else
          slack[i] = slack[i] - delta;
      }
    }

    private void AddToTree(int x, int prevx)
    {
      //x-current vertex, prevx-vertex from x before x in the alternating path,
      //so we are adding edges (prevx, matchX[x]), (matchX[x],x)

      s[x] = true; //adding x to S
      prev[x] = prevx;

      var lxx = minRow[x];
      //updateing slack
      for (var y = 0; y < n; y++)
      {
        if (costMatrix[x, y] - lxx - minColumn[y] >= slack[y])
          continue;
        slack[y] = costMatrix[x, y] - lxx - minColumn[y];
        slackx[y] = x;
      }
    }

    private void InitialMatching()
    {
      for (var x = 0; x < n; x++)
      {
        for (var y = 0; y < n; y++)
        { 
          if (costMatrix[x, y] != minRow[x] + minColumn[y] || matchY[y] != -1)
            continue;
          matchX[x] = y;
          matchY[y] = x;
          maxMatch++;
          break;
        }
      }
    }

    private void InitSt()
    {
      for (var i = 0; i < n; i++)
      {
        s[i] = false;
        t[i] = false;
      }
    }

  }
}
