using UnityEngine;

public class CollapsibleSection : MonoBehaviour
{
    [Tooltip("Панель со списком, которую будем скрывать/показывать")]
    public GameObject contentPanel;

    // Этот метод будем вызывать при клике на кнопку заголовка
    public void ToggleContent()
    {
        if (contentPanel != null)
        {
            // Меняем состояние на противоположное
            contentPanel.SetActive(!contentPanel.activeSelf);
        }
    }
}
