using UnityEngine;

public class PlantGrower : MonoBehaviour
{
    public float maxGrowth = 5;
    public float growRate = 1.01f;
    public string soilTag = "Soil.001";

    [SerializeField]
    private Transform transformPlant;
    [SerializeField]
    private float growth = 0;
    [SerializeField]
    private Vector3 initScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transformPlant = transform.Find("Planta");

        if (transformPlant == null){
            Debug.LogError("ContinuousPlantGrower couldnt find Planta");
            return;
        }

        initScale = transformPlant.localScale;
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Agua") && growth < maxGrowth)
        {
            growth *= growRate;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transformPlant == null){
            return;
        }
        
        transformPlant.localScale = initScale * growth;
    }
}
