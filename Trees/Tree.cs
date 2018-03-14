using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.StaticRandom;

namespace Trees
{
  public class Draw
  {
    static double thick_coef = 0.9;
    static double height_coef = 0.95;
    public enum side { center, left, right };
    const double leafHOR = 2.0;
    const double leafVER = 3.0;
    const double rad2deg = 0.01745;

    public class Trunk
    {
      List<Shape> listTrunk1;
      TreeViaPath startOfTrees1;

      public List<Shape> ListTrunk1
      { get { return listTrunk1; } }
      public TreeViaPath StartOfTrees1
      { get { return startOfTrees1; } }


      public Trunk(double _x, double _y, double _thickness, double _height)
      {
        int n = StaticRandom.Next(1, 2);
        double new_x = _x;
        double new_y = _y;
        listTrunk1 = new List<Shape>();
        for (int i = 0; i < n; i++)
        {
          Polygon pol = new Polygon();
          pol.Stroke = Brushes.Black;
          pol.Fill = Brushes.Black;
          pol.FillRule = FillRule.Nonzero;
          pol.Points.Add(new Point(new_x , new_y));
          pol.Points.Add(new Point(new_x + (1 - thick_coef) * _thickness / 2, new_y - _height));
          pol.Points.Add(new Point(new_x + (1 + thick_coef) * _thickness / 2, new_y - _height));
          pol.Points.Add(new Point(new_x + _thickness, new_y));
          listTrunk1.Add(pol);
          _height = _height * height_coef;
          new_x += (1 - thick_coef) * _thickness / 2;
          _thickness = _thickness * thick_coef;
          new_y -= _height;
        }
        startOfTrees1 = new TreeViaPath();
        startOfTrees1.MakeATree(new_x, new_y, _thickness, _height, 0, 0, true);
      }
    }

    public class TreeViaShape
    {
      TreeViaShape center;
      TreeViaShape left;
      TreeViaShape right;
      TreeViaShape parent;
      Shape brunch;

      public TreeViaShape Center
      { get { return center; } set { center = value; } }
      public TreeViaShape Left
      { get { return left; } set { left = value; } }
      public TreeViaShape Right
      { get { return right; } set { right = value; } }
      public Shape Brunch
      { get { return brunch; } set { brunch = value; } }

