using UnityEngine;

public class initWater : MonoBehaviour
{
    public ParticleSystem waterParticleSystem;
    public float pourAngleThreshold = 30f; // Por ejemplo, 60 grados.
    public float maxPourAngle = 160f;

    [SerializeField]
    private bool isPouring = false;

    [SerializeField]
    private float localZRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (waterParticleSystem != null)
        {
            waterParticleSystem.Stop();
        }
        else
        {
            Debug.LogError("There are no particle system");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Warning, only works if can has no parent
        // TODO: use https://stackoverflow.com/a/19684901
        localZRotation = (transform.localEulerAngles.z + 360) % 360;
        bool shouldPour = (localZRotation >= pourAngleThreshold) && (localZRotation <= maxPourAngle);

        if (shouldPour && !isPouring)
        {
            waterParticleSystem.Play();
            isPouring = true;
        }
        else if (!shouldPour && isPouring)
        {
            waterParticleSystem.Stop();
            isPouring = false;
        }
    }       
    
}
