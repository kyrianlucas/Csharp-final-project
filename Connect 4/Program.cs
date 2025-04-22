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

// ComputerPlayer Class with Basic AI Logic
public class ComputerPlayer : Player
{
    public ComputerPlayer(string name, char marker) : base(name, marker) { }

    public override int MakeMove(GameController controller)
    {
        Random random = new Random();

        // Try to find a winning move
        int winningMove = FindWinningMove(Marker, controller);
        if (winningMove != -1)
        {
            Console.WriteLine($"{Name} (Computer) plays in column {winningMove + 1} to WIN.");
            return winningMove;
        }

        // Try to block opponent's winning move
        char opponentMarker = (Marker == 'X') ? 'O' : 'X';
        int blockingMove = FindWinningMove(opponentMarker, controller);
        if (blockingMove != -1)
        {
            Console.WriteLine($"{Name} (Computer) plays in column {blockingMove + 1} to BLOCK.");
            return blockingMove;
        }

        // Otherwise, play in the center column (strategic positioning)
        int preferredColumn = 3;
        if (IsColumnAvailable(preferredColumn, controller))
        {
            Console.WriteLine($"{Name} (Computer) plays in center column {preferredColumn + 1}.");
            return preferredColumn;
        }

        // If center isn't available, choose a random valid column
        int randomMove;
        do
        {
            randomMove = random.Next(0, 7);
        } while (!IsColumnAvailable(randomMove, controller));

        Console.WriteLine($"{Name} (Computer) plays randomly in column {randomMove + 1}.");
        return randomMove;
    }

    private int FindWinningMove(char marker, GameController controller)
    {
        for (int col = 0; col < 7; col++)
        {
            if (IsColumnAvailable(col, controller))
            {
                for (int row = 5; row >= 0; row--)
                {
                    if (controller.Board[row, col] == ' ')
                    {
                        controller.Board[row, col] = marker; // Simulate move

                        bool isWinningMove = controller.CheckWin(marker);

                        controller.Board[row, col] = ' '; // Undo simulated move

                        if (isWinningMove)
                        {
                            return col;
                        }

                        break;
                    }
                }
            }
        }
        return -1; // No winning move found
    }

    private bool IsColumnAvailable(int column, GameController controller)
    {
        return controller.Board[0, column] == ' '; // If top row is empty, the column is available
    }
}

// GameController Class with Fixed Win Detection
public class GameController
{
    private char[,] board;
    private Player player1;
    private Player player2;
    private Player currentPlayer;
    private const int Rows = 6;
    private const int Columns = 7;

    // Public property to allow AI to check the board state
    public char[,] Board => board;

    public GameController(Player p1, Player p2)
    {
        player1 = p1;
        player2 = p2;
        currentPlayer = player1;
        board = new char[Rows, Columns];

        // Initialize the board with empty spaces
        for (int i = 0; i < Rows; i++)
            for (int j = 0; j < Columns; j++)
                board[i, j] = ' ';
    }

    private bool IsBoardFull()
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
            Console.WriteLine($"{currentPlayer.Name}'s turn.");

            int column;
            bool validMove;
            do
            {
                column = currentPlayer.MakeMove(this);
                validMove = PlaceDisc(currentPlayer.Marker, column);

                if (!validMove)
                    Console.WriteLine("Invalid move. Try again.");
            } while (!validMove);

            if (CheckWin(currentPlayer.Marker))
            {
                DisplayBoard();
                Console.WriteLine($"{currentPlayer.Name} wins!");
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

    private bool PlaceDisc(char marker, int column)
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
        for (int i = 3; i < Rows; i++)
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
        for (int i = 0; i <= Rows - 4; i++)
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

        return false; // No win detected
    }
}

// Program Class with Main Method
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Connect 4!");

        Player player1 = new HumanPlayer("Player 1", 'X');
        Player player2 = new ComputerPlayer("Computer", 'O');

        GameController controller = new GameController(player1, player2);
        controller.PlayGame();

        Console.WriteLine("Thank you for playing Connect 4!");
    }
}