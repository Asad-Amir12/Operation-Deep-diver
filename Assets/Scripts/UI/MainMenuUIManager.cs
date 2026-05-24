using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        // Load the main game scene (make sure to add it to your Build Settings)
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    private void OnQuitButtonClicked()
    {
        // Quit the application (note: this won't do anything in the editor)
        Application.Quit();
    }
}
