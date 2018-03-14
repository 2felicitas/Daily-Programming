using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace Mazes
{
  public enum MazeMode { OneWay, MultipleWays }
  public enum ControlMode { Manual, Auto }
  public enum Orientation { VER, HOR }
  public enum Sides { up, left, down, right }

  public partial class Maze : Notifier
  {
    private MazeMap map;
    private String printMap;
    private double fontSize;
    private FontWeight bold;
    private Player pl;
    private List<Troll> trolls;
    public static bool ArrowDown { get; set; }
    public static Key KeyDown { get; set; }

    #region{Свойства для биндинга}
    public string PrintMap
    {
      get { return printMap; }
      set { SetField(ref printMap, value, "PrintMap"); }
    }
    public double FontSize 
    {
      get { return fontSize;}
      set { SetField(ref fontSize, value, "FontSize");}
    }
    public FontWeight Bold 
    {
      get { return bold;}
      set { SetField(ref bold, value, "Bold"); }
    }
    public ICommand StartCommand { get; set; }
    #endregion

    public Maze()
    {
      StartCommand = new RelayCommand(arg => Start());
      ArrowDown = false;
    }

    public void Start()
    {
      int height = 55;
      int width = 95;
      Create(height, width, MazeMode.MultipleWays);
      ControlMode mode = ControlMode.Manual;
      Action Control;
      if (mode == ControlMode.Auto)
        Control = AutoControl;
      else
        Control = ManualControl;

      genPlayer();
      genTrolls(Math.Min(height, width) / 8);

      PrintMap = map.Print(pl);
      Task a = Task.Factory.StartNew(() => Control());
      a.ContinueWith(task => { MessageBox.Show("Error");});  
    }

    public void Create(int _height, int _width, MazeMode mode)
    {
      map = new MazeMap((_height + 1) / 2, (_width + 1) / 2, mode);
      FontSize = -21.7297 * Math.Log(0.01129 * _height) - 1;
      Bold = FontWeights.Bold;
    }

    private void Eat() 
    {
      pl.isDead = true;
      PrintMap = map.Print();
      MessageBox.Show("You were eaten");
    }
  }
}
