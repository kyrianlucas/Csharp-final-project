using System;

// IMovable Interface ensures all players implement MakeMove()
public interface IMovable
{
    int MakeMove(GameController controller);
}

// Abstract Player Class implementing IMovable
public abstract class Player : IMovable
{
    public string Name { get; private set; }
    public char Marker { get; private set; }

    public Player(string name, char marker)
    {
        Name = name;
        Marker = marker;
    }

    public abstract int MakeMove(GameController controller);
}

// HumanPlayer Class with Input Validation
public class HumanPlayer : Player
{
    public HumanPlayer(string name, char marker) : base(name, marker) { }

    public override int MakeMove(GameController controller)
    {
        int column = -1;
        bool validInput = false;

        while (!validInput)
        {
            Console.WriteLine($"{Name}, enter the column (1-7) where you want to drop your disc:");
            string input = Console.ReadLine();

            if (int.TryParse(input, out column) && column >= 1 && column <= 7)
            {
                validInput = true;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 1 and 7.");
            }
        }

        return column - 1; // Convert to zero-based index
    }
}

// ComputerPlayer Class with Minimax AI
public class ComputerPlayer : Player
{
    public ComputerPlayer(string name, char marker) : base(name, marker) { }

    public override int MakeMove(GameController controller)
    {
        Dictionary<int, int> moveScores = new Dictionary<int, int>();

        for (int col = 0; col < 7; col++)
        {
            if (controller.IsColumnAvailable(col))
            {
                controller.PlaceDisc(Marker, col);
                int score = Minimax(controller, 3, false, Marker);
                controller.UndoMove(col);

                moveScores[col] = score;
            }
        }

        int bestScore = moveScores.Values.Max();
        List<int> bestMoves = moveScores.Where(m => m.Value == bestScore).Select(m => m.Key).ToList();

        Random random = new Random();
        int finalMove = bestMoves[random.Next(bestMoves.Count)];

        Console.WriteLine($"{Name} (Computer) chooses column {finalMove + 1} using Minimax.");
        return finalMove;
    }

    private int Minimax(GameController controller, int depth, bool isMaximizing, char marker)
    {
        if (controller.CheckWin('X')) return -1000;
        if (controller.CheckWin('O')) return 1000;
        if (depth == 0 || controller.IsBoardFull()) return 0;

        int bestScore = isMaximizing ? int.MinValue : int.MaxValue;

        for (int col = 0; col < 7; col++)
        {
            if (controller.IsColumnAvailable(col))
            {
                controller.PlaceDisc(marker, col);
                int score = Minimax(controller, depth - 1, !isMaximizing, marker == 'X' ? 'O' : 'X');
                controller.UndoMove(col);

                if (isMaximizing) bestScore = Math.Max(bestScore, score);
                else bestScore = Math.Min(bestScore, score);
            }
        }

        return bestScore;
    }
}

// GameController Class with Mode Selection
public class GameController
{
    private char[,] board;
    private Player player1;
    private Player player2;
    private Player currentPlayer;
    private const int Rows = 6;
    private const int Columns = 7;

    public char[,] Board => board;

