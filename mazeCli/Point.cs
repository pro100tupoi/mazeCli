namespace mazeCli;

public class Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Point(Point other)
    {
        X = other.X;
        Y = other.Y;
    }

    public bool Equals(Point other)
    {
        if (other == null)
        {
            return false;
        }
        return X == other.X && Y == other.Y;
    }
}
