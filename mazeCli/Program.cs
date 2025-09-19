namespace mazeCli;

public class Program
{
    public static void Main()
    {
        // Создаем и запускаем игру
        Game game = new Game(21, 21);
        game.Start();
    }
}
