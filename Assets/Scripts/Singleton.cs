using UnityEngine;
using System.Collections.Generic;

namespace Glauz
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {

        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameObject().AddComponent<T>();

                return instance;
            }
        }

        public void Awake()
        {
            if (instance != null)
            {
                Debug.Log("Other Instance of " + this.GetType().Name + " has been destroyed on GameObject " + name + "!");
            }

            instance = this as T;
        }

    } 
}
