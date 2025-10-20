using UnityEngine;
using UnityEngine.InputSystem;

public class ScooterMovementXR : MonoBehaviour
{
    public bool scooterMode = true;

    // --- Referencias a Input Actions ---
    [Header("Referencias de Input Actions")]
    // Asigna esta acción a la entrada del joystick (eje Y para adelante/atrás)
    public InputActionReference moveAction;
    // Asigna esta acción a la entrada del joystick (eje X para izquierda/derecha)
    // NOTA: Si usas una acción de Vector2 para el joystick, solo necesitarás una de estas referencias.
    public InputActionReference turnAction; 
    

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
        // Suscribirse a los eventos de las acciones cuando se activa el script
        if (moveAction != null && moveAction.action != null)
        {
            moveAction.action.Enable();
        }

        // Si usas una sola acción Vector2 para movimiento/giro, solo necesitas esta línea
        if (turnAction != null && turnAction.action != null)
        {
            turnAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        // Desuscribirse cuando se desactiva el script
        if (moveAction != null && moveAction.action != null)
        {
            moveAction.action.Disable();
        }
        if (turnAction != null && turnAction.action != null)
        {
            turnAction.action.Disable();
        }
    }

    // -------------------------------------------------------------------
    // --- Lógica de Movimiento ---
    // -------------------------------------------------------------------
    void Update()
    {
        if (!scooterMode)
        {
            return; // Si no estamos en modo patinete, no hacer nada
        }
        // 1. Leer el valor del joystick (Vector2)
        // Usamos TryReadValue para evitar errores si la acción no está asignada
        if (moveAction != null && moveAction.action != null)
        {
            moveInput = moveAction.action.ReadValue<Vector2>();
        } 
        // Si usas dos referencias (una para mover, una para girar), adapta la lectura aquí:
        // float verticalInput = moveAction.action.ReadValue<float>();
        // float horizontalInput = turnAction.action.ReadValue<float>();
        
        float verticalInput = moveInput.y;
        float horizontalInput = moveInput.x;

        // 2. Actualizar la velocidad actual (aceleración/deceleración)
        UpdateSpeed(verticalInput);

        // 3. Aplicar movimiento hacia adelante/atrás
        ApplyMovement();

        // 4. Aplicar rotación (giro)
        ApplyRotation(horizontalInput);
    }

    private void UpdateSpeed(float input)
    {
        if (Mathf.Abs(input) > 0.1f) // Usa un umbral pequeño para evitar movimientos accidentales
        {
            // Si hay entrada (acelerar), usa el input como dirección y magnitud
            float targetSpeed = input * maxSpeed;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            // Si no hay entrada (decelerar o frenar)
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }
    }

    private void ApplyMovement()
    {
        // Mueve el objeto a lo largo de su eje Z local
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    private void ApplyRotation(float horizontalInput)
    {
        // Gira el objeto alrededor de su eje Y
        float turnRate = horizontalInput * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, turnRate);
    }
}