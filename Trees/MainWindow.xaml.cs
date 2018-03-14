using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.StaticRandom;

namespace Trees
{
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      List<Draw.Trunk> list = new List<Draw.Trunk>();
      for (int i = 0; i < 8; i++)
      {
        double thickness = StaticRandom.NextDouble(5, 20);
        double height = thickness * StaticRandom.NextDouble(3.5, 4.5);
        list.Add(new Draw.Trunk(i * 150 + 50, StaticRandom.Next(200, 600),
                                thickness, height));
      }
      //Draw.Trunk tree = new Draw.Trunk(500, 600, 18, 80);
      foreach (var tree in list)
      {
        foreach (var item in tree.ListTrunk1)
          canvas1.Children.Add(item);
        Path p = new Path();
        p.Data = tree.StartOfTrees1.Tree;
        p.Fill = Brushes.Black;
        canvas1.Children.Add(p);
      }
    }

    public void Draw(Draw.TreeViaShape r)
    {
      canvas1.Children.Add(r.Brunch);
      if (r.Center != null)
        Draw(r.Center);
      if (r.Left != null)
        Draw(r.Left);
      if (r.Right != null)
        Draw(r.Right);
    }
  }
}
