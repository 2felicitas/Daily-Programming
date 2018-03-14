using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interpreter
{
  public enum InTypes {Function, Expression, Variable};
  public enum Tokens {Operator, Function, Number, Variable, LB, RB, Comma};

  static class INTMain
  {
    static Dictionary<string, double> savedVariables;
    static Dictionary<string, Tuple<string[], string>> savedFunctions;
    static void Main(string[] args)
    {
      savedVariables = new Dictionary<string, double>();
      savedFunctions = new Dictionary<string, Tuple<string[], string>>();
      while (true)
      {
        try
        {
          string input = Console.ReadLine();
          switch (InterpretExpression(input))
          {
            case InTypes.Function:
              Console.WriteLine(SolveFunction(input));
              break;
            case InTypes.Expression:
              Console.WriteLine(SolveExpression(input));
              break;
            case InTypes.Variable:
              Console.WriteLine(SolveVariable(input));
              break;
          }
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
        }
      }
    } 

    static InTypes InterpretExpression(string input)
    {
      if (!input.Contains('='))
        return InTypes.Expression;
      string[] parts = input.Split('=');
      if (!parts[0].Contains('('))
        return InTypes.Variable;
      return InTypes.Function;
    }

    static double SolveExpression(string input)
    {
      Stack<Token> variables = new Stack<Token>();
      Queue<Token> postfix = Infix2Postfix(Input2List(input));
      foreach (var token in postfix)
        if (token.Type == Tokens.Operator)
        {
          double op1 = double.Parse(variables.Pop().Name);
          double op2 = double.Parse(variables.Pop().Name);
          double result = 0;
          switch (token.Name)
          {
            case "^":
              result = Math.Pow(op2, op1);
              break;
            case "*":
              result = op2 * op1;
              break;
            case "/":
              if (op1 != 0)
                result = op2 / op1;
              else
                throw new Exception("Division by 0");
              break;
            case "+":
              result = op2 + op1;
              break;
            case "-":
              result = op2 - op1;
              break;
          }
          variables.Push(new Token(Tokens.Number, result));
        }
        else if (token.Type == Tokens.Function)
        {
          string name = token.Name;
          int varCount = savedFunctions[name].Item1.Length;
          string formula = savedFunctions[name].Item2;
          for (int i = 0; i < varCount; i++)
            formula = formula.Replace(savedFunctions[name].Item1[i], variables.Pop().Name);
          variables.Push(new Token(Tokens.Number, SolveExpression(formula)));   
        }
        else if (token.Type == Tokens.Variable)
          variables.Push(new Token(Tokens.Number, savedVariables[token.Name]));
        else if (token.Type == Tokens.Number)
          variables.Push(token);
      return double.Parse(variables.Pop().Name);
    }

    static TokenList Input2List(string input)
    {
      TokenList infix = new TokenList();
      for (int i = 0; i < input.Length; i++)
      {
        if (input[i].isDigit() || (input[i] == '-' && (infix.Last == null || infix.Last.Type != Tokens.Number)))
        {
          StringBuilder s = new StringBuilder();
          s.Append(input[i]);
          while (i + 1 < input.Length &&
                (input[i + 1].isDigit() ||
                 input[i + 1].isSpace() ||
                 input[i + 1].isDot()))
            if (!input[++i].isSpace())
              s.Append(input[i]);
          infix.Add(Tokens.Number, s);
          continue;
        }
        else if (input[i].isLetter())
        {
          StringBuilder s = new StringBuilder();
          s.Append(input[i]);
          while (i + 1 < input.Length && 
                 (input[i + 1].isLetter() ||
                 input[i + 1].isDigit()))
            s.Append(input[++i]);
          if (i + 1 < input.Length && input[i + 1] == '(')
          {
            i++;
            StringBuilder inception = new StringBuilder();
            int brackets = 1;
            int commas = 0;
            while (brackets > 0)
            {
              i++;
              if (i > input.Length)
                throw new Exception("brackets don't balanced");
              if (input[i] == '(')
                brackets++;
              else if (input[i] == ')')
                brackets--;
              if (brackets > 0)
                inception.Append(input[i]);
              if (input[i].isComma() && brackets == 1)
                commas++;
            }
            if (inception.Length == 0)
              infix.Add(Tokens.Function, s.Append(0));
            else
              infix.Add(Tokens.Function, s.Append(commas + 1));
            infix.Add(Tokens.LB, '(');
            infix.AddRange(Input2List(inception.ToString()));
            infix.Add(Tokens.RB, ')');
          }
          else
            infix.Add(Tokens.Variable, s);
          continue;
        }
        else if (input[i].isOperator())
          infix.Add(Tokens.Operator, input[i]);
        else if (input[i] == '(')
          infix.Add(Tokens.LB, '(');
        else if (input[i] == ')')
          infix.Add(Tokens.RB, ')');
        else if (input[i] == ',')
          infix.Add(Tokens.Comma, ',');
      }
      if (!infix.BracketsBalance())
        throw new Exception("Brackets don't balanced");
      return infix;
    }

    static Queue<Token> Infix2Postfix(TokenList infix)
    {
      Queue<Token> postfix = new Queue<Token>();
      Stack<Token> opStack = new Stack<Token>();
      while (true)
      {
        Token token = infix.First;
        if (token != null)
        {
          if (token.Type == Tokens.Number || token.Type == Tokens.Variable)
            postfix.Enqueue(token);
          else if (token.Type == Tokens.Function)
            opStack.Push(token);
          else if (token.Type == Tokens.Comma)
            while (opStack.Peek().Type != Tokens.LB)
              postfix.Enqueue(opStack.Pop());
          else if (token.Type == Tokens.Operator)
          {
            while (opStack.Count > 0 && opStack.Peek().Type == Tokens.Operator &&
                   Operators.Precedence(opStack.Peek().Name) >= Operators.Precedence(token.Name))
              postfix.Enqueue(opStack.Pop());
            opStack.Push(token);
          }
          else if (token.Type == Tokens.LB)
            opStack.Push(token);
          else if (token.Type == Tokens.RB)
          {
            while (opStack.Peek().Type != Tokens.LB)
              postfix.Enqueue(opStack.Pop());
            opStack.Pop();
            if (opStack.Count > 0 && opStack.Peek().Type == Tokens.Function)
              postfix.Enqueue(opStack.Pop());
          }
        }
        else
        {
          while (opStack.Count > 0)
            postfix.Enqueue(opStack.Pop());
          break;
        }
        infix.RemoveAt(0);
      }
      return postfix;
    }

    static string SolveFunction(string input)
    {
      string[] parts = input.Split('=');
      string[] leftpart = parts[0].Trim().Split("( , )".Split(' '), StringSplitOptions.RemoveEmptyEntries);
      string name = leftpart[0].Trim();
      savedFunctions.Add(name + (leftpart.Length - 1).ToString(), 
                         new Tuple<string[], string>(leftpart.Skip(1).ToArray(), parts[1].Trim()));
      return parts[1].Trim();
    }

    static double SolveVariable(string input)
    {
      string[] parts = input.Split('=');
      string name = parts[0].Trim();
      if (savedVariables.Keys.Contains(name))
        savedVariables[name] = SolveExpression(parts[1].Trim());
      else
        savedVariables.Add(name, SolveExpression(parts[1].Trim()));
      return savedVariables[name];
    }
  }

  public class Token
  {
    Tokens type;
    string name;

    public Token(Tokens t, object o)
    {
      type = t;
      name = o.ToString();
    }

    public Tokens Type
    {
      get { return type; }
    }
    public string Name
    {
      get { return name; }
    }
  }
  public class TokenList
  {
    List<Token> list;

    public int Count
    {
      get { return list.Count; }
    }
    public Token First
    {
      get { return list.FirstOrDefault(); }
    }
    public Token Last
    {
      get { return list.LastOrDefault(); }
    }
    public TokenList()
    {
      list = new List<Token>();
    }

    public void Add(Tokens t, object o)
    {
      list.Add(new Token(t, o.ToString()));
    }
    public void AddRange(TokenList collection)
    {
      list.AddRange(collection.list);
    }
    public void RemoveAt(int n)
    {
      list.RemoveAt(n);
    }
    public bool BracketsBalance()
    {
      return list.Count(x => x.Type == Tokens.LB) == list.Count(x => x.Type == Tokens.RB);
    }
    public Token this[int n]
    {
      get { return list[n]; }
    }

  }
  public static class Extensions
  {
    public static bool isNumber(this string s)
    {
      double result;
      return double.TryParse(s, out result);
    }
    public static bool isOperator(this char c)
    {
      return Operators.Set.Contains(c);
    }
    public static bool isDigit(this char c)
    {
      return c >= '0' && c <= '9';
    }
    public static bool isDot(this char c)
    {
      return c == '.';
    }
    public static bool isSpace(this char c)
    {
      return c == ' ';
    }
    public static bool isComma(this char c)
    {
      return c == ',';
    }
    public static bool isLetter(this char c)
    {
      return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }
  }
  public static class Operators
  {
    public static char[] Set = {'+', '-', '*', '^', '/'};
    public static int Precedence(string s)
    {
      switch (s)
      {
        case "^":
          return 3;
        case "*":
        case "/":
          return 2;
        case "+":
        case "-":
          return 1;
        default:
          return 0;
      }
    }
  }
}
