using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public float fallSpeed = 4f;

    void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}

