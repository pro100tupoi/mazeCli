namespace mazeCli;

public class Game
{
    private MazeGenerator maze;
    private Player player;
    private bool gameRunning;

    public Game(int width, int height)
    {
        maze = new MazeGenerator(width, height);
        maze.GenerateMaze();
        player = new Player(maze);
        gameRunning = false;
    }

    public void Start()
    {
        ShowWelcomeMessage();
        WaitForInput();
        StartGameLoop();
    }

    private void ShowWelcomeMessage()
    {
        Console.WriteLine("Добро пожаловать в лабиринт!");
        Console.WriteLine("Управление: W - вверх, S - вниз, A - влево, D - вправо");
        Console.WriteLine("Найдите выход (E) чтобы победить!");
        Console.WriteLine("Нажмите любую клавишу для начала...");
    }

    private void WaitForInput()
    {
        Console.ReadKey();
        Console.Clear();
    }

    private void StartGameLoop()
    {
        gameRunning = true;
        while (gameRunning)
        {
            Render();
            HandleInput();
            CheckWinCondition();
        }
    }

    private void Render()
    {
        Console.Clear();
        maze.PrintMap(player.GetPosition());
    }

    private void HandleInput()
    {
        Console.WriteLine("\nВведите направление (W/A/S/D) или Q для выхода:");

        ConsoleKeyInfo keyInfo = Console.ReadKey();
        var key = keyInfo.KeyChar;

        if (key == 'q' || key == 'Q')
        {
            gameRunning = false;
            Console.WriteLine("\nСпасибо за игру!");
        }
        else
        {
            var moved = player.Move(key);
            if (!moved)
            {
                Console.WriteLine("\nНевозможно двигаться в этом направлении!");
                Console.ReadKey();
            }
        }
    }

    private void CheckWinCondition()
    {
        if (maze.IsAtExit(player.GetPosition()))
        {
            Render();
            Console.WriteLine("\nПоздравляем! Вы нашли выход и победили!");
            Console.WriteLine("Спасибо за игру!");
            gameRunning = false;
        }
    }
}
