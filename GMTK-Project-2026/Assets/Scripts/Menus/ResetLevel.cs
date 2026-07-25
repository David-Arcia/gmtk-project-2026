using UnityEngine;
using UnityEngine.SceneManagement;
public class ResetLevel : MonoBehaviour
{
    private Input inputController;
    [SerializeField]
    public GameObject pauseButton;
    [SerializeField]
    public GameObject pauseMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputController = GetComponent<Input>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputController.PressedReset)
        {
            ResetScene();
        }
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void ShowPauseScreen(bool show)
    {
        pauseMenu.SetActive(show);
        pauseButton.SetActive(!show);
        if (show)
        {
            Time.timeScale = 0f;
        } else
        {
            Time.timeScale = 1f;
        }
    }
}
