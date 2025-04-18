using System;
// Player Abstract Class
public abstract class Player
{
    public string Name { get; set; }
    public char Marker { get; set; }

    public Player(string name, char marker)
    {
        Name = name;
        Marker = marker;
    }

    public abstract int MakeMove();
}

// HumanPlayer Class
public class HumanPlayer : Player
{
    public HumanPlayer(string name, char marker) : base(name, marker) { }

    public override int MakeMove()
    {
        Console.WriteLine($"{Name}, enter the column (1-4) where you want to drop your disc:");
        return int.Parse(Console.ReadLine()) - 1; // Return the column index
    }
}

// ComputerPlayer Class (Basic Implementation for Now)
public class ComputerPlayer : Player
{
    public ComputerPlayer(string name, char marker) : base(name, marker) { }

    public override int MakeMove()
    {
        Random random = new Random();
        return random.Next(0, 7); // Random column for now
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Welcome message
        Console.WriteLine("Welcome to Connect 4!");

        // Create players
        Player player1 = new HumanPlayer("Player 1", 'X');
        Player player2 = new HumanPlayer("Player 2", 'O');

        // Initialize the game controller
        GameController controller = new GameController(player1, player2);

        // Start the game
        controller.PlayGame();

        // End message
        Console.WriteLine("Thank you for playing Connect 4!");
    }
}


// GameController with Encapsulation Applied
public class GameController
{
    private char[,] board;
    private Player player1;
    private Player player2;
    private Player currentPlayer;
    private const int Rows = 6;
    private const int Columns = 7;

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
                column = currentPlayer.MakeMove();
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

    private bool CheckWin(char marker)
    {
        // Horizontal, vertical, diagonal checks...
        return false; // For now, keeping logic the same
    }

    private bool IsBoardFull()
    {
        foreach (char slot in board)
        {
            if (slot == ' ') return false;
        }
        return true;
    }
}

// IMovable Interface ensures all players implement MakeMove()
public interface IMovable
{
    int MakeMove();
}

// Modify Player Class to Implement Interface
public abstract class Player : IMovable
{
    public string Name { get; private set; }
    public char Marker { get; private set; }

    public Player(string name, char marker)
    {
        Name = name;
        Marker = marker;
    }

    public abstract int MakeMove();
}