      public TreeViaShape()
      {
      }
      public TreeViaShape(double _x, double _y, double _thickness, double _height, double depth, double angle = 0, bool polygon = false, bool leaves = false)
      {
        if (leaves)
        {
          Line leaf = new Line();
          leaf.Stroke = Brushes.Black;
          leaf.StrokeStartLineCap = PenLineCap.Round;
          leaf.StrokeEndLineCap = PenLineCap.Triangle;
          leaf.StrokeThickness = 2;
          leaf.X1 = _x;// + _thickness / 2 * Math.Cos(angle);
          leaf.Y1 = _y;// + _thickness / 2 * Math.Sin(angle);
          leaf.X2 = leaf.X1 + 5 * Math.Sin(angle);
          leaf.Y2 = leaf.Y1 - 5 * Math.Cos(angle);
          brunch = leaf;
          return;
        }
        if (polygon)
        {
          Polygon pol = new Polygon();
          pol.Stroke = Brushes.Black;
          pol.Fill = Brushes.Black;
          pol.FillRule = FillRule.Nonzero;
          pol.Points.Add(new Point(_x, _y));
          pol.Points.Add(new Point(_x + (1 - thick_coef) * _thickness / 2, _y - _height));
          pol.Points.Add(new Point(_x + (1 + thick_coef) * _thickness / 2, _y - _height));
          pol.Points.Add(new Point(_x + _thickness, _y));
          if (angle > 0)
            pol.RenderTransform = new RotateTransform(angle / rad2deg,
                                                      _thickness, 0);
          else
            pol.RenderTransform = new RotateTransform(angle / rad2deg,
                                                      0, 0);
        }
        else
        {
          brunch = new Rectangle();
          brunch.Height = _height;
          brunch.Width = _thickness;
          brunch.Stroke = Brushes.Black;
          brunch.Fill = Brushes.Black;
          Canvas.SetLeft(brunch, _x);
          Canvas.SetTop(brunch, _y);
          //if (angle > 0)
            //brunch.RenderTransform = new RotateTransform(angle / rad2deg,
                  //                                       _thickness,
                   //                                      _height);
          //else
            brunch.RenderTransform = new RotateTransform(angle / rad2deg,
                                                         0, _height);
        }
        _thickness = _thickness * thick_coef;
        //_y -= _height;
        if (_height > 5 && depth < 350)
        {
          _x += _height * Math.Sin(angle) + ((brunch.Width - _thickness) / 2) * Math.Cos(angle);
          _y += _height * (1 - Math.Cos(angle)) - ((brunch.Width - _thickness) / 2) * Math.Sin(-angle);
          if (StaticRandom.NextDouble() < 0.9 || _thickness > 4)
            center = new TreeViaShape(_x, _y - _height * height_coef,
                                      _thickness,
                                      _height * height_coef,
                                      depth + _height,
                                      angle,
                                      polygon);
          if (StaticRandom.NextDouble() < 0.7 && (angle > -110 * rad2deg || _height < 15))
          {
            double new_height = _height * StaticRandom.NextGaussian(0.8, 0.2, 0.3, 0.9);
            //double new_height = _height * StaticRandom.NextDouble(0.7, 0.9);
            left = new TreeViaShape(_x, _y - new_height,
                                    StaticRandom.NextDouble(_thickness * 0.6, _thickness * 0.9),
                                    new_height,
                                    depth + _height,
                                    angle - StaticRandom.NextDouble(10, 46) * rad2deg,
                                    polygon);
          }
          if (StaticRandom.NextDouble() < 0.7 && (angle < 110 * rad2deg || _height < 15))
          {
            double new_height = _height * StaticRandom.NextGaussian(0.8, 0.2, 0.3, 0.9);
            //double new_height = _height * StaticRandom.NextDouble(0.7, 0.9);
            right = new TreeViaShape(_x, _y - new_height,
                                     StaticRandom.NextDouble(_thickness * 0.6, _thickness * 0.9),
                                     new_height,
                                     depth + _height,
                                     angle + StaticRandom.NextDouble(10, 46) * rad2deg,
                                     polygon);
          }
        }
        else
        {
          _x += _height * Math.Sin(angle) + (brunch.Width / 2) * Math.Cos(angle);
          _y += _height * (1 - Math.Cos(angle)) + (brunch.Width / 2) * Math.Sin(Math.Abs(angle));
          center = new TreeViaShape(_x, _y, _thickness, _height, 0, angle, polygon, true);
          left = new TreeViaShape(_x - StaticRandom.NextDouble(_height / 5, _height / 2) * Math.Sin(angle),
                                  _y + StaticRandom.NextDouble(_height / 5, _height / 2) * Math.Cos(angle), 
                                  _thickness, _height, 0,
                                  angle - StaticRandom.NextDouble(60, 90) * rad2deg, polygon, true);
          right = new TreeViaShape(_x - StaticRandom.NextDouble(_height / 5, _height / 2) * Math.Sin(angle),
                                   _y + StaticRandom.NextDouble(_height / 5, _height / 2) * Math.Cos(angle), 
                                   _thickness, _height, 0,
                                   angle + StaticRandom.NextDouble(60, 90) * rad2deg, polygon, true);
        }
      }

