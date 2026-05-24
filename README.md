# Operation Deep-Diver: Submersible Hull Integrity

This project is a 2D physics-based cooperative prototype testing mobile frame rate consistency and tight synchronization under local constraints, developed for the Neptune-V evaluation.

## Architectural Approach

The system is designed with a strict decoupling between physics processing, game state management, and input handling to ensure maximum performance and adherence to the zero unnecessary Garbage Collection (GC) mandate.

* **StabilizerController (Input & Physics):** 
  * Acts as the core mechanical driver. 
  * Input is processed using a standard `for` loop over `Input.touchCount` paired with `Input.GetTouch(i)`. This bypasses the hidden array allocation of `Input.touches`, completely eliminating GC spikes during mobile multi-touch.
  * To ensure mathematical consistency when the players are compensating against gravity, thrust is applied by directly manipulating the `Rigidbody2D.angularVelocity` combined with Unity's Angular Drag, rather than applying raw unconstrained force. This creates a responsive, non-slippery feel regardless of the Anomaly's mass.
  * Rotation is physically clamped in `FixedUpdate` to enforce the 45-degree fail state and prevent physics engine tunneling or momentum buildup.

* **GameManager (State & Timers):**
  * Operates as a singleton managing the core loop (45-second win condition, 5-second current bursts).
  * Timer systems utilize Coroutines (`IEnumerator`) with pre-cached `WaitForSeconds` objects. This prevents frame-by-frame float math evaluation in the `Update` loop and avoids the GC allocation created by instantiating new `WaitForSeconds` objects at runtime.
  * UI Text updates are constrained to fire only when the integer value of the seconds changes, rather than parsing strings (`.ToString()`) every frame.

* **FallDetector:**
  * Uses `OnTriggerEnter2D` coupled with `CompareTag("FallZone")` to detect fail states without allocating new strings to memory.

## AI Usage

* **GitHub Copilot (Gemini 3.1 Pro Preview):** 
  * Used as a brainstorming and architectural sounding board to ensure strict adherence to the zero-GC mandate.
  * Assisted in optimizing the mobile touch input loop (replacing `Input.touches` with `Input.GetTouch`).
  * Consulted for physics tuning strategies, specifically the decision to manipulate `angularVelocity` and utilize `PhysicsMaterial2D` properties (friction tuning) to create a responsive, interview-ready prototype without the Anomaly endlessly rolling or slipping.
  * Aided in structuring the Coroutines in the `GameManager` to properly loop without generating garbage.

## Uncertainties & Ambiguities

During development, a few mechanical ambiguities were identified and resolved with specific design choices:

1. **The Nature of the Anomaly (Sphere vs Box):** The prompt allowed for either a primitive sphere or box. I opted to freeze the Z-rotation constraint of the object (and utilize a custom low-friction PhysicsMaterial2D). Allowing a perfect sphere to roll freely on a tilting platform created an incredibly difficult difficulty curve. Constraining its rotation to a slide, or using a box, aligned better with a 45-second survival test.
2. **Current Surge Delivery (Force vs Impulse):** The prompt dictated a "sudden, unpredictable current surge." I utilized `ForceMode2D.Impulse` for an instant burst of torque. This required careful balancing of Angular Drag on the stabilizer plate so the impulse didn't instantly push the plate past the 45-degree kill threshold before the players could react.
3. **Thrust Translation (Forces vs Velocity):** When applying compensatory thrust, standard `AddForce` is heavily impacted by the resting mass of the falling object. I made the conscious design choice to alter `angularVelocity` directly. This ensures the players' inputs are always mathematically consistent and responsive, adhering to the requirement of an "input-precise" prototype.