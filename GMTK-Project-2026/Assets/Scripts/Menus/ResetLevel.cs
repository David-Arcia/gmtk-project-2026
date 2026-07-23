using UnityEngine;
using UnityEngine.SceneManagement;
public class ResetLevel : MonoBehaviour
{
    private Input inputController;
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
}
