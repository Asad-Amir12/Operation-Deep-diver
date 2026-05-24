using UnityEngine;

public class FallDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we collided with has the tag "FallZone"
        if (other.CompareTag("fallZone"))
        {
            // Trigger the Game Over state in the GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseGame("Anomaly fell off the stabilizer.");
            }
            else
            {
                Debug.LogWarning("Fall detected, but no GameManager found!");
            }
        }
    }
}