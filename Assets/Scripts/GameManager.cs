using UnityEngine;
using UnityEngine.UI;
using TMPro; 
public class GameManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private StabilizerController stabilizer;
    [SerializeField] private TextMeshProUGUI timerText; 
    [SerializeField] private TextMeshProUGUI warningText; 

    [Header("Win Condition")]
    [SerializeField] private float timeToWin = 45f;
    
    [Header("Currents Mechanic")]
    [SerializeField] private float currentInterval = 5f;
    [SerializeField] private float minTorque = 5f;
    [SerializeField] private float maxTorque = 15f;

    private float gameTimer;
    private float currentTimer;
    private int lastSecondPrinted;
    private bool isGameOver = false;

    public static GameManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        gameTimer = 0f;
        currentTimer = 0f;
        lastSecondPrinted = 0;
        
        if (warningText != null) warningText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isGameOver) return;

        UpdateTimers();
    }

    private void UpdateTimers()
    {
        gameTimer += Time.deltaTime;
        currentTimer += Time.deltaTime;

        // GC Optimization: Only update the UI text when the whole second changes
        int currentSecond = Mathf.FloorToInt(gameTimer);
        if (currentSecond != lastSecondPrinted)
        {
            lastSecondPrinted = currentSecond;
            int timeRemaining = Mathf.Max(0, Mathf.FloorToInt(timeToWin) - currentSecond);
            if (timerText != null) timerText.text = $"Time Remaining: {timeRemaining}";
        }

        // Check Win Condition
        if (gameTimer >= timeToWin)
        {
            WinGame();
            return;
        }

        // Check Currents Mechanic
        if (currentTimer >= currentInterval)
        {
            currentTimer = 0f;
            TriggerDeepSeaCurrent();
        }
    }

    private void TriggerDeepSeaCurrent()
    {
        // Randomize magnitude and direction (-1 or 1)
        float randomTorque = Random.Range(minTorque, maxTorque);
        int direction = Random.value > 0.5f ? 1 : -1;
        float finalTorque = randomTorque * direction;

        // Visual warning
        if (warningText != null)
        {
            warningText.gameObject.SetActive(true);
            warningText.text = direction == 1 ? "Surge: Left!" : "Surge: Right!";
            Invoke(nameof(HideWarning), 1.5f); // Hide after 1.5s
        }

        // Apply to stabilizer
        stabilizer.ApplyCurrentTorque(finalTorque);
    }

    private void HideWarning()
    {
        if (warningText != null) warningText.gameObject.SetActive(false);
    }

    public void LoseGame(string reason)
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over! " + reason);
       
    }

    private void WinGame()
    {
        isGameOver = true;
        Debug.Log("You Survived!");
       
    }
}