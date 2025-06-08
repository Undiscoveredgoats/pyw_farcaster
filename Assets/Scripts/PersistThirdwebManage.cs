using UnityEngine;
public class PersistThirdwebManager : MonoBehaviour
{
    private static PersistThirdwebManager _instance;
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        transform.SetParent(null); // Ensure root GameObject
        DontDestroyOnLoad(gameObject);
    }
}