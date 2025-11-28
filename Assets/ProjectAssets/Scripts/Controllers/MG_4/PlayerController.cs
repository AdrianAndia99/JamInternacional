using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float limitX = 8f;

    void Update()
    {
        float move = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        transform.position += new Vector3(move, 0, 0);

        // Limitar el movimiento
        float clampedX = Mathf.Clamp(transform.position.x, -limitX, limitX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Candy"))
        {
            CandyCatchManager.Instance.AddScore(1);
            Destroy(col.gameObject);
        }
        else if (col.CompareTag("Rotten"))
        {
            CandyCatchManager.Instance.AddScore(-1);
            Destroy(col.gameObject);
        }
    }
}
