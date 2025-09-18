using System;
using System.Collections.Generic;

namespace mazeCli;

public class MazeGenerator
{
    public enum CellType
    {
        Wall,
        Clear,
        Start,
        End
    }

    private Random random = new Random();
    private int width;
    private int height;
    private CellType[,] map;

    public MazeGenerator(int width, int height)
    {
        // Делаем размеры нечетными для корректной работы алгоритма
        this.width = width % 2 == 1 ? width : width + 1;
        this.height = height % 2 == 1 ? height : height + 1;
        this.map = new CellType[this.width, this.height];
    }

    public CellType[,] GenerateMaze()
    {
        // Шаг 1: Создаем идеальный лабиринт алгоритмом Прима
        GeneratePerfectMaze();

        // Шаг 2: Добавляем вход и выход на границах с гарантией доступности
        AddStartAndEndOnBorders();

        // Шаг 3: Минимальное удаление тупиков для лучшего заполнения
        RemoveSomeDeadEnds();

        // Шаг 4: Гарантируем доступность входа и выхода
        EnsureAccessibility();

        return map;
    }

    private void GeneratePerfectMaze()
    {
        // Создаем массив и заполняем все ячейки стенами
        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                map[w, h] = CellType.Wall;
            }
        }

        // Выбираем случайную ячейку с нечетными координатами и очищаем её
        int x = (random.Next(0, width / 2)) * 2 + 1;
        int y = (random.Next(0, height / 2)) * 2 + 1;
        map[x, y] = CellType.Clear;

        // Создаем список ячеек для проверки
        List<Point> toCheck = new List<Point>();

        // Добавляем валидные ячейки, которые находятся на расстоянии 2 по ортогонали
        if (y - 2 >= 1)
        {
            toCheck.Add(new Point(x, y - 2));
        }

        if (y + 2 < height - 1)
        {
            toCheck.Add(new Point(x, y + 2));
        }

        if (x - 2 >= 1)
        {
            toCheck.Add(new Point(x - 2, y));
        }

        if (x + 2 < width - 1)
        {
            toCheck.Add(new Point(x + 2, y));
        }

        // Пока есть ячейки для проверки
        while (toCheck.Count > 0)
        {
            // Выбираем случайную ячейку
            int index = random.Next(toCheck.Count);
            Point cell = toCheck[index];
            x = cell.X;
            y = cell.Y;

            // Если ячейка еще стена
            if (map[x, y] == CellType.Wall)
            {
                map[x, y] = CellType.Clear;

                // Соединяем с уже очищенной ячейкой
                ConnectToExistingClearCell(x, y);

                // Добавляем новые ячейки для проверки
                if (y - 2 >= 1 && map[x, y - 2] == CellType.Wall)
                {
                    toCheck.Add(new Point(x, y - 2));
                }

                if (y + 2 < height - 1 && map[x, y + 2] == CellType.Wall)
                {
                    toCheck.Add(new Point(x, y + 2));
                }

                if (x - 2 >= 1 && map[x - 2, y] == CellType.Wall)
                {
                    toCheck.Add(new Point(x - 2, y));
                }

                if (x + 2 < width - 1 && map[x + 2, y] == CellType.Wall)
                {
                    toCheck.Add(new Point(x + 2, y));
                }
            }

            toCheck.RemoveAt(index);
        }
    }

    private void ConnectToExistingClearCell(int x, int y)
    {
        // Направления: Север, Юг, Восток, Запад
        int[] dx = { 0, 0, -2, 2 };
        int[] dy = { -2, 2, 0, 0 };

        // Создаем список направлений и перемешиваем его
        List<int> directions = new List<int> { 0, 1, 2, 3 };
        for (int i = directions.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            int temp = directions[i];
            directions[i] = directions[j];
            directions[j] = temp;
        }

        foreach (int dir in directions)
        {
            int newX = x + dx[dir];
            int newY = y + dy[dir];

            if (newX >= 1 && newX < width - 1 && newY >= 1 && newY < height - 1)
            {
                if (map[newX, newY] == CellType.Clear)
                {
                    // Очищаем ячейку между текущей и найденной
                    int wallX = x + dx[dir] / 2;
                    int wallY = y + dy[dir] / 2;
                    map[wallX, wallY] = CellType.Clear;
                    return;
                }
            }
        }
    }

    private void AddStartAndEndOnBorders()
    {
        // Ищем подходящие места для входа и выхода на границах
        List<Point> possibleStarts = new List<Point>();
        List<Point> possibleEnds = new List<Point>();

        // Проверяем верхнюю границу (y = 0)
        for (int x = 1; x < width - 1; x += 2)
        {
            if (map[x, 1] == CellType.Clear)
            {
                possibleStarts.Add(new Point(x, 0));
            }
        }

        // Проверяем нижнюю границу (y = height - 1)
        for (int x = 1; x < width - 1; x += 2)
        {
            if (map[x, height - 2] == CellType.Clear)
            {
                possibleEnds.Add(new Point(x, height - 1));
            }
        }

        // Проверяем левую границу (x = 0)
        for (int y = 1; y < height - 1; y += 2)
        {
            if (map[1, y] == CellType.Clear)
            {
                possibleStarts.Add(new Point(0, y));
            }
        }

        // Проверяем правую границу (x = width - 1)
        for (int y = 1; y < height - 1; y += 2)
        {
            if (map[width - 2, y] == CellType.Clear)
            {
                possibleEnds.Add(new Point(width - 1, y));
            }
        }

        // Если нашли подходящие места
        if (possibleStarts.Count > 0 && possibleEnds.Count > 0)
        {
            // Выбираем случайные точки
            Point start = possibleStarts[random.Next(possibleStarts.Count)];
            Point end = possibleEnds[random.Next(possibleEnds.Count)];

            // Убеждаемся, что они разные
            if (start.X == end.X && start.Y == end.Y && possibleStarts.Count > 1)
            {
                // Выбираем другие точки если совпали
                do
                {
                    start = possibleStarts[random.Next(possibleStarts.Count)];
                } while (start.X == end.X && start.Y == end.Y && possibleStarts.Count > 1);
            }

            map[start.X, start.Y] = CellType.Start;
            map[end.X, end.Y] = CellType.End;
        }
        else
        {
            // Если не нашли - делаем стандартные на противоположных сторонах
            map[1, 0] = CellType.Start;  // Вход сверху
            map[width - 2, height - 1] = CellType.End;  // Выход снизу
        }
    }

    private void RemoveSomeDeadEnds()
    {
        // Только одна итерация удаления для сохранения плотности
        List<Point> deadEnds = new List<Point>();

        // Находим тупики
        for (int h = 1; h < height - 1; h++)
        {
            for (int w = 1; w < width - 1; w++)
            {
                if (map[w, h] == CellType.Clear)
                {
                    int neighbors = CountClearNeighbors(w, h);
                    if (neighbors == 1)  // Только один выход - тупик
                    {
                        deadEnds.Add(new Point(w, h));
                    }
                }
            }
        }

        // Удаляем половину тупиков для лучшего заполнения
        int toRemove = Math.Min(deadEnds.Count / 2, deadEnds.Count);
        for (int i = 0; i < toRemove; i++)
        {
            Point cell = deadEnds[i];
            map[cell.X, cell.Y] = CellType.Wall;
        }
    }

    private void EnsureAccessibility()
    {
        // Проверяем и обеспечиваем доступность входа и выхода
        Point start = GetStartPoint();
        Point end = GetEndPoint();

        if (start != null)
        {
            EnsurePointAccessibility(start);
        }

        if (end != null)
        {
            EnsurePointAccessibility(end);
        }
    }

    private void EnsurePointAccessibility(Point point)
    {
        // Проверяем, есть ли доступные соседи
        int accessibleNeighbors = CountAccessibleNeighbors(point.X, point.Y);

        // Если нет доступных соседей, создаем проход
        if (accessibleNeighbors == 0)
        {
            CreateAccessForPoint(point);
        }
    }

    private void CreateAccessForPoint(Point point)
    {
        // Определяем направление от границы внутрь
        if (point.X == 0)  // Левая граница
        {
            if (point.Y > 0 && point.Y < height - 1)
            {
                map[1, point.Y] = CellType.Clear;
            }
        }
        else if (point.X == width - 1)  // Правая граница
        {
            if (point.Y > 0 && point.Y < height - 1)
            {
                map[width - 2, point.Y] = CellType.Clear;
            }
        }
        else if (point.Y == 0)  // Верхняя граница
        {
            if (point.X > 0 && point.X < width - 1)
            {
                map[point.X, 1] = CellType.Clear;
            }
        }
        else if (point.Y == height - 1)  // Нижняя граница
        {
            if (point.X > 0 && point.X < width - 1)
            {
                map[point.X, height - 2] = CellType.Clear;
            }
        }
    }

    private int CountAccessibleNeighbors(int x, int y)
    {
        int neighbors = 0;
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { -1, 1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int newX = x + dx[i];
            int newY = y + dy[i];

            if (newX >= 0 && newX < width && newY >= 0 && newY < height)
            {
                if (map[newX, newY] == CellType.Clear)
                {
                    neighbors++;
                }
            }
        }

        return neighbors;
    }

    private int CountClearNeighbors(int x, int y)
    {
        int neighbors = 0;
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { -1, 1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int newX = x + dx[i];
            int newY = y + dy[i];

            if (newX >= 0 && newX < width && newY >= 0 && newY < height)
            {
                if (map[newX, newY] == CellType.Clear)
                {
                    neighbors++;
                }
            }
        }

        return neighbors;
    }

    // Метод для отображения карты
    public void PrintMap(Point playerPosition = null)
    {
        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                // Проверяем, находится ли игрок в этой позиции
                if (playerPosition != null && playerPosition.X == w && playerPosition.Y == h)
                {
                    Console.Write("P ");
                }
                else
                {
                    switch (map[w, h])
                    {
                        case CellType.Clear:
                            Console.Write(". ");
                            break;
                        case CellType.Wall:
                            Console.Write("# ");
                            break;
                        case CellType.Start:
                            Console.Write("S ");
                            break;
                        case CellType.End:
                            Console.Write("E ");
                            break;
                    }
                }
            }
            Console.WriteLine();
        }
    }

    // Получить координаты начала и конца
    public Point GetStartPoint()
    {
        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                if (map[w, h] == CellType.Start)
                {
                    return new Point(w, h);
                }
            }
        }
        return null;
    }

    public Point GetEndPoint()
    {
        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                if (map[w, h] == CellType.End)
                {
                    return new Point(w, h);
                }
            }
        }
        return null;
    }

    // Получить карту в виде двумерного массива для дальнейшей обработки
    public CellType[,] GetMap()
    {
        return map;
    }

    // Получить размеры карты
    public int GetWidth() => width;
    public int GetHeight() => height;

    // Проверить, можно ли двигаться в указанную позицию
    public bool CanMoveTo(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return false;
        }

        return map[x, y] != CellType.Wall;
    }

    // Проверить, достиг ли игрок выхода
    public bool IsAtExit(Point playerPosition)
    {
        Point endPoint = GetEndPoint();
        if (endPoint == null || playerPosition == null)
        {
            return false;
        }

        return playerPosition.X == endPoint.X && playerPosition.Y == endPoint.Y;
    }
}
