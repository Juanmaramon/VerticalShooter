using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Core
{

    /// <summary>
    /// Object pool item.
    /// </summary>
    [System.Serializable]
    public class ObjectPoolItem
    {
        public GameObject objectToPool;
        public string poolName;
        public int amountToPool;
        public bool shouldExpand = true;
    }

    /// <summary>
    /// Object pooler. Reuse objects to avoid GC spikes
    /// </summary>
    public class ObjectPooler : MonoBehaviour
    {
        public const string DefaultRootObjectPoolName = "Pooled Objects";

        public static ObjectPooler Instance;
        public string rootPoolName = DefaultRootObjectPoolName;
        public List<GameObject> pooledObjects;
        public List<ObjectPoolItem> itemsToPool;

        void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (string.IsNullOrEmpty(rootPoolName))
                rootPoolName = DefaultRootObjectPoolName;

            GetParentPoolObject(rootPoolName);

            pooledObjects = new List<GameObject>();
            foreach (var item in itemsToPool)
            {
                for (int i = 0; i < item.amountToPool; i++)
                {
                    StartCoroutine(CreatePooledObject(item));
                }
            }
        }

        public GameObject GetParentPoolObject(string objectPoolName)
        {
            // Use the root object pool name if no name was specified
            if (string.IsNullOrEmpty(objectPoolName))
                objectPoolName = rootPoolName;

            GameObject parentObject = GameObject.Find(objectPoolName);

            // Create the parent object if necessary
            if (parentObject == null)
            {
                parentObject = new GameObject();
                parentObject.name = objectPoolName;

                // Add sub pools to the root object pool if necessary
                if (objectPoolName != rootPoolName)
                    parentObject.transform.parent = GameObject.Find(rootPoolName).transform;
            }

            return parentObject;
        }

        public GameObject GetPooledObject(string tag)
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].CompareTag(tag))
                {
                    pooledObjects[i].SetActive(true);
                    return pooledObjects[i];
                }
            }

            foreach (var item in itemsToPool)
            {
                if (item.objectToPool.CompareTag(tag))
                {
                    if (item.shouldExpand)
                    {
                        return CreatePooledObject(item, true);
                    }
                }
            }

            return null;
        }

        private IEnumerator CreatePooledObject(ObjectPoolItem item)
        {
            // Load assets async
            ResourceRequest resourceRequest = Resources.LoadAsync<GameObject>("Prefabs/" + item.poolName);
            while (!resourceRequest.isDone)
            {
                yield return 0;
            }

            // Instantiate prefab from async loaded prefab
            GameObject objOriginal = resourceRequest.asset as GameObject;
            GameObject obj = Instantiate<GameObject>(objOriginal);

            // Get the parent for this pooled object and assign the new object to it
            var parentPoolObject = GetParentPoolObject(item.poolName);
            obj.transform.parent = parentPoolObject.transform;

            obj.SetActive(false);
            pooledObjects.Add(obj);
            yield return obj;
        }

        private GameObject CreatePooledObject(ObjectPoolItem item, bool expading)
        {
            GameObject obj;
            obj = Instantiate<GameObject>(item.objectToPool);  

            // Get the parent for this pooled object and assign the new object to it
            var parentPoolObject = GetParentPoolObject(item.poolName);
            obj.transform.parent = parentPoolObject.transform;

            obj.SetActive(true);
            pooledObjects.Add(obj);
            return obj;
        }

        public void ReturnPooledObjects()
        {
            for (int i = 0; i < pooledObjects.Count; i++)
            {
                if (pooledObjects[i].activeInHierarchy)
                {
                    pooledObjects[i].SetActive(false);
                }
            }
        }
    }
}