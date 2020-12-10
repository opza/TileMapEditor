using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Util
{
    public static class ResourceLoader
    {
        public static Dictionary<string, T> LoadResources<T>(string path) where T : UnityEngine.Object
        {
            T[] loadedResources = Resources.LoadAll<T>(path);
            return loadedResources.ToDictionary(value => value.name);
        }

        public static Dictionary<string, BlockInfo> LoadBlockInfos(string path)
        {
            var loadedBlockInfos = Resources.LoadAll<BlockInfo>(path);
            return loadedBlockInfos.ToDictionary(value => value.Name);
        }
    }

    public static class MousePoint
    {
        public static Vector2 GetWorldPoint()
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public static class ObjectPool
    {
        static Dictionary<string, PoolItem> poolItems = new Dictionary<string, PoolItem>();

        public static bool Contains(string name)
        {
            return poolItems.ContainsKey(name);
        }

        public static void AddBase(string name, GameObject baseObject, Transform parent = null)
        {
            if (poolItems.ContainsKey(name))
                return;

            poolItems[name] = new PoolItem(name, baseObject, parent);
        }

        public static void RemoveBase(string name)
        {
            if (!poolItems.ContainsKey(name))
                return;

            var item = poolItems[name];

            GameObject.Destroy(item.BaseObject.GetComponent<PoolMember>());
            item.Clear();

            poolItems.Remove(name);
        }

        public static GameObject Spawn(string name, Vector3 spawnPosition)
        {
            if (!poolItems.ContainsKey(name))
                return null;

            return poolItems[name].Spawn(spawnPosition);
        }

        public static void Destroy(GameObject poolMemberObject)
        {
            string name = poolMemberObject.GetComponent<PoolMember>()?.Name;
            if (name == null)
                return;

            poolItems[name].Destroy(poolMemberObject);
        }

        class PoolItem
        {
            string name;
            public string Name => name;

            GameObject baseObject;
            public GameObject BaseObject => baseObject;

            Transform parent;

            Queue<GameObject> inActiveObjects = new Queue<GameObject>();
            List<GameObject> allObjects = new List<GameObject>();

            public PoolItem(string name, GameObject baseObject, Transform parent = null)
            {
                this.name = name;
                this.baseObject = baseObject;
                this.parent = parent;

                baseObject.SetActive(false);
            }

            public GameObject Spawn(Vector3 spawnPosition)
            {
                GameObject obj;
                if (inActiveObjects.Count <= 0)
                {
                    obj = GameObject.Instantiate(baseObject);
                    obj.transform.parent = parent;
                    obj.AddComponent<PoolMember>().Name = name;

                    allObjects.Add(obj);
                }
                else
                    obj = inActiveObjects.Dequeue();

                obj.SetActive(true);
                obj.transform.position = spawnPosition;

                return obj;
            }

            public void Destroy(GameObject poolItemObject)
            {
                poolItemObject.SetActive(false);
                inActiveObjects.Enqueue(poolItemObject);
            }

            public void Clear()
            {
                foreach (var obj in allObjects)
                {
                    Destroy(obj);
                }

                allObjects.Clear();
            }

        }

        public class PoolMember : MonoBehaviour
        {
            public string Name { get; set; }
        }


    }

    
}