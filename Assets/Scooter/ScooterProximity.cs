using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

public class ScooterProximity : MonoBehaviour
{
    // Asigna el MeshRenderer del patinete en el Inspector
    public MeshRenderer scooterRenderer;

    public LocomotionProvider enableProvider;

    // Se llama cuando otro collider entra en el trigger
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Objeto en OnTriggerEnter.");
        // Revisa si el objeto que entró es el personaje (p. ej., por su tag o nombre)
        if (other.CompareTag("Player"))
        {
            // Oculta el patinete
            scooterRenderer.enabled = false;
            enableProvider.enabled = true;
            this.gameObject.SetActive(false);
            Debug.Log("Patinete oculto.");
            //ScooterMovement.StartScooterMode();
        }
    }

    // Se llama cuando otro collider sale del trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Objeto en OnTriggerExit.");
            //this.gameObject.SetActive(true);
            //scooterRenderer.enabled = true;
            //ScooterMovement.StopScooterMode();
        }
    }
}
