using UnityEngine;

public class Move_Water : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float scrollSpeedX = 0.05f;
    public float scrollSpeedY = 0.03f;
    private Renderer rend;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Time.time * scrollSpeedX;
        float y = Time.time * scrollSpeedY;
        rend.material.mainTextureOffset = new Vector2(x, y);
    }
}
