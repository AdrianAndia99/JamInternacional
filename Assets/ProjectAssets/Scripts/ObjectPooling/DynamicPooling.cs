using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DynamicPooling", menuName = "ObjectPooling/DynamicPooling")]
public class DynamicPooling : ScriptableObject, IObjectPooling
{
    [Header("Prefab")]
    [SerializeField] private GameObject objectPrefab; // The object that is gonna be pooled

    private Queue<GameObject> availableObjects; // Objects which can be pooled
    private List<GameObject> allInstances; // Every instance of the prefab from this pool that exist

    ///<summary>
    /// DONT FORGET TO USE THIS AT AWAKE!!! (￢_￢)
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
        // If there are not available objects, instance a new one (´▽`)b
        else
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
        // If there are not available objects, instance a new one (´▽`)b
        else
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
        // This does not do anything if an object different to "prefab" is returned with this method（˶′◡‵˶）

        objectInstance.SetActive(false);
        objectInstance.transform.SetParent(null);
        objectInstance.transform.position = Vector3.zero;
        objectInstance.transform.rotation = Quaternion.identity;
        availableObjects.Enqueue(objectInstance);
    }
}