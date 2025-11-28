using UnityEngine;

public abstract class ObjectMovement : MonoBehaviour
{
    [Header("--------- Key parameters ---------")]
    [SerializeField] protected float speed = 10f;
    [SerializeField] protected Vector3 direction = Vector3.forward;

    [Header("--------- Dynamic Pooling ---------")]
    [SerializeField] protected DynamicPooling myPooling;

    private void Update()
    {
        Move();
    }
    protected virtual void Move()
    {
        transform.position += direction.normalized * speed * Time.deltaTime;
    }
    public virtual void Remove(Collider other)
    {
        if (other.gameObject.tag == "PoolLimit")
        {
            myPooling.ReturnObject(this.gameObject);
        }
        else if (other.gameObject.tag == "DestroyLimit")
        {
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Player")
        {
            myPooling.ReturnObject(this.gameObject);
        }
    }
    public virtual void SetSpeed(float _speed)
    {
        speed = _speed;
    }
}