using UnityEngine;

public class ScooterProximity : MonoBehaviour
{
    // Asigna el MeshRenderer del patinete en el Inspector
    public MeshRenderer scooterRenderer;

    // Se llama cuando otro collider entra en el trigger
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Objeto en OnTriggerEnter.");
        // Revisa si el objeto que entró es el personaje (p. ej., por su tag o nombre)
        if (other.CompareTag("Player"))
        {
            // Oculta el patinete
            scooterRenderer.enabled = false;
            // CharacterMovement.StartScooterMode();
        }
    }

    // Se llama cuando otro collider sale del trigger (útil si quieres que reaparezca)
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            scooterRenderer.enabled = true;
            // CharacterMovement.StopScooterMode();
        }
    }
}
