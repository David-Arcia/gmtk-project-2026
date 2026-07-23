using UnityEngine;
public enum GameStates
{
    PLAYING,
    EXIT,
}

public class GameStateController : MonoBehaviour
{
    public GameStates currGameState = GameStates.PLAYING;
    

    public void ExitApp()
    {
        Application.Quit();
    }
}
