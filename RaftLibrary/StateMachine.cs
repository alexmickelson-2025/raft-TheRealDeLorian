using System.Security.Cryptography.X509Certificates;

public class StateMachine
{
  public int X { get; set; }
  public int Y { get; set; }
  public int Z { get; set; }
  public StateMachine(int x = 0, int y = 0, int z = 0)
  {
    X = x;
    Y = y;
    Z = z;
  }

  public string GetState()
  {
    return $"X={X}, Y={Y}, Z={Z}";
  }

  public void AddToX(int operand)
  {
    X += operand;
  }
  public void AddToY(int operand)
  {
    Y += operand;
  }

  public void AddToZ(int operand)
  {
    Z += operand;
  }
}