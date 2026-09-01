using UnityEngine;

public abstract class SingletonComponent<T> : MonoBehaviour
    where T : Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            var instances = FindObjectsByType<T>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );
            if (instances.Length > 0)
            {
                for (int i = 1; i < instances.Length; i++)
                {
                    Destroy(instances[i].gameObject);
                }

                instance = instances[0];
                instance.gameObject.SetActive(true);
                return instance;
            }

            GameObject go = new GameObject { name = $"[{typeof(T).Name}]" };
            instance = go.AddComponent<T>();
            DontDestroyOnLoad(instance.gameObject);
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this as T;
        DontDestroyOnLoad(gameObject);
        gameObject.name = $"[{typeof(T).Name}]";
    }
}
