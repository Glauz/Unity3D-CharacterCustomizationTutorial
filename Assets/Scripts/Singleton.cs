using UnityEngine;


public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T instance;


    public void Awake()
    {
        if (instance == null)
        {
            instance = GetComponent<T>();
        }
        else
            Destroy(this.gameObject);
    }


    public static T Instance
    {
        get
        {
            if (instance == null)
                print("Instance of GameObject does not exist!");

            return instance;
        }
    }
}
