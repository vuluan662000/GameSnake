using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

namespace Snake;
class Point
{
    public int _x { get; set; }
    public int _y { get; set; }
    public Point(int x, int y)
    {
        this._x = x;
        this._y = y;
    }
}
class Program
{
    static int row = 20;
    static int col = 40;
    static int speed = 500;
    static char[,] gameBoard = new char[row, col];
    static string direction = "right";
    static Point _head = new Point(4, 5);
    static Point[] _body = new Point[2]
    {
        new Point(-1, -1),
        new Point(-1, -1),
    };
    static Point _food = new Point(-1, -1);
    static bool FoodExist = false;
    static int score = 0;
    static bool GameOver = false;
    static Stopwatch playTime = new Stopwatch();
    static string filePath = @"F:\Snake\Snake\Data\Save.txt";
    static string playerName = "";
    static void Main(string[] args)
    {
        Console.WriteLine("Enter Your Name:");
        playerName = Console.ReadLine();
        playTime.Start();
        Thread gameListenKey = new Thread(ListenKey);
        gameListenKey.Start();
        do
        {
            Console.Clear();
            CreateGameBoard();
            DrawSnake();
            MoveSnakeHead();
            DisplayGameBoard();
            CreateFood();
            Task.Delay(speed).Wait();

        } while (!GameOver);
        playTime.Stop();
        SaveGameInfor();
        ShowHighScore();
    }
    static void CreateGameBoard()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (i == 0 || i == row - 1 || j == 0 || j == col - 1)
                {
                    gameBoard[i, j] = '#';
                }
                else if (i == _food._x && j == _food._y)
                {
                    gameBoard[i, j] = '@';
                }
                else gameBoard[i, j] = ' ';
            }
        }
    }
    static void DrawSnake()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (i == _head._x && j == _head._y)
                {
                    gameBoard[i, j] = '*';
                }
                for (int b = 0; b < _body.Length; b++)
                {
                    if (_body[b]._x == i && _body[b]._y == j)
                    {
                        gameBoard[i, j] = '*';
                    }
                }
            }
        }
    }
    static void DisplayGameBoard()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (gameBoard[i, j] == '*' || gameBoard[i, j] == '#' || gameBoard[i, j] == '@')
                {
                    if (gameBoard[i, j] == '*')
                    {
                        if (i == _head._x && j == _head._y)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"{gameBoard[i, j]}");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write($"{gameBoard[i, j]}");
                            Console.ResetColor();
                        }

                    }
                    else if (gameBoard[i, j] == '@')
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"{gameBoard[i, j]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"{gameBoard[i, j]}");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.Write($"{gameBoard[i, j]}");
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine($"Score : {score}");
        Console.WriteLine($"Time : {playTime.Elapsed.Hours}h {playTime.Elapsed.Minutes}m {playTime.Elapsed.Seconds}s");
    }
    static void MoveSnakeHead()
    {
        CheckGameOver();
        Point oldHead = new Point(_head._x, _head._y);
        switch (direction)
        {
            case "right":
                _head._y += 1;
                if (_head._y == col - 1)
                {
                    _head._y = 1;
                }
                MoveBody(oldHead);
                break;
            case "left":
                _head._y -= 1;
                if (_head._y == 0)
                {
                    _head._y = col - 2;
                }
                MoveBody(oldHead);
                break;
            case "up":
                _head._x -= 1;
                if (_head._x == 0)
                {
                    _head._x = row - 2;
                }
                MoveBody(oldHead);
                break;
            case "down":
                _head._x += 1;
                if (_head._x == row - 1)
                {
                    _head._x = 1;
                }
                MoveBody(oldHead);
                break;
        }
        EatFood();
    }
    static void MoveBody(Point head)
    {
        for (int b = 0; b < _body.Length; b++)
        {
            Point node = new Point(_body[b]._x, _body[b]._y);
            _body[b]._x = head._x;
            _body[b]._y = head._y;
            head._x = node._x;
            head._y = node._y;
        }
    }
    static void ListenKey()
    {
        while (!GameOver)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            switch (keyInfo.Key)
            {
                case ConsoleKey.RightArrow:
                    if (direction != "left")
                    {
                        direction = "right";
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    if (direction != "right")
                    {
                        direction = "left";
                    }
                    break;
                case ConsoleKey.UpArrow:
                    if (direction != "down")
                    {
                        direction = "up";
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (direction != "up")
                    {
                        direction = "down";
                    }
                    break;
            }
        }
    }
    static void CreateFood()
    {
        if (FoodExist == false)
        {
            Random random = new Random();
            int foodX = random.Next(1, row - 1);
            int foodY = random.Next(1, col - 1);
            _food = new Point(foodX, foodY);
            FoodExist = true;
        }
    }
    static void EatFood()
    {
        if (_head._x == _food._x && _head._y == _food._y)
        {
            score++;
            FoodExist = false;
            Array.Resize(ref _body, _body.Length + 1);
            _body[_body.Length - 1] = new Point(-1, -1);
            speed = speed - 20;
        }
    }
    static void CheckGameOver()
    {
        for (int b = 0; b < _body.Length; b++)
        {
            if (_body[b]._x == _head._x && _body[b]._y == _head._y)
            {
                GameOver = true;
                break;
            }
        }
    }
    static void SaveGameInfor()
    {
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine($"{playerName},{score},{playTime.Elapsed.Hours}:{playTime.Elapsed.Minutes}:{playTime.Elapsed.Seconds}");
        }
    }
    static void ShowHighScore()
    {
        List<(string Name, int Score, string Time)> players = new List<(string Name, int Score, string Time)>();
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(',');
                string name = parts[0];
                int score = int.Parse(parts[1]);
                string time = parts[2];
                players.Add((name, score, time));
            }
        }
        for (int i = 0; i < players.Count - 1; i++)
        {
            for (int j = 0; j < players.Count - i - 1; j++)
            {
                if (players[j].Score < players[j + 1].Score)
                {
                    var temp = players[j];
                    players[j] = players[j + 1];
                    players[j + 1] = temp;
                }
            }
        }
        Console.WriteLine("High Scores:");
        foreach (var player in players)
        {
            Console.WriteLine($"{player.Name} - {player.Score} - {player.Time}");
        }
    }
}
