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

namespace Mazes
{
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      /*if (e.NewSize.Height > 500)
      {
        textBox1.RenderSize = e.NewSize;
        textBox1.FontSize *= e.NewSize.Height / e.PreviousSize.Height;
        this.SizeToContent = SizeToContent.WidthAndHeight;
      }*/
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Left || e.Key == Key.Up ||
          e.Key == Key.Down || e.Key == Key.Right || e.Key == Key.A)
      {
        Maze.ArrowDown = true;
        Maze.KeyDown = e.Key;
      }
    }
  }

  public static class Extensions
  {
    public static Point Direction(double j)
    {
      return new Point(-(int)Math.Sin(j), (int)Math.Cos(j));
    }
  }
}
