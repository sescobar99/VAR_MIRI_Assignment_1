using UnityEngine;

public class RotateLight : MonoBehaviour
{
    [Tooltip("Rotation step in degrees (default: 90°).")]
    public float rotationStep = 90f;

    [Tooltip("Rotate around this axis (default: X axis).")]
    public Vector3 rotationAxis = Vector3.right;

    public void PassTime() => transform.Rotate(rotationAxis * rotationStep, Space.World);
}