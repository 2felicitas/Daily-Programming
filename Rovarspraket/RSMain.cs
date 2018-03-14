using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rovarspraket
{
  class RSMain
  {
    static string vowels = "aeiouyAEIOUYаеёиоуыэюяАЕЁИОУЫЭЮЯ";

    static void Main(string[] args)
    {
      Console.WriteLine("Выберите режим \n Кодировать - 0 \n Декодировать - 1");
      if (Console.ReadLine() == "0")
        Code();
      else
        Decode();
    }

    static void Code()
    {
      Console.Clear();
      Console.WriteLine("Введите фразу:");
      string input = Console.ReadLine();
      foreach (var ch in input)
      {
        if (!char.IsLetter(ch) || vowels.Contains(ch))
          Console.Write(ch);
        else
          Console.Write(ch.ToString() + 'o' + ch.ToString().ToLower());
      }
      Console.ReadKey();
    }

    static void Decode()
    {
      Console.Clear(); 
      Console.WriteLine("Введите фразу:");
      string input = Console.ReadLine();
      for(int i = 0; i < input.Length; i++)
      {
        if (!char.IsLetter(input[i]) || vowels.Contains(input[i]))
          Console.Write(input[i]);
        else
        {
          i += 2;
          Console.Write(input[i]);
        }
      }
      Console.ReadKey();
    }
  }
}
