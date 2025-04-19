public class PlayerSwitcher
{
    private string currentPlayer;
    private string player1 = "Player 1";
    private string player2 = "Player 2";

    public void SwitchPlayer()
    {
        currentPlayer = currentPlayer == player1 ? player2 : player1;
    }
}
