using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Vector3 lastPlayerPosition;
    public string playerName;
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;               
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }
}
