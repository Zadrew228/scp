using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class RandomEvent
{
    public string eventName;
    [TextArea(3, 5)]
    public string eventDescription;
    public Sprite eventIcon;

    [Range(0, 100)]
    public float probabilityWeight = 50f;

    [Range(0, 1f)]
    public float minTimeOfDay = 0f;
    [Range(0, 1f)]
    public float maxTimeOfDay = 1f;

    public int minDay = 1;
    public int maxDay = int.MaxValue;
    public bool canRepeat = false;

    // Длительность события в секундах (0 = мгновенное)
    public float eventDuration = 0f;

    // События Unity
    public UnityEvent onEventTriggered;
    public UnityEvent onEventEnded;
}

public class Timer : MonoBehaviour
    {
    [Header("Настройки времени")]
    public float dayLengthInSeconds = 60f; // длина дня в секундах
    public int currentDay = 1;

    [Header("События")]
    public List<RandomEvent> allEvents = new List<RandomEvent>();

    [Header("Настройки событий")]
    public float minTimeBetweenEvents = 5f;
    public float maxTimeBetweenEvents = 15f;

    private float timeOfDay = 0f;
    private List<RandomEvent> todayEvents = new List<RandomEvent>();
    private bool isDayEnding = false;

    void Start()
    {
        StartNewDay();
        StartCoroutine(DayCycle());
        StartCoroutine(EventScheduler());
    }

    // Главная корутина дня
    IEnumerator DayCycle()
    {
        while (true)
        {
            // Считаем время дня
            float timer = 0f;
            while (timer < dayLengthInSeconds)
            {
                if (!isDayEnding)
                {
                    timer += Time.deltaTime;
                    timeOfDay = timer / dayLengthInSeconds;
                }
                yield return null;
            }

            // День закончился
            yield return StartCoroutine(EndDay());
        }
    }

    // Корутина планировщика событий
    IEnumerator EventScheduler()
    {
        while (true)
        {
            // Ждем случайное время между событиями
            float waitTime = Random.Range(minTimeBetweenEvents, maxTimeBetweenEvents);
            yield return new WaitForSeconds(waitTime);

            // Пытаемся запустить событие
            if (todayEvents.Count > 0 && !isDayEnding)
            {
                TryTriggerEvent();
            }
        }
    }

    // Запуск нового дня
    void StartNewDay()
    {
        todayEvents.Clear();
        timeOfDay = 0f;
        isDayEnding = false;

        Debug.Log($"=== ДЕНЬ {currentDay} НАЧАЛСЯ ===");

        // Собираем доступные события для сегодняшнего дня
        foreach (var ev in allEvents)
        {
            if (ev.minDay <= currentDay && ev.maxDay >= currentDay)
            {
                // Проверяем вероятность
                if (Random.Range(0f, 100f) <= ev.probabilityWeight)
                {
                    todayEvents.Add(ev);
                    Debug.Log($"Событие доступно сегодня: {ev.eventName}");
                }
            }
        }

        Debug.Log($"Всего доступно событий: {todayEvents.Count}");
    }

    // Попытка запустить случайное событие
    void TryTriggerEvent()
    {
        // Фильтруем события по времени дня
        var possibleEvents = todayEvents.Where(e =>
            e.minTimeOfDay <= timeOfDay &&
            e.maxTimeOfDay >= timeOfDay
        ).ToList();

        if (possibleEvents.Count == 0) return;

        // Выбираем случайное событие
        int randomIndex = Random.Range(0, possibleEvents.Count);
        RandomEvent selectedEvent = possibleEvents[randomIndex];

        // Запускаем событие как корутину
        StartCoroutine(RunEvent(selectedEvent));

        // Если событие не повторяется - убираем из списка
        if (!selectedEvent.canRepeat)
        {
            todayEvents.Remove(selectedEvent);
        }
    }

    // Корутина выполнения события
    IEnumerator RunEvent(RandomEvent selectedEvent)
    {
        Debug.Log($"⚡ СОБЫТИЕ: {selectedEvent.eventName}");
        Debug.Log($"📝 {selectedEvent.eventDescription}");

        // Запускаем событие
        selectedEvent.onEventTriggered?.Invoke();

        // Если у события есть длительность - ждем
        if (selectedEvent.eventDuration > 0)
        {
            // Можно добавить визуальный индикатор времени события
            float eventTimer = 0f;
            while (eventTimer < selectedEvent.eventDuration)
            {
                eventTimer += Time.deltaTime;

                // Здесь можно обновлять UI события
                // eventUI.UpdateProgress(eventTimer / selectedEvent.eventDuration);

                yield return null;
            }
        }
        else
        {
            // Если длительности нет - просто ждем 1 кадр для визуала
            yield return null;
        }

        // Завершаем событие
        selectedEvent.onEventEnded?.Invoke();
        Debug.Log($"✅ Событие завершено: {selectedEvent.eventName}");
    }

    // Корутина конца дня
    IEnumerator EndDay()
    {
        isDayEnding = true;

        Debug.Log($"=== ДЕНЬ {currentDay} ЗАКАНЧИВАЕТСЯ ===");

        // Даем время на завершающие события
        yield return new WaitForSeconds(2f);

        currentDay++;
        StartNewDay();
    }

    // Вспомогательные методы для UI
    public float GetDayProgress()
    {
        return timeOfDay;
    }

    public string GetTimeString()
    {
        int hours = Mathf.FloorToInt(timeOfDay * 24);
        int minutes = Mathf.FloorToInt((timeOfDay * 24 - hours) * 60);
        return $"{hours:00}:{minutes:00}";
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }

    // Для отладки - принудительное событие
    public void ForceEvent(string eventName = null)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            TryTriggerEvent();
        }
        else
        {
            var ev = todayEvents.Find(e => e.eventName == eventName);
            if (ev != null)
            {
                StartCoroutine(RunEvent(ev));
            }
        }
    }

    // Для отладки - пропустить день
    public void SkipDay()
    {
        StopAllCoroutines();
        StartCoroutine(EndDay());
        StartCoroutine(DayCycle());
        StartCoroutine(EventScheduler());
    }
}
