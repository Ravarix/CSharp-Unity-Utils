using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unitilities
{
    public static class GameObjectPool
    {
        private const int DefaultSize = 10;

        //prefab to queue of inactives
        private static readonly Dictionary<GameObject, Queue<GameObject>> Pool = new Dictionary<GameObject, Queue<GameObject>>();
        //instance to prefab
        private static readonly Dictionary<GameObject, GameObject> Active = new Dictionary<GameObject, GameObject>();
        
        public static Queue<GameObject> Warm(GameObject prefab, int count)
        {
            Queue<GameObject> queue;
            if (!Pool.TryGetValue(prefab, out queue))
                Pool.Add(prefab, queue = new Queue<GameObject>(count));
            var stored = queue.Count;
            for (int i = 0; i < count - stored; i++)
            {
                var go = Object.Instantiate(prefab);
                go.SetActive(false);
                queue.Enqueue(go);
            }
            return queue;
        }

        public static GameObject Get(GameObject prefab, Transform parent = null)
        {
            if (!prefab) return null; //idiot check

            Queue<GameObject> queue;
            if (!Pool.TryGetValue(prefab, out queue))
                queue = Warm(prefab, DefaultSize);
            GameObject go;
            if (queue.Count == 0 || !(go = queue.Dequeue())) //check if go hasn't been Destroyed
                go = Object.Instantiate(prefab);
            go.transform.SetParent(parent);
            go.SetActive(true);
            Active.Add(go, prefab);
            return go;
        }

        public static void Put(GameObject go)
        {
            if (!go) return; //idiot check

            GameObject prefab;
            if (Active.TryGetValue(go, out prefab))
            {
                go.SetActive(false);
                go.transform.SetParent(null);
                go.transform.localPosition = Vector3.zero;
                Active.Remove(go);
                foreach (var mono in go.GetComponents<MonoBehaviour>())
                    (mono as IDisposable)?.Dispose(); //try dispose
                Pool[prefab].Enqueue(go);
            }
            else
                Object.Destroy(go);
        }
        //replacement for Destroy
        public static void Put(GameObject go, float delay) => MEC.Timing.CallDelayed(delay, () => Put(go));

        public static void Clear()
        {
            foreach (var prefab in Pool.Keys)
                Clear(prefab);
            Pool.Clear();
        }

        public static void Clear(GameObject prefab)
        {
            if (!prefab) return; //idiot check
            
            Queue<GameObject> queue;
            if (!Pool.TryGetValue(prefab, out queue))
                return;
            foreach (var go in queue)
                Object.Destroy(go);
            queue.Clear();
        }
    }
}