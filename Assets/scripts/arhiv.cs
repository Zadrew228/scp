using UnityEngine;

public class arhiv : MonoBehaviour
{
    public GameObject Arhive;
    public GameObject Arhive1;
    private int click = 0;  // private для безопасности

    // Эту функцию вешаем на кнопку
    public void ToggleArchive()
    {
        click++; // увеличиваем счетчик

        if (click % 2 == 1) // нечетный клик (1, 3, 5...) - включаем
        {
            Arhive.SetActive(true);
            Arhive1.SetActive(true);
            Debug.Log("Архив включен, клик #" + click);
        }
        else // четный клик (2, 4, 6...) - выключаем
        {
            Arhive1.SetActive(false);
            Arhive.SetActive(false);
            Debug.Log("Архив выключен, клик #" + click);
        }
    }
}