namespace mazeCli;

public class Player
{
    public Point Position { get; private set; }
    private MazeGenerator maze;

    public Player(MazeGenerator maze)
    {
        this.maze = maze;
        Point startPoint = maze.GetStartPoint();
        if (startPoint != null)
        {
            Position = new Point(startPoint.X, startPoint.Y);
        }
        else
        {
            Position = new Point(1, 1); // Позиция по умолчанию
        }
    }

    public bool Move(char direction)
    {
        int newX = Position.X;
        int newY = Position.Y;

        switch (direction)
        {
            case 'w':
            case 'W':
            case 'ц':
            case 'Ц':
                newY--;
                break;
            case 's':
            case 'S':
            case 'ы':
            case 'Ы':
                newY++;
                break;
            case 'a':
            case 'A':
            case 'ф':
            case 'Ф':
                newX--;
                break;
            case 'd':
            case 'D':
            case 'в':
            case 'В':
                newX++;
                break;
            default:
                return false;
        }

        // Проверяем, можно ли двигаться в новую позицию
        if (maze.CanMoveTo(newX, newY))
        {
            Position.X = newX;
            Position.Y = newY;
            return true;
        }

        return false;
    }

    public Point GetPosition()
    {
        return new Point(Position);
    }
}
