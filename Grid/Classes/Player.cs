using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Schedule.Classes
{
  public enum Position { GK, LB, CB, RB, DM, LM, CM, RM, AM, CF };

  public class Player
  {
    private int id;
    private string name;
    private HashSet<Position> positions;
    private double defence;
    private double finishing;
    private double shortpassing;
    private double longpassing;
    private double dribbling;
    private double pace;
    private double strength;
    private double heading;
    private double goalkeeping;
    private double setpieces;
    public int Injured { get; set; }
    public double DEF { get { return defence; } }
    public double FIN { get { return finishing; } }
    public double SPS { get { return shortpassing; } }
    public double LPS { get { return longpassing; } }
    public double DRB { get { return dribbling; } }
    public double PAC { get { return pace; } }
    public double STR { get { return strength; } }
    public double HEA { get { return heading; } }
    public double GK  { get { return goalkeeping; } }
    public double SP  { get { return setpieces; } }
    public string Name { get {return name;}}
    public double BestOVR { get { return positions.Max(x => OVRByPosition(x)); } }
    public HashSet<Position> Positions { get { return positions; } }
    public int ID { get { return id; } }
    
    /*private double GKAbility  { get { return (4 * goalkeeping + heading + shortpassing + longpassing) / 6.5; } }
    private double CBAbility  { get { return (3 * defence + longpassing + strength + 2 * heading) / 7.0; } }
    private double SBAbility  { get { return (2 * defence + shortpassing + 2* longpassing + 2 * pace) / 7.0; } }
    private double DMAbility  { get { return (3 * defence + shortpassing + 2 * strength + heading) / 7.0; } }
    private double CMAbility  { get { return (defence + 2 * shortpassing + longpassing + dribling + pace + strength) / 7.0; } }
    private double CMDAbility { get { return (2 * defence + shortpassing + longpassing + 2 * strength) / 6.0; } }
    private double CMAAbility { get { return (defence + finishing + 2 * shortpassing + longpassing + dribbling) / 6.0; } }
    private double AMAbility  { get { return (2 * finishing + 2 * shortpassing + longpassing + 2 * dribbling) / 7.0; } }
    private double SMAbility  { get { return (finishing + shortpassing + 2 * longpassing + 2 * dribbling + 2 * pace) / 8.0; } }
    private double CFAbility  { get { return (3 * finishing + shortpassing + dribbling + 2 * Math.Max(pace, (strength+heading)/2)) / 7.0; } }*/
    private double GKAbility  { get { return 0.75 * goalkeeping +
                                             0.05 * heading +
                                             0.1 * longpassing +
                                             0.1 * shortpassing; } }
    private double CBAbility  { get { return 0.56 * defence +
                                             0.08 * shortpassing +
                                             0.05 * longpassing +
                                             0.16 * strength +
                                             0.15 * heading; } }
    private double SBAbility  { get { return Math.Max(0.48 * defence +
                                                      0.1 * shortpassing +
                                                      0.12 * longpassing +
                                                      0.15 * pace +
                                                      0.07 * heading +
                                                      0.08 * dribbling,

                                                      0.42 * defence +
                                                      0.14 * shortpassing +
                                                      0.16 * longpassing +
                                                      0.14 * pace +
                                                      0.14 * dribbling); } }
    private double DMAbility  { get { return 0.47 * defence +
                                             0.21 * shortpassing +
                                             0.16 * longpassing +
                                             0.16 * strength; } }
    private double CMAbility  { get { return 0.17 * defence +
                                             0.25 * shortpassing +
                                             0.22 * longpassing +
                                             0.17 * dribbling +
                                             0.19 * finishing; } }
    private double CMDAbility { get { return 0.32 * defence +
                                             0.23 * shortpassing +
                                             0.19 * longpassing +
                                             0.10 * strength +
                                             0.08 * dribbling +
                                             0.08 * finishing; } }
    private double CMAAbility { get { return 0.16 * defence +
                                             0.22 * shortpassing +
                                             0.14 * longpassing +
                                             0.19 * dribbling +
                                             0.17 * finishing +
                                             0.07 * pace +
                                             0.05 * strength; } }
    private double AMAbility  { get { return 0.22 * shortpassing +
                                             0.09 * longpassing +
                                             0.29 * dribbling +
                                             0.27 * finishing +
                                             0.13 * pace; } }
    private double SMAbility  { get { return Math.Max(0.28 * dribbling +
                                                      0.14 * shortpassing +
                                                      0.22 * longpassing +
                                                      0.18 * pace +
                                                      0.18 * finishing,

                                                      0.30 * dribbling +
                                                      0.12 * shortpassing +
                                                      0.12 * longpassing +
                                                      0.20 * pace +
                                                      0.26 * finishing); } }
    private double CFAbility  { get { return Math.Max(0.30 * dribbling +
                                                      0.12 * shortpassing +
                                                      0.20 * pace +
                                                      0.38 * finishing,

                                                      0.13 * dribbling +
                                                      0.08 * shortpassing +
                                                      0.13 * strength +
                                                      0.19 * heading +
                                                      0.47 * finishing); } }


    public Player(int _id, string _name, string attributes)
    {
      id = _id;
      name = _name;
      string[] att = attributes.Split(" ".ToArray(), StringSplitOptions.RemoveEmptyEntries);
      defence      = Extensions.AttributesDistribution(double.Parse(att[0], CultureInfo.InvariantCulture));
      finishing    = Extensions.AttributesDistribution(double.Parse(att[1], CultureInfo.InvariantCulture));
      shortpassing = Extensions.AttributesDistribution(double.Parse(att[2], CultureInfo.InvariantCulture));
      longpassing  = Extensions.AttributesDistribution(double.Parse(att[3], CultureInfo.InvariantCulture));
      dribbling    = Extensions.AttributesDistribution(double.Parse(att[4], CultureInfo.InvariantCulture));
      pace         = Extensions.AttributesDistribution(double.Parse(att[5], CultureInfo.InvariantCulture));
      strength     = Extensions.AttributesDistribution(double.Parse(att[6], CultureInfo.InvariantCulture));
      heading      = Extensions.AttributesDistribution(double.Parse(att[7], CultureInfo.InvariantCulture));
      goalkeeping  = Extensions.AttributesDistribution(double.Parse(att[8], CultureInfo.InvariantCulture));
      setpieces    = Extensions.AttributesDistribution(double.Parse(att[9], CultureInfo.InvariantCulture));

      positions = new HashSet<Position>();
      Injured = 0;
      foreach (var pos in att.Skip(10))
        positions.Add(pos.String2Position());
    }
    public double OVRByPosition(Position pos)
    {
      double OVR = 0;
      switch (pos)
      {
        case Position.GK:
          OVR = GKAbility; break;
        case Position.LB:
        case Position.RB:
          OVR = SBAbility; break;
        case Position.CB:
          OVR = CBAbility; break;
        case Position.LM:
        case Position.RM:
          OVR = SMAbility; break;
        case Position.DM:
          OVR = DMAbility; break;
        case Position.CM:
          OVR = Extensions.Max(CMDAbility, CMAbility, CMAAbility); break;
        case Position.AM:
          OVR = AMAbility; break;
        case Position.CF:
          OVR = CFAbility; break;
        default:
          return -1;
      }
      if (!positions.Contains(pos))
        return OVR / 1.5;
      else
        return OVR;
    }

    public static bool operator !=(Player p1, Player p2)
    {
      return !(p1 == p2);
    }
    public static bool operator ==(Player p1, Player p2)
    {
      if (System.Object.ReferenceEquals(p1, p2))
        return true;
      if ((object)p1 == null || (object)p2 == null)
        return false;
      return p1.id == p2.id;
    }
    public override bool Equals(object o)
    {
      if (o == null)
        return false;
      Player p = o as Player;
      return (object)p != null && p.id == this.id;
    }
    public override int GetHashCode()
    {
      int hash = 9769;
      hash = hash * 8641 ^ id.GetHashCode();
      return hash;
    }
  }
}