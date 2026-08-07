using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AccordionUI : MonoBehaviour
{
    [Header("Ссылки на внутренние элементы")]
    public Button headerButton;         // Кнопка заголовка
    public TextMeshProUGUI headerText;  // Текст на кнопке
    public GameObject contentContainer; // Контейнер для списка
    public Transform contentTransform;  // Ссылка на Transform контейнера (куда спавнить строки)

    void Awake()
    {
        // Автоматически вешаем прослушку клика через код!
        // Больше не нужно настраивать On Click() руками в Инспекторе.
        headerButton.onClick.AddListener(ToggleContent);
    }

    // Внешний метод для быстрой настройки заголовка
    public void SetTitle(string title)
    {
        headerText.text = title;
    }

    // Внешний метод, чтобы передать менеджеру место для спавна строчек
    public Transform GetContentArea()
    {
        return contentTransform;
    }

    private void ToggleContent()
    {
        // Переключаем активность контейнера
        bool isActive = contentContainer.activeSelf;
        contentContainer.SetActive(!isActive);

        // Маленький хак Юнити: иногда Layout Group забывает пересчитать размер при включении.
        // Эта строчка принудительно обновляет UI.
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}
