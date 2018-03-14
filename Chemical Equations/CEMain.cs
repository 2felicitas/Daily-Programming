using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Chemical_Equations
{
  class CEMain
  {
    static char[] sep = {'+', ' '};
    static void Main(string[] args)
    {
      string[] input = File.ReadAllLines("input.txt");
      foreach (string equation in input)
      {
        Console.WriteLine(equation);
        Console.WriteLine(SolveEquation(equation));
        Console.WriteLine();
      }
      Console.ReadKey();
    }
    static string SolveEquation(string equation)
    {
      string[] parts = equation.Split("->".Split(' '), StringSplitOptions.None);
      string[] left_elements = parts[0].Split(sep, StringSplitOptions.RemoveEmptyEntries);
      string[] right_elements = parts[1].Split(sep, StringSplitOptions.RemoveEmptyEntries);
      MoleculesList left;
      MoleculesList right;
      if ((left = GetElements(left_elements)) == null || (right = GetElements(right_elements)) == null)
        return "Nope!";
      Dictionary<string, int> left_full = FullElements(left);
      Dictionary<string, int> right_full = FullElements(right);
      if (left_full.Count != right_full.Count || !left_full.Keys.All(right_full.ContainsKey))
        return "Nope!";
      double[][] slae = new double[left_full.Count][];
      for (int i = 0; i < left_full.Count; i++)
        slae[i] = new double[left.Count + right.Count];
      for (int i = 0; i < left_full.Count; i++)
      {
        for (int j = 0; j < left.Count; j++)
          if (left[j].ContainsKey(left_full.Keys.ElementAt(i)))
            slae[i][j] = left[j][left_full.Keys.ElementAt(i)];
          else
            slae[i][j] = 0;
        for (int j = left.Count; j < left.Count + right.Count; j++)
          if (right[j - left.Count].ContainsKey(left_full.Keys.ElementAt(i)))
            slae[i][j] = -right[j - left.Count][left_full.Keys.ElementAt(i)];
          else
            slae[i][j] = 0;
      }
      var a = GetGauss(slae);
      StringBuilder answer = new StringBuilder();
      for (int i = 0; i < left.Count; i++)
      {
        if (a[i] != 1)
          answer.Append(a[i]);
        answer.Append(left_elements[i]);
        if (i != left.Count - 1)
          answer.Append(" + ");
        else
          answer.Append(" -> ");
      }
      for (int i = 0; i < right.Count; i++)
      {
        if (a[i + left.Count] != 1)
          answer.Append(a[i + left.Count]);
        answer.Append(right_elements[i]);
        if (i != right.Count - 1)
          answer.Append(" + ");
      }

      return answer.ToString();
    }
    static MoleculesList GetElements(string[] molecules) 
    {
      MoleculesList result = new MoleculesList(molecules.Length);
      int molNumber = 0;
      int atomNumber;
      int bracketsNumber = 0;
      ;
      foreach (var mol in molecules)
      {
        atomNumber = 0;
        for (int i = 0; i < mol.Length; i++)
        {
          if (mol[i] == '(')
          {
            bracketsNumber = atomNumber;
            continue;
          }
          if (mol[i] == ')')
          {
            i++;
            if (++i < mol.Length && mol[i] >= '0' && mol[i] <= '9')
              result.AddBrackets(molNumber, bracketsNumber, atomNumber, int.Parse(mol.Substring(i - 1, 2)));
            else
              result.AddBrackets(molNumber, bracketsNumber, atomNumber, int.Parse(mol[--i].ToString()));
            continue;
          }
          if (mol[i] >= 'A' && mol[i] <= 'Z')
          {
            string element = new string(mol[i], 1);
            i++;
            if (i < mol.Length && mol[i] >= 'a' && mol[i] <= 'z')
              element += mol[i++];
            if (i < mol.Length && mol[i] > '0' && mol[i] <= '9')
              if (++i < mol.Length && mol[i] >= '0' && mol[i] <= '9')
                result.Add(molNumber, element, int.Parse(mol.Substring(i - 1, 2)));
              else
                result.Add(molNumber, element, int.Parse(mol[--i].ToString()));
            else if (i >= mol.Length || (mol[i] >= 'A' && mol[i] <= 'Z') || mol[i] == '(' || mol[i] == ')')
            {
              result.Add(molNumber, element, 1);
              i--;
            }
            else
              return null;
            atomNumber++;
          }
        }
        molNumber++;
      }
      return result;
    }
    static int[] GetGauss(double[][] slae)
    {
      int w = slae[0].Length;
      int h = slae.Length;
      double[] dAns = new double[w];
      int[] iAns = new int[w];
      double divCoef = 1;
      for (int k = 0; k < Math.Min(w - 1, h); k++)                  // Проход по диагонали
      {
        int l = k;                                                  //
        while (l + 1 < h && slae[l][k] == 0)                        //
          l++;                                                      // Выведение наверх строчки
        if (l != k)                                                 //   с ненулевым начальным элементом
        {                                                           // 
          var temp = slae[k];                                       //
          slae[k] = slae[l];                                        //
          slae[l] = temp;                                           //
        }
        //
        if (slae[k][k] != 1)                                        //
        {                                                           //
          var pivot = slae[k][k];                                   //
          for (int j = k; j < w; j++)                               // Делаем начальный элемент равным единице,
            slae[k][j] /= pivot;                                    //   проходя по строке и деля её на него
          divCoef *= pivot;
        }

        for (int i = k + 1; i < h; i++)                             // Проход по столбцам для обнуления
          if (slae[i][k] != 0)                                      //   нижнего треугольника
          {
            double m = slae[i][k] / slae[k][k];
            for (int j = k; j < w; j++)                             // Обнуление k столбца ниже k строки
              slae[i][j] -= m * slae[k][j];                         //   (проход по строкам)
            divCoef *= slae[k][k];
          }
      }
      for (int k = Math.Min(w - 2, h - 1); k >= 0; k--)             // Проход по диагонали снизу вверх для
                                                                    //   создания единичной матрицы в левой части
        for (int i = k - 1; i >= 0; i--)                            // Проход по столбцам для обнуления строки над 1
          if (slae[i][k] != 0)
          {
            double m = slae[i][k] / slae[k][k];                     // Обнуление k столбца выше k строки
            for (int j = k; j < w; j += w - k - 1)                  //   (проход по строкам)
              slae[i][j] -= m * slae[k][j];
            divCoef *= slae[k][k];
          }
        dAns[w - 1] = 1;
      for (int i = 0; i < w-1; i++)
        dAns[i] = -slae[i][w - 1];
      if (dAns.All(i => i % 1 == 0))
        for (int j = 0; j < w; j++)
          iAns[j] = (int)dAns[j];
      else
      {
        for (int j = 0; j < w; j++)
        {
          dAns[j] = dAns[j] * Math.Abs(divCoef);
          iAns[j] = (int)Math.Round(dAns[j]);
        }
        int gcd = GCD(iAns);
        for (int i = 0; i < w; i++)
          iAns[i] /= gcd;
      }
      return iAns;
    }
    static Dictionary<string, int> FullElements(MoleculesList list)
    {
      Dictionary<string, int> result = new Dictionary<string, int>();
      foreach (Dictionary<string, int> elem in list)
        foreach (var atom in elem.Keys)
          if (result.ContainsKey(atom))
            result[atom] += elem[atom];
          else
            result.Add(atom, elem[atom]);
      return result;
    }

    static int GCD(int[] numbers)
    {
      return numbers.Aggregate(GCD);
    }

    static int GCD(int a, int b)
    {
      return b == 0 ? a : GCD(b, a % b);
    }
  }

  class MoleculesList : IEnumerable
  {
    List<Dictionary<string, int>> list;

    public int Count
    { get { return list.Count; } }

    public MoleculesList(int n)
    {
      list = new List<Dictionary<string, int>>();
      for (int i = 0; i < n; i++)
        list.Add(new Dictionary<string, int>());
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return list.GetEnumerator();
    }

    public Dictionary<string, int> this[int mol]
    { get { return list[mol]; } }

    public bool Add(int dictNum, string el, int amount)
    {
      if (dictNum >= list.Count)
        return false;
      if (list[dictNum].ContainsKey(el))
        list[dictNum][el] += amount;
      else
        list[dictNum].Add(el, amount);
      return true;
    }

    public bool AddBrackets(int dictNum, int atomNumStart, int atomNumEnd, int amount)
    {
      for (int i = atomNumStart; i < atomNumEnd; i++)
        list[dictNum][list[dictNum].ElementAt(i).Key] *= amount;
      return true;
    }
  }
}
