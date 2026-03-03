using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class SCPEntry
{
    public string scpNumber;      // например "173"
    public Sprite scpIcon;        // иконка SCP
    public Color buttonColor = Color.white; // цвет фона
}

public class scpList : MonoBehaviour
{
    [Header("Настройки списка")]
    public GameObject scpButtonPrefab;    // префаб элемента SCP
    public Transform contentParent;      // куда добавлять (объект Content)

    [Header("Список SCP (добавляй в инспекторе)")]
    public List<SCPEntry> scp_List = new List<SCPEntry>();

    void Start()
    {
        GenerateSCPList();
    }


    public void GenerateSCPList()
    {
        // Очищаем старые элементы
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Создаем новые
        foreach (SCPEntry entry in scp_List)
        {
            CreateSCPButton(entry);
        }
    }

    void CreateSCPButton(SCPEntry entry)
    {
        // Создаем кнопку из префаба
        GameObject newButtonObj = Instantiate(scpButtonPrefab, contentParent);
        Button button = newButtonObj.GetComponent<Button>();

        // Настраиваем цвет кнопки
        ColorBlock colors = button.colors;
        colors.normalColor = entry.buttonColor;
        colors.highlightedColor = entry.buttonColor * 1.2f; // чуть светлее при наведении
        button.colors = colors;

        Transform iconTransform = newButtonObj.transform.Find("Icon");
        Transform nameTransform = newButtonObj.transform.Find("NameText");

        // СПОСОБ 2: Если объекты вложены глубже
        if (iconTransform == null)
            iconTransform = newButtonObj.transform.Find("Icon/Image"); // пример пути

        // СПОСОБ 3: Поиск по компонентам (если не нашли по имени)
        Image iconImage = null;
        TextMeshProUGUI nameText = null;

        if (iconTransform != null)
            iconImage = iconTransform.GetComponent<Image>();
        else
            iconImage = newButtonObj.GetComponentInChildren<Image>(); // ищет во всех дочерних

        if (nameTransform != null)
            nameText = nameTransform.GetComponent<TextMeshProUGUI>();
        else
            nameText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();

        // Заполняем данные
        if (iconImage != null && entry.scpIcon != null)
        {
            iconImage.sprite = entry.scpIcon;
            Debug.Log($"Иконка установлена для SCP-{entry.scpNumber}");
        }
        else
        {
            Debug.LogWarning($"Не удалось найти иконку для SCP-{entry.scpNumber}");
        }

        if (nameText != null)
        {
            nameText.text = "SCP-" + entry.scpNumber;
            Debug.Log($"Текст установлен для SCP-{entry.scpNumber}");
        }
        else
        {
            Debug.LogWarning($"Не удалось найти текст для SCP-{entry.scpNumber}");
        }

        // Добавляем обработчик нажатия
        button.onClick.AddListener(() => OnSCPButtonClicked(entry));
    
}

    // Что происходит при нажатии на SCP
    void OnSCPButtonClicked(SCPEntry entry)
    {
        Debug.Log($"Нажали на SCP-{entry.scpNumber}");

        // Здесь можно вызвать открытие подробной информации
        // Например: ShowSCPDetails(entry);

        // Или отправить событие
        // FindObjectOfType<SomeManager>().OpenSCPFile(entry.scpNumber);
    }

    // Метод для добавления нового SCP через код
    public void AddSCP(string number, Sprite icon)
    {
        SCPEntry newEntry = new SCPEntry
        {
            scpNumber = number,
            scpIcon = icon
        };

        scp_List.Add(newEntry);
        CreateSCPButton(newEntry);
    }

    // Очистить список
    public void ClearList()
    {
        scp_List.Clear();
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }

    // Назначить действие для всех кнопок (если нужно поменять логику)
    public void SetAllButtonsAction(System.Action<SCPEntry> action)
    {
        foreach (Transform child in contentParent)
        {
            Button btn = child.GetComponent<Button>();
            SCPEntry entry = GetEntryFromButton(child.gameObject);

            if (btn != null && entry != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => action(entry));
            }
        }
    }

    SCPEntry GetEntryFromButton(GameObject buttonObj)
    {
        // Простой способ - ищем по тексту
        TextMeshProUGUI nameText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (nameText != null)
        {
            string number = nameText.text.Replace("SCP-", "");
            return scp_List.Find(x => x.scpNumber == number);
        }
        return null;
    }
         void Update()
        {

        }
}

