using UnityEngine;
using UnityEngine.SceneManagement; 
public class MainMenuController : MonoBehaviour
{
    public enum MainMenuState
    {
        MAIN_MENU,
        GAMEPLAY,
        INSTRUCTIONS,
        CREDITS,
        EXIT
    }
    public MainMenuState currState = MainMenuState.MAIN_MENU;
    [SerializeField]
    public GameObject mainMenuElements;
    [SerializeField]
    public GameObject instructionElements;
    [SerializeField]
    public GameObject creditsElements;
    [SerializeField]
    public string gameScene;

    public void SwapToGameplay()
    {
        SceneManager.LoadScene(gameScene);
        currState = MainMenuState.GAMEPLAY;
    }

    public void SwapToInstructions() {
        mainMenuElements.SetActive(false);
        instructionElements.SetActive(true);
        creditsElements.SetActive(false);
        currState = MainMenuState.INSTRUCTIONS;
    }

    public void SwapToCredits() {
        mainMenuElements.SetActive(false);
        instructionElements.SetActive(false);
        creditsElements.SetActive(true);
        currState = MainMenuState.CREDITS;
    }

    public void SwapToMainMenu() {
        mainMenuElements.SetActive(true);
        instructionElements.SetActive(false);
        creditsElements.SetActive(false);
        currState = MainMenuState.MAIN_MENU;
    }

    public void ExitApp()
    {
        currState = MainMenuState.EXIT;
        Application.Quit();
    }
}
