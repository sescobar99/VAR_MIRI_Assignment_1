using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

// This class serves as a provider for the Patinete (scooter) functionality within the XR environment.
// When the right hand is turned like on a motorbike, the acceleration of the scooter is activated.
// The acceleration is higher as higher is the turn angle of the hand.
// The scooter decelerates when the hand is straightened.
// The scooter turns like a scooter or bike, depending on the turn angle of both hands.
// I am using XR Interaction Toolkit 3.2.1
public class PatineteProvider : UnityEngine.XR.Interaction.Toolkit.Locomotion.LocomotionProvider
{
    [Header("Controller Actions")]
    public InputActionReference leftHandPositionAction;
    public InputActionReference rightHandPositionAction;
    public InputActionReference rightHandRotationAction;

    [Header("Movement Settings")]
    public float maxSpeed = 5.0f;
    public float accelerationRate = 2.0f;
    public float decelerationRate = 3.0f;

    [Header("Rotation Thresholds")]
    [Range(0, 90)]
    public float minRotationAngle = 10.0f;
    [Range(0, 90)]
    public float maxRotationAngle = 60.0f;

    /// <summary>
    /// The transformation that is used by this component to apply translation movement.
    /// </summary>
    public XROriginMovement transformation { get; set; } = new XROriginMovement();

    [SerializeField]
    private float m_CurrentSpeed;

    [SerializeField, Tooltip("The current roll angle of the hand used for throttle calculation.")]
    private float m_RollAngle;

    protected void OnEnable()
    {
        rightHandRotationAction.action.Enable();
    }

    protected void OnDisable()
    {
        rightHandRotationAction.action.Disable();
    }

    void Update()
    {
        // Always read rotation for debugging purposes
        var handRotation = rightHandRotationAction.action.ReadValue<Quaternion>();
        Debug.LogWarning($"Raw Controller Quaternion: {handRotation.x}, {handRotation.y}, {handRotation.z}, {handRotation.w}");

        // Calculate throttle for debugging regardless of locomotion state
        float debugThrottle = CalculateThrottle(handRotation);
        Debug.LogWarning($"Calculated Throttle: {debugThrottle}");
        
        // if (CanBeginLocomotion())
        // {

            // Calculate throttle based on hand rotation
            float throttle = CalculateThrottle(handRotation);

            // Determine the target speed
            float targetSpeed = throttle * maxSpeed;

            // Smoothly adjust the current speed towards the target speed
            if (m_CurrentSpeed < targetSpeed)
                m_CurrentSpeed = Mathf.MoveTowards(m_CurrentSpeed, targetSpeed, accelerationRate * Time.deltaTime);
            else
                m_CurrentSpeed = Mathf.MoveTowards(m_CurrentSpeed, targetSpeed, decelerationRate * Time.deltaTime);

            // If there's any speed, begin locomotion
            if (m_CurrentSpeed > 0.001f)
            {
                MoveScooter();
            }
            else
            {
                TryEndLocomotion();
            }
        // }
        // else
        // {
        //     TryEndLocomotion();
        // }
    }

    /// <summary>
    /// Calculates the throttle value based on the hand's rotation.
    /// </summary>
    /// <param name="handRotation">The current rotation of the hand.</param>
    /// <returns>A throttle value between 0 and 1.</returns>
    private float CalculateThrottle(Quaternion handRotation)
    {
        // Method 1: Using Euler angles directly
        Vector3 eulerAngles = handRotation.eulerAngles;
        float eulerRollAngle = eulerAngles.z;
        if (eulerRollAngle > 180f) eulerRollAngle -= 360f;
        
        // Method 2: Using forward and right vectors
        Vector3 handForward = handRotation * Vector3.forward;
        Vector3 handRight = handRotation * Vector3.right;
        Vector3 handUp = handRotation * Vector3.up;
        
        // Log all vectors for debugging
        Debug.LogWarning($"Hand Forward: {handForward}");
        Debug.LogWarning($"Hand Right: {handRight}");
        Debug.LogWarning($"Hand Up: {handUp}");

        // Este es el bueno
        float upTiltAngle = Vector3.Angle(handUp, Vector3.up);
        /* Vector3 tiltAxis = Vector3.Cross(Vector3.up, handUp).normalized;
        float upRollAngle = Vector3.Angle(tiltAxis, Vector3.right);
        
        // Calculate angles with dot products
        float dotRollAngle = Mathf.Asin(Vector3.Dot(handRight, Vector3.up)) * Mathf.Rad2Deg;
        
        Debug.LogWarning($"Euler Roll Angle: {eulerRollAngle}");
        Debug.LogWarning($"Up Tilt Angle: {upTiltAngle}");
        Debug.LogWarning($"Up Roll Angle: {upRollAngle}");
        Debug.LogWarning($"Dot Roll Angle: {dotRollAngle}");
        
        // Use the dot product version for now
        float rollAngle = dotRollAngle;
        m_RollAngle = rollAngle;
        Debug.LogWarning($"Selected Roll Angle: {m_RollAngle}");

        // Use the absolute value to allow twisting in either direction
        rollAngle = Mathf.Abs(rollAngle);

        // Map the angle from the min/max range to a 0-1 throttle value
        if (rollAngle < minRotationAngle)
            return 0f;

        return Mathf.Clamp01((rollAngle - minRotationAngle) / (maxRotationAngle - minRotationAngle));*/
        return Mathf.Clamp01((upTiltAngle - minRotationAngle) / (maxRotationAngle - minRotationAngle));
    }

    /// <summary>
    /// Applies the movement to the XR Origin.
    /// </summary>
    private void MoveScooter()
    {
        var xrOrigin = mediator.xrOrigin;
        if (xrOrigin == null)
            return;

        // Get the forward direction of the XR Origin's camera
        var xrOriginTransform = xrOrigin.Origin.transform;
        var cameraForward = xrOrigin.Camera.transform.forward;

        // We want to move along the ground, so we project the camera's forward vector onto the horizontal plane.
        var forwardDirection = Vector3.ProjectOnPlane(cameraForward, Vector3.up).normalized;
        if (forwardDirection == Vector3.zero)
        {
            // If the camera is looking straight up or down, use the rig's forward direction instead.
            forwardDirection = Vector3.ProjectOnPlane(xrOriginTransform.forward, Vector3.up).normalized;
        }

        // Calculate the movement vector for this frame
        var movement = forwardDirection * (m_CurrentSpeed * Time.deltaTime);

        TryStartLocomotionImmediately();
        if (locomotionState == LocomotionState.Moving)
        {
            transformation.motion = movement;
            TryQueueTransformation(transformation);
        }
    }
}