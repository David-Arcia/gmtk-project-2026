using UnityEngine;
public class MainMenuController : MonoBehaviour
{
    
    [SerializeField]
    public GameObject mainMenuElements;
    [SerializeField]
    public GameObject instructionElements;
    [SerializeField]
    public GameObject creditsElements;

    public void SwapToInstructions() {
        mainMenuElements.SetActive(false);
        instructionElements.SetActive(true);
        creditsElements.SetActive(false);
    }

    public void SwapToCredits() {
        mainMenuElements.SetActive(false);
        instructionElements.SetActive(false);
        creditsElements.SetActive(true);
    }

    public void SwapToMainMenu() {
        mainMenuElements.SetActive(true);
        instructionElements.SetActive(false);
        creditsElements.SetActive(false);
    }

    public void ExitApp()
    {
        Application.Quit();
    }
}
