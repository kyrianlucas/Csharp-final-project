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
        Console.WriteLine($"{Name}, enter the column (1-7) where you want to drop your disc:");
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
        //GameController controller = new GameController(player1, player2);

        // Start the game
        //controller.PlayGame();

        // End message
        Console.WriteLine("Thank you for playing Connect 4!");
    }
}
