using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{

    [SerializeField] private Button retryButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private string mainMenuSceneName = "MainMenuScene";
    [SerializeField] private GameObject gameOverPanel;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.OnGameOver += ShowGameOverPanel;
        retryButton.onClick.AddListener(OnRetryButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
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
        GameManager.OnGameOver -= ShowGameOverPanel;
    }
}
