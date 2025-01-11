namespace Snake.Model;

public class Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Sum(Point src)
    {
        X += src.X;
        Y += src.Y;
    }
}