      public TreeViaShape MakeATree(double _x, double _y, double _thickness, double _height, double depth, double angle = 0, bool polygon = false, bool leaves = false)
      {
        if (leaves)
        {
          Line leaf = new Line();
          leaf.Stroke = Brushes.Black;
          leaf.StrokeStartLineCap = PenLineCap.Round;
          leaf.StrokeEndLineCap = PenLineCap.Triangle;
          leaf.StrokeThickness = 2;
          leaf.X1 = _x;// + _thickness / 2 * Math.Cos(angle);
          leaf.Y1 = _y;// + _thickness / 2 * Math.Sin(angle);
          leaf.X2 = leaf.X1 + 5 * Math.Sin(angle);
          leaf.Y2 = leaf.Y1 - 5 * Math.Cos(angle);
          brunch = leaf;
          return this;
        }
        if (polygon)
        {
          Polygon pol = new Polygon();
          pol.Stroke = Brushes.Black;
          pol.Fill = Brushes.Black;
          pol.FillRule = FillRule.Nonzero;
          pol.Points.Add(new Point(_x, _y));
          pol.Points.Add(new Point(_x + (1 - thick_coef) * _thickness / 2, _y - _height));
          pol.Points.Add(new Point(_x + (1 + thick_coef) * _thickness / 2, _y - _height));
          pol.Points.Add(new Point(_x + _thickness, _y));
          if (angle > 0)
            pol.RenderTransform = new RotateTransform(angle / rad2deg,
                                                      _thickness, 0);
          else
            pol.RenderTransform = new RotateTransform(angle / rad2deg,
                                                      0, 0);
          brunch = pol;
        }
        else
        {
          brunch = new Rectangle();
          brunch.Height = _height;
          brunch.Width = _thickness;
          brunch.Stroke = Brushes.Black;
          brunch.Fill = Brushes.Black;
          Canvas.SetLeft(brunch, _x);
          Canvas.SetTop(brunch, _y);
          if (angle > 0)
            brunch.RenderTransform = new RotateTransform(angle / rad2deg,
                                                         _thickness,
                                                         _height);
          else
            brunch.RenderTransform = new RotateTransform(angle / rad2deg,
                                                         0, _height);
        }
        _thickness = _thickness * thick_coef;
        //_y -= _height;
        if (_height > 5 && depth < 350)
        {
          _x += _height * Math.Sin(angle) + ((brunch.Width - _thickness) / 2) * Math.Cos(angle);
          _y += _height * (1 - Math.Cos(angle)) - ((brunch.Width - _thickness) / 2) * Math.Sin(-angle);
          if (StaticRandom.NextDouble() < 0.9 || _thickness > 4)
          {
            center = new TreeViaShape();
            center.parent = this;
            center = center.MakeATree(_x, _y - _height * height_coef,
                                      _thickness,
                                      _height * height_coef,
                                      depth + _height,
                                      angle,
                                      polygon);
          }
          if (StaticRandom.NextDouble() < 0.7 && (angle > -110 * rad2deg || _height < 15))
          {
            double new_height = _height * StaticRandom.NextGaussian(0.8, 0.2, 0.3, 0.9);
            //double new_height = _height * StaticRandom.NextDouble(0.7, 0.9);
            left = new TreeViaShape();
            left.parent = this;
            left = left.MakeATree(_x, _y - new_height,
                                  StaticRandom.NextDouble(_thickness * 0.6, _thickness * 0.9),
                                  new_height,
                                  depth + _height,
                                  angle - StaticRandom.NextDouble(10, 46) * rad2deg,
                                  polygon);
          }
          if (StaticRandom.NextDouble() < 0.7 && (angle < 110 * rad2deg || _height < 15))
          {
            double new_height = _height * StaticRandom.NextGaussian(0.8, 0.2, 0.3, 0.9);
            //double new_height = _height * StaticRandom.NextDouble(0.7, 0.9);
            right = new TreeViaShape();
            right.parent = this;
            right = right.MakeATree(_x, _y - new_height,
                                    StaticRandom.NextDouble(_thickness * 0.6, _thickness * 0.9),
                                    new_height,
                                    depth + _height,
                                    angle + StaticRandom.NextDouble(10, 46) * rad2deg,
                                    polygon);
          }
        }
        else
        {
          _x += _height * Math.Sin(angle) + (brunch.Width / 2) * Math.Cos(angle);
          _y += _height * (1 - Math.Cos(angle)) + (brunch.Width / 2) * Math.Sin(Math.Abs(angle));
          center = new TreeViaShape();
          center.parent = this;
          center = center.MakeATree(_x, _y, _thickness, _height, 0, angle, polygon, true);
          left = new TreeViaShape();
          left.parent = this;
          left = left.MakeATree(_x - StaticRandom.NextDouble(_height / 5, _height / 2) * Math.Sin(angle),
                                _y + StaticRandom.NextDouble(_height / 5, _height / 2) * Math.Cos(angle),
                                _thickness, _height, 0,
                                angle - StaticRandom.NextDouble(60, 90) * rad2deg, polygon, true);
          right = new TreeViaShape();
          right.parent = this;
          right = right.MakeATree(_x - StaticRandom.NextDouble(_height / 5, _height / 2) * Math.Sin(angle),
                                  _y + StaticRandom.NextDouble(_height / 5, _height / 2) * Math.Cos(angle),
                                  _thickness, _height, 0,
                                  angle + StaticRandom.NextDouble(60, 90) * rad2deg, polygon, true);
        }
        return this;
      }
    }

    public class TreeViaPath
    {
      GeometryGroup tree;
      public GeometryGroup Tree
      { get { return tree; } set { tree = value; } }

      public TreeViaPath()
      {
        tree = new GeometryGroup();
        tree.FillRule = FillRule.Nonzero;
      }

