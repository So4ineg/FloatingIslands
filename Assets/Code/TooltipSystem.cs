using UnityEngine;
using TMPro;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance;

    [Header("UI Элементы тултипа")]
    public GameObject tooltipPanel;      // Фон тултипа
    public TextMeshProUGUI tooltipText;  // Текст тултипа

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Hide(); // Прячем при старте
    }

    void Update()
    {
        // Если тултип активен, заставляем его следовать за мышью
        if (tooltipPanel.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;
            RectTransform rect = tooltipPanel.GetComponent<RectTransform>();

            // УМНЫЙ PIVOT:
            // Если мышка в правой половине экрана (mouse.x / ширина > 0.5), то Pivot.x = 1 (рисуем влево)
            // Иначе Pivot.x = 0 (рисуем вправо)
            float pivotX = mousePos.x / Screen.width > 0.5f ? 1f : 0f;

            // То же самое для высоты: если мышка сверху - рисуем вниз (1), если снизу - вверх (0)
            float pivotY = mousePos.y / Screen.height > 0.5f ? 1f : 0f;

            // Применяем новые оси
            rect.pivot = new Vector2(pivotX, pivotY);

            // ДИНАМИЧЕСКИЙ ОТСТУП:
            // Чтобы мышка не перекрывала текст, нам нужен небольшой отступ. 
            // Но отступать нужно в ту сторону, куда смотрит тултип!
            float offsetX = pivotX == 1 ? -15f : 15f;
            float offsetY = pivotY == 1 ? -15f : 15f;

            // Двигаем панель на позицию мыши + правильный отступ
            tooltipPanel.transform.position = mousePos + new Vector2(offsetX, offsetY);
        }
    }

    public void Show(string message)
    {
        tooltipText.text = message;
        tooltipPanel.SetActive(true);
    }

    public void Hide()
    {
        tooltipPanel.SetActive(false);
    }
}
