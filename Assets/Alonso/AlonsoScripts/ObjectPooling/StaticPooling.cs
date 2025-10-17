using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StaticPooling", menuName = "ObjectPooling/StaticPooling")]
public class StaticPooling : ScriptableObject, IObjectPooling
{
    [Header("Prefab and limit")]
    [SerializeField] private GameObject objectPrefab; // The object that is gonna be pooled
    [SerializeField] private int maxInstances; // The limit of instances that can exist

    private Queue<GameObject> availableObjects; // Objects wich  can be pooled
    private List<GameObject> allInstances; // Every instance of the prefab from this pool that exist

    ///<summary>
    /// This shi is her just for narrative purposes (￢_￢)
    ///</summary>
    public void SetUp()
    {
        if (availableObjects == null || allInstances == null)
        {
            availableObjects = new Queue<GameObject>();
            allInstances = new List<GameObject>();
        }

        availableObjects.Clear();
        allInstances.Clear();
    }
    ///<summary>
    /// This takes an object from this pool but as a child (◠‿◠)
    ///</summary>
    public GameObject GetObject(Transform parent)
    {
        GameObject objectInstance = null; // This object will travel through this function ◕⩊◕

        // If there are available objects you take one (˶ᵔᵕᵔ˶)
        if (availableObjects.Count > 0)
        {
            objectInstance = availableObjects.Dequeue();
        }
        // If there are not available objects you instance a new one, only if you have not passed the instance limit (´▽`)b
        else if (allInstances.Count < maxInstances)
        {
            objectInstance = Instantiate(objectPrefab);
            allInstances.Add(objectInstance);
        }
        // If at this point "objectInstance" isn´t null is geometrically transformed ≽^•⩊•^≼
        if (objectInstance != null)
        {
            objectInstance.transform.SetParent(parent);
            objectInstance.transform.localPosition = Vector3.zero;
            objectInstance.transform.localRotation = Quaternion.identity;
            objectInstance.SetActive(true);
        }
        // This means everything failed, so return null (;¬_¬)
        return objectInstance;
    }
    ///<summary>
    /// This takes an object from this pool (◠‿◠)
    ///</summary>
    public GameObject GetObject(Vector3 newPosition, Quaternion newRotation)
    { 
        GameObject objectInstance = null; // This object will travel through this function ◕⩊◕

        // If there are available objects you take one (˶ᵔᵕᵔ˶)
        if (availableObjects.Count > 0)
        {
            objectInstance = availableObjects.Dequeue();
        }
        // If there are not available objects you instance a new one, only if you have not passed the instance limit (´▽`)b
        else if (allInstances.Count < maxInstances)
        {
            objectInstance = Instantiate(objectPrefab);
            allInstances.Add(objectInstance);
        }
        // If at this point "objectInstance" isn´t null is geometrically transformed ≽^•⩊•^≼
        if (objectInstance != null)
        {
            objectInstance.transform.SetParent(null);
            objectInstance.transform.position = newPosition;
            objectInstance.transform.rotation = newRotation;
            objectInstance.SetActive(true);
        }
        // This means everything failed, so return null (;¬_¬)
        return objectInstance;
    }
    ///<summary>
    /// This returns an object to this pool (˶˃ ᵕ ˂˶)
    ///</summary>
    public void ReturnObject(GameObject objectInstance)
    {
        if (!allInstances.Contains(objectInstance)) return;
        // This does not do anything if an object diferent to "prefab" is returned with this method（˶′◡‵˶）

        objectInstance.SetActive(false);
        objectInstance.transform.SetParent(null);
        objectInstance.transform.position = Vector3.zero;
        objectInstance.transform.rotation = Quaternion.identity;
        availableObjects.Enqueue(objectInstance);
    }
}