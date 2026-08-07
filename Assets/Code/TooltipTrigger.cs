using UnityEngine;
using UnityEngine.EventSystems; // Для отслеживания мыши над UI

// Интерфейсы IPointerEnterHandler и IPointerExitHandler заставляют Юнити 
// сообщать скрипту, когда мышка вошла в зону объекта и вышла из нее
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Multiline] // Позволяет писать текст в инспекторе в несколько строк
    public string tooltipMessage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipSystem.Instance.Show(tooltipMessage);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Instance.Hide();
    }
}