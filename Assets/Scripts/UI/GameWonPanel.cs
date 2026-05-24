using UnityEngine;
using UnityEngine.UI;

public class GameWonPanel : MonoBehaviour
{

    [SerializeField] private Button retryButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";
    [SerializeField] private GameObject gameWonPanel;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.OnGameWin += ShowGameWonPanel;
        retryButton.onClick.AddListener(OnRetryButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void ShowGameWonPanel()
    {
        gameWonPanel.SetActive(true);
    }

    private void OnRetryButtonClicked()
    {
        // Reload the current scene to restart the game
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    private void OnQuitButtonClicked()
    {
         UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }

    void OnDestroy()
    {
        retryButton.onClick.RemoveListener(OnRetryButtonClicked);
        quitButton.onClick.RemoveListener(OnQuitButtonClicked);
        GameManager.OnGameWin -= ShowGameWonPanel;
    }
}