    public GameController(Player p1, Player p2)
    {
        player1 = p1;
        player2 = p2;

        Random random = new Random();
        currentPlayer = random.Next(2) == 0 ? player1 : player2; // Random first move in 1-player mode

        board = new char[Rows, Columns];
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Columns; j++)
                board[i, j] = ' ';
    }

    public bool IsBoardFull()
    {
        foreach (char slot in board)
        {
            if (slot == ' ') return false;
        }
        return true;
    }

    public void PlayGame()
    {
        bool gameWon = false;

        while (!gameWon)
        {
            DisplayBoard();
            Console.WriteLine($"It is {currentPlayer.Name}'s turn ({currentPlayer.Marker}):");

            int column;
            bool validMove;
            do
            {
                column = currentPlayer.MakeMove(this);
                validMove = PlaceDisc(currentPlayer.Marker, column);

                if (!validMove)
                    Console.WriteLine("Column full. Choose a different column.");
            } while (!validMove);

            if (CheckWin(currentPlayer.Marker))
            {
                DisplayBoard();
                Console.WriteLine($"🎉 {currentPlayer.Name} wins! 🎉");
                gameWon = true;
            }
            else if (IsBoardFull())
            {
                DisplayBoard();
                Console.WriteLine("It's a draw!");
                break;
            }
            else
            {
                SwitchPlayer();
            }
        }

        Console.WriteLine("\nGame over. Returning to start screen...\n");
        StartGame();
    }

    private void SwitchPlayer()
    {
        currentPlayer = currentPlayer == player1 ? player2 : player1;
    }

    private void DisplayBoard()
    {
        Console.Clear();
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Console.Write($"| {board[i, j]} ");
            }
            Console.WriteLine("|");
        }
        Console.WriteLine(new string('-', Columns * 4));
        Console.WriteLine("  1   2   3   4   5   6   7");
    }

    public bool IsColumnAvailable(int column)
    {
        return board[0, column] == ' ';
    }

    public bool PlaceDisc(char marker, int column)
    {
        if (column < 0 || column >= Columns) return false;

        for (int i = Rows - 1; i >= 0; i--)
        {
            if (board[i, column] == ' ')
            {
                board[i, column] = marker;
                return true;
            }
        }
        return false;
    }

    public void UndoMove(int column)
    {
        for (int i = 0; i < Rows; i++)
        {
            if (board[i, column] != ' ')
            {
                board[i, column] = ' ';
                return;
            }
        }
    }

    public bool CheckWin(char marker)
    {
        // Check horizontal wins
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j <= Columns - 4; j++)
            {
                if (board[i, j] == marker &&
                    board[i, j + 1] == marker &&
                    board[i, j + 2] == marker &&
                    board[i, j + 3] == marker)
                {
                    return true;
                }
            }
        }

        // Check vertical wins
        for (int j = 0; j < Columns; j++)
        {
            for (int i = 0; i <= Rows - 4; i++)
            {
                if (board[i, j] == marker &&
                    board[i + 1, j] == marker &&
                    board[i + 2, j] == marker &&
                    board[i + 3, j] == marker)
                {
                    return true;
                }
            }
        }

        // Check diagonal wins (bottom-left to top-right)
        for (int i = 3; i < Rows; i++) // Start from row 3 to avoid out-of-bounds
        {
            for (int j = 0; j <= Columns - 4; j++)
            {
                if (board[i, j] == marker &&
                    board[i - 1, j + 1] == marker &&
                    board[i - 2, j + 2] == marker &&
                    board[i - 3, j + 3] == marker)
                {
                    return true;
                }
            }
        }

        // Check diagonal wins (top-left to bottom-right)
        for (int i = 0; i <= Rows - 4; i++) // Start from top for correct diagonal check
        {
            for (int j = 0; j <= Columns - 4; j++)
            {
                if (board[i, j] == marker &&
                    board[i + 1, j + 1] == marker &&
                    board[i + 2, j + 2] == marker &&
                    board[i + 3, j + 3] == marker)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static void StartGame()
    {
        Console.WriteLine("Select Game Mode:");
        Console.WriteLine("1: Two-Player Mode");
        Console.WriteLine("2: One-Player Mode (Play Against Computer)");

        int mode;
        do
        {
            Console.Write("Enter your choice (1 or 2): ");
            string input = Console.ReadLine();
            int.TryParse(input, out mode);
        } while (mode != 1 && mode != 2);

        Player player1 = new HumanPlayer("Player 1", 'X');
        Player player2 = mode == 2 ? new ComputerPlayer("Computer", 'O') : new HumanPlayer("Player 2", 'O');

        GameController controller = new GameController(player1, player2);
        controller.PlayGame();
    }
}

// Program Class with Mode Selection
class Program
{
    static void Main(string[] args)
    {
        GameController.StartGame();
    }
}