      public void MakeATree(double _x, double _y, double _thickness, double _height, double depth, double angle = 0, bool polygon = false, bool leaves = false)
      {
        if (leaves)
        {
          EllipseGeometry leaf = new EllipseGeometry(new Point(_x, _y), leafHOR, leafVER);
          leaf.Transform = new RotateTransform(angle / rad2deg, _x, _y);
          tree.Children.Add(leaf);
          double new_x = _x - StaticRandom.NextDouble(_height / 5, _height / 2) * Math.Sin(angle);
          double new_y = _y + StaticRandom.NextDouble(_height / 5, _height / 2) * Math.Cos(angle);
          leaf = new EllipseGeometry(new Point(new_x, new_y), leafHOR, leafVER);
          leaf.Transform = new RotateTransform(angle / rad2deg + 90, new_x, new_y);
          tree.Children.Add(leaf);
          new_x = _x - StaticRandom.NextDouble(_height / 5, _height / 2) * Math.Sin(angle);
          new_y = _y + StaticRandom.NextDouble(_height / 5, _height / 2) * Math.Cos(angle);
          leaf = new EllipseGeometry(new Point(new_x, new_y), leafHOR, leafVER);
          leaf.Transform = new RotateTransform(angle / rad2deg - 90, new_x, new_y);
          tree.Children.Add(leaf);
          return;
        }
        if (polygon)
        {
          StreamGeometry a = new StreamGeometry();
          using (var b = a.Open())
          {
            b.BeginFigure(new Point(_x, _y + _height), true, true);
            b.LineTo(new Point(_x + (1 - thick_coef) * _thickness / 2, _y), true, true);
            b.LineTo(new Point(_x + (1 + thick_coef) * _thickness / 2, _y), true, true);
            b.LineTo(new Point(_x + _thickness, _y + _height), true, true);
            b.Close();
          }
          a.Transform = new RotateTransform(angle / rad2deg, _x, _y + _height);
          tree.Children.Add(a);
        }
        else
        {
          Rect rectangle = new Rect(new Point(_x, _y), new Size(_thickness, _height));
          RectangleGeometry brunch = new RectangleGeometry(rectangle);
          if (angle > 0)
            brunch.Transform = new RotateTransform(angle / rad2deg,
                                                   _x + _thickness,
                                                   _y + _height);
          else
            brunch.Transform = new RotateTransform(angle / rad2deg,
                                                   _x,
                                                   _y + _height);
          tree.Children.Add(brunch);
        }
        //_y -= _height;
        if (_height > 5 && depth < 350)
        {
          _x += _height * Math.Sin(angle) + (1 - thick_coef) * _thickness / 2 * Math.Cos(angle);
          _y += _height * (1 - Math.Cos(angle)) - (1 - thick_coef) * _thickness / 2 * Math.Sin(-angle);
          _thickness = _thickness * thick_coef;
          if (StaticRandom.NextDouble() < 0.9 || _thickness > 4)
          {
            double new_height = _height * StaticRandom.NextGaussian(0.85, 0.1, 0.7, 1);
            MakeATree(_x, _y - new_height,
                      _thickness,
                      new_height,
                      depth + _height,
                      angle, polygon);
          }
          if (StaticRandom.NextDouble() < 0.7 && (angle > -110 * rad2deg || _height < 15))
          {
            double new_height = _height * StaticRandom.NextGaussian(0.8, 0.2, 0.3, 0.9);
            //double new_height = _height * StaticRandom.NextDouble(0.7, 0.9);
            MakeATree(_x, _y - new_height,
                      StaticRandom.NextDouble(_thickness * 0.6, _thickness * 0.9),
                      new_height,
                      depth + _height,
                      angle - StaticRandom.NextDouble(10, 46) * rad2deg, polygon);
          }
          if (StaticRandom.NextDouble() < 0.7 && (angle < 110 * rad2deg || _height < 15))
          {
            double new_height = _height * StaticRandom.NextGaussian(0.8, 0.2, 0.3, 0.9);
            //double new_height = _height * StaticRandom.NextDouble(0.7, 0.9);
            MakeATree(_x, _y - new_height,
                      StaticRandom.NextDouble(_thickness * 0.6, _thickness * 0.9),
                      new_height,
                      depth + _height,
                      angle + StaticRandom.NextDouble(10, 46) * rad2deg, polygon);
          }
        }
        else
        {
          _x += _height * Math.Sin(angle) + thick_coef * _thickness / 2 * Math.Cos(angle);
          _y += _height * (1 - Math.Cos(angle)) + thick_coef * _thickness / 2 * Math.Sin(Math.Abs(angle));
          MakeATree(_x, _y, _thickness, _height, 0, angle, polygon, true);
        }
        return;
      }
    }
  }
}
