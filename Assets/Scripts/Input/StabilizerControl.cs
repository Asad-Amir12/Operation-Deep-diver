using UnityEngine;

public class StabilizerController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform leftTip;
    [SerializeField] private Transform rightTip;
    [SerializeField] private ParticleSystem leftThrusterParticles;
    [SerializeField] private ParticleSystem rightThrusterParticles;
    [Header("Settings")]
    [SerializeField] private float thrustPower = 200f;
    [SerializeField] private float maxTiltAngle = 45f;

    // State variables (kept out of methods to avoid GC)
    private bool applyLeftThrust;
    private bool applyRightThrust;
    private float currentAngle;
 private bool isGameOver = false;
  void Update()
    {
       // if (isGameOver) return; // Stop checking inputs if game is over

        ProcessInput();
        CheckFailState();
    }

    void FixedUpdate()
    {
       // if (isGameOver) return; // Stop applying thrust if game is over

        ApplyPhysicsThrust();
        ClampRotation(); // Add this new method call here!
    }

    private void ProcessInput()
    {
        applyLeftThrust = false;
        applyRightThrust = false;

        // Zero-GC Mobile Touch Input
        int touchCount = Input.touchCount;
        for (int i = 0; i < touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            
            // Only process active touches
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
            {
                if (touch.position.x < Screen.width * 0.5f)
                    applyLeftThrust = true;
                else
                    applyRightThrust = true;
            }
        }


        // Editor fallback for testing
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) applyLeftThrust = true;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) applyRightThrust = true;
#endif
    }

       private void ApplyPhysicsThrust()
    {
        // By changing angularVelocity directly, we bypass the mass of the ball.
        // thrustPower now represents 'Degrees Per Second' of rotation instead of raw force.
        
        float rotationAcceleration = thrustPower * Time.fixedDeltaTime;

        if (applyLeftThrust)
        {
            // Subtracting angular velocity rotates it clockwise (Rightward tilt)
            rb.angularVelocity -= rotationAcceleration;
        }
        
        if (applyRightThrust)
        {
            // Adding angular velocity rotates it counter-clockwise (Leftward tilt)
            rb.angularVelocity += rotationAcceleration;
        }
    }
    private void ClampRotation()
    {
        float angle = rb.rotation;
        
        // Normalize angle to be between -180 and 180
        angle = angle > 180 ? angle - 360 : angle;
        angle = angle < -180 ? angle + 360 : angle;

        if (angle >= maxTiltAngle)
        {
            rb.rotation = maxTiltAngle;
            if (rb.angularVelocity > 0) rb.angularVelocity = 0;
        }
        else if (angle <= -maxTiltAngle)
        {
            rb.rotation = -maxTiltAngle;
            if (rb.angularVelocity < 0) rb.angularVelocity = 0;
        }
    }
        private void CheckFailState()
    {
        currentAngle = rb.rotation;
        currentAngle = currentAngle > 180 ? currentAngle - 360 : currentAngle;

        if (Mathf.Abs(currentAngle) >= maxTiltAngle)
        {
            if (!isGameOver)
            {
                isGameOver = true;
                // Find GameManager and trigger loss
                GameManager.Instance.LoseGame("Tilt exceeded 45 degrees.");
            }
        }
    }

        public void ApplyCurrentTorque(float torqueAmount)
    {
        Debug.Log($"Applying current torque: {torqueAmount}");
        rb.AddTorque(torqueAmount, ForceMode2D.Impulse);

        if (torqueAmount > 0)
        {
            if (rightThrusterParticles != null)
            {
                rightThrusterParticles.Play();
                Invoke(nameof(StopRightParticles), 1f); // Stops after 1 second
            }
        }
        else if (torqueAmount < 0)
        {
            if (leftThrusterParticles != null)
            {
                leftThrusterParticles.Play();
                Invoke(nameof(StopLeftParticles), 1f); // Stops after 1 second
            }
        }
    }

    private void StopRightParticles()
    {
        if (rightThrusterParticles != null) rightThrusterParticles.Stop();
    }

    private void StopLeftParticles()
    {
        if (leftThrusterParticles != null) leftThrusterParticles.Stop();
    }
}