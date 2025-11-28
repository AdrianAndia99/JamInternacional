using UnityEngine;

public interface IObjectPooling
{
    abstract GameObject GetObject(Transform parent);
    abstract GameObject GetObject(Vector3 newPosition, Quaternion newRotation);
    abstract void ReturnObject(GameObject objectInstance);
}