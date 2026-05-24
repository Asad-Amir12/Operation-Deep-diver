using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class GameManager : MonoBehaviour
{
#region Fields
    [Header("Dependencies")]
    [SerializeField] private StabilizerController stabilizer;
    [SerializeField] private TextMeshProUGUI timerText; 
    [SerializeField] private TextMeshProUGUI warningText; 
    

    [Header("Win Condition")]
    [SerializeField] private int timeToWin = 45;
    
    [Header("Currents Mechanic")]
    [SerializeField] private float currentInterval = 5f;
    [SerializeField] private float minTorque = 5f;
    [SerializeField] private float maxTorque = 15f;

#endregion

#region State Variables
    private float gameTimer;
    private float currentTimer;
    private int lastSecondPrinted;
    private bool isGameOver = false;
    private int timeRemaining;
    private WaitForSeconds warningDuration ;
    private WaitForSeconds ws_1;
    public static GameManager Instance { get; private set; }
    
#endregion

#region Events

public static event System.Action OnGameOver;
public static event System.Action OnGameWin;
#endregion
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            
        }
    }
    void Start()
    {
        isGameOver = false;
        gameTimer = 0f;
        currentTimer = 0f;
        lastSecondPrinted = 0;
        warningDuration = new WaitForSeconds(currentInterval); // Cache this to avoid GC allocations
        ws_1 = new WaitForSeconds(1f); // For timer
        if (warningText != null) warningText.gameObject.SetActive(false);
        timeRemaining = timeToWin;
        StartCoroutine(Timer());
        StartCoroutine(WarningCoroutine());
    }

    void Update()
    {
        if (isGameOver) return;

        
    }

       IEnumerator WarningCoroutine()
    {
        // Add a while loop so it repeats every 5 seconds until the game is over
        while (!isGameOver)
        {
            yield return warningDuration;
            
            // Re-check game over just in case the game ended during the yield
            if (isGameOver) yield break; 

            float randomTorque = Random.Range(minTorque, maxTorque);
            int direction = Random.value > 0.5f ? 1 : -1;
            float finalTorque = randomTorque * direction;

            if (warningText != null)
            {
                warningText.gameObject.SetActive(true);
                warningText.text = direction == 1 ? "Surge: Left!" : "Surge: Right!";
                Invoke(nameof(HideWarning), 1.5f); // Hide after 1.5s
            }

            // Apply to stabilizer
            stabilizer.ApplyCurrentTorque(finalTorque);
        }
    }

    IEnumerator Timer()
    {
        // Add a while loop so it ticks down every 1 second
        while (!isGameOver && timeRemaining > 0)
        {
            yield return ws_1;
            
            if (isGameOver) yield break;

            timeRemaining--;
            if (timerText != null) timerText.text = $"Time Remaining: {timeRemaining}";

            if (timeRemaining <= 0)
            {
                WinGame();
            }
        }
    }
   

    private void HideWarning()
    {
        if (warningText != null) warningText.gameObject.SetActive(false);
    }

    public void LoseGame(string reason)
    {
        StopCoroutine(WarningCoroutine());
        StopCoroutine(Timer());
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over! " + reason);
        OnGameOver?.Invoke();
    }

    private void WinGame()
    {
        StopCoroutine(WarningCoroutine());
        StopCoroutine(Timer());
        isGameOver = true;
        Debug.Log("You Survived!");
        OnGameWin?.Invoke();
       
    }
}