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


// GameController Class
public class GameController
{
    private char[,] board;
    private Player player1;
    private Player player2;
    private int rows = 4;
    private int columns = 4;

    public GameController(Player p1, Player p2)
    {
        player1 = p1;
        player2 = p2;
        board = new char[rows, columns];

        // Initialize the board with empty spaces
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                board[i, j] = ' ';
            }
        }
    }

    public void PlayGame()
    {
        bool gameWon = false;
        Player currentPlayer = player1;

        while (!gameWon)
        {
            DisplayBoard();

            Console.WriteLine($"{currentPlayer.Name}'s turn.");
            int column = currentPlayer.MakeMove();

            if (!PlaceDisc(currentPlayer.Marker, column))
            {
                Console.WriteLine("Invalid move. Try again.");
                continue;
            }

            if (CheckWin(currentPlayer.Marker))
            {
                DisplayBoard();
                Console.WriteLine($"{currentPlayer.Name} wins!");
                gameWon = true;
            }
            else if (IsBoardFull())
            {
                DisplayBoard();
                Console.WriteLine("The game is a draw!");
                break;
            }
            else
            {
                // Switch players
                currentPlayer = currentPlayer == player1 ? player2 : player1;
            }
        }
    }

    private void DisplayBoard()
    {
        Console.Clear();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Console.Write($"| {board[i, j]} ");
            }
            Console.WriteLine("|");
        }
        Console.WriteLine(new string('-', columns * 4));
        Console.WriteLine("  1   2   3   4");
    }

    
}