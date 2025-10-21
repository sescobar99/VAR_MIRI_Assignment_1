using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    [Tooltip("Minimum Y position before reset")]
    public float fallThreshold = -10f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }


    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < fallThreshold)
        {
            ResetRigPosition();
        }
    }

    public void ResetRigPosition()
    {
        transform.SetPositionAndRotation(initialPosition, initialRotation);
    }
}



   