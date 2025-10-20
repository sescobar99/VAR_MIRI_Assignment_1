using UnityEngine;
using UnityEngine.InputSystem;

public class ScooterMovementXR : MonoBehaviour
{
    public bool scooterMode = true;

    // --- Referencias a Input Actions ---
    [Header("Referencias de Input Actions")]
    // Asigna esta acción a la entrada del joystick (eje Y para adelante/atrás)
    public InputActionReference moveAction;
    // (opcional) acción única para giro (Vector2 or float X)
    public InputActionReference turnAction;

    // --- Si usas entradas separadas en cada mano (XRI) ---
    // Asigna la acción del joystick izquierdo (Vector2) - solo X se usa para giro
    public InputActionReference leftTurnAction;
    // Asigna la acción del joystick derecho (Vector2) - solo X se usa para giro
    public InputActionReference rightTurnAction;

    // --- Ajustes de Velocidad ---
    [Header("Ajustes de Movimiento")]
    public float maxSpeed = 5f;        
    public float acceleration = 2f;    
    public float deceleration = 10f;   
    public float rotationSpeed = 100f; 

    // --- Variables Internas ---
    private float currentSpeed = 0f;
    private Vector2 moveInput = Vector2.zero; // Almacenará el valor (x, y) del joystick

    // -------------------------------------------------------------------
    // --- Configuración de Eventos de Acciones ---
    // -------------------------------------------------------------------

    private void StartScooterMode()
    {
        scooterMode = true;
    }
    private void StopScooterMode()
    {
        scooterMode = false;
    }
    private void OnEnable()
    {
        if (moveAction != null && moveAction.action != null) moveAction.action.Enable();
        if (turnAction != null && turnAction.action != null) turnAction.action.Enable();
        if (leftTurnAction != null && leftTurnAction.action != null) leftTurnAction.action.Enable();
        if (rightTurnAction != null && rightTurnAction.action != null) rightTurnAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null && moveAction.action != null) moveAction.action.Disable();
        if (turnAction != null && turnAction.action != null) turnAction.action.Disable();
        if (leftTurnAction != null && leftTurnAction.action != null) leftTurnAction.action.Disable();
        if (rightTurnAction != null && rightTurnAction.action != null) rightTurnAction.action.Disable();
    }

    // -------------------------------------------------------------------
    // --- Lógica de Movimiento ---
    // -------------------------------------------------------------------
    void Update()
    {
        if (!scooterMode) return;

        if (moveAction != null && moveAction.action != null)
        {
            moveInput = moveAction.action.ReadValue<Vector2>();
        }

        float verticalInput = moveInput.y;

        // Read horizontal/turn input: prefer single turnAction if assigned,
        // otherwise read left/right joystick X and pick a strategy.
        float horizontalInput = 0f;

        if (turnAction != null && turnAction.action != null)
        {
            // If turnAction is Vector2, take x; if it's a float action, ReadValue<float>()
            var turnVec = turnAction.action.controls.Count > 0 && turnAction.action.controls[0].valueType == typeof(Vector2)
                ? turnAction.action.ReadValue<Vector2>().x
                : turnAction.action.ReadValue<float>();
            horizontalInput = turnVec;
        }
        else
        {
            float rightX = 0f, leftX = 0f;
            if (rightTurnAction != null && rightTurnAction.action != null)
            {
                rightX = rightTurnAction.action.ReadValue<Vector2>().x;
            }
            if (leftTurnAction != null && leftTurnAction.action != null)
            {
                leftX = leftTurnAction.action.ReadValue<Vector2>().x;
            }

            // Strategy options:
            // 1) Prefer right stick when it has input, otherwise left (used here)
            if (Mathf.Abs(rightX) > 0.05f) horizontalInput = rightX;
            else horizontalInput = leftX;

            // 2) Alternative: average both sticks
            // horizontalInput = (leftX + rightX) * 0.5f;

            // 3) Alternative: sum both sticks (useful if both contribute)
            // horizontalInput = leftX + rightX;
        }

        UpdateSpeed(verticalInput);
        ApplyMovement();
        ApplyRotation(horizontalInput);
    }

    private void UpdateSpeed(float input)
    {
        if (Mathf.Abs(input) > 0.1f)
        {
            float targetSpeed = input * maxSpeed;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }
    }

    private void ApplyMovement()
    {
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    private void ApplyRotation(float horizontalInput)
    {
        float turnRate = horizontalInput * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, turnRate);
    }
}