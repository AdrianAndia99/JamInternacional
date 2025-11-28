using UnityEngine;

public class TeleportHelper : MonoBehaviour
{
    public void TransformTeleport(Transform target)
    {
        transform.position = target.position;
    }
}