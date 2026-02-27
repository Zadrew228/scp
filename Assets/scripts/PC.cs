using UnityEngine;
using UnityEngine.SceneManagement;

public class PC : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "Desktop";
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    private bool playerInRange = false;
    void Start()
    {
        
    }

    
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            
            SavePlayerPosition();
            SceneManager.LoadScene("Desktop");
        }
    }

    void SavePlayerPosition()
    {
        // Простой вариант через статику
        if (GameManager.Instance != null) // Если используем Singleton
        {
            GameManager.Instance.lastPlayerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("косание");   
        }
        
        if (other.gameObject.tag == "Player")
        {
            playerInRange = true;
            ShowInteractionText("Нажмите E, чтобы сесть за компьютер");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInRange = false;
            HideInteractionText();
        }
    }

    void ShowInteractionText(string message)
    {
        
        Debug.Log(message); 
    }

    void HideInteractionText()
    {
        Debug.Log("Текст скрыт");
    }
}
