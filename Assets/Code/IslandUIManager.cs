using UnityEngine;
using TMPro; // Используем TextMeshPro для красивого текста
using System.Text;
using UnityEngine.EventSystems;

public class IslandUIManager : MonoBehaviour
{
    public static IslandUIManager Instance;

    [Header("Базовый UI")]
    public GameObject islandInfoPanel;   // Сама панелька (окно)
    public TextMeshProUGUI basicStatsText; // Текстовое поле внутри панели

    [Header("Списки (Аккордеоны)")]
    public AccordionUI resourcesAccordion;
    public AccordionUI effectsAccordion;
    public AccordionUI buildingsAccordion;

    [Header("Префаб строчки")]
    public GameObject listItemPrefab; // Префаб текста с прикрепленным TooltipTrigger

    private IslandProperties targetIsland; // Остров, на который мы кликнули

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        resourcesAccordion.SetTitle("СОСТАВ НЕДР");
        effectsAccordion.SetTitle("ЭФФЕКТЫ СРЕДЫ");
        buildingsAccordion.SetTitle("ЗДАНИЯ");

        // Скрываем панель при старте игры
        islandInfoPanel.SetActive(false);
    }

    void Update()
    {
        // Работает только если панель открыта
        if (targetIsland != null && islandInfoPanel.activeSelf)
        {
            UpdateBasicStats(); // Обновляем цифры высоты/скорости

            // Проверяем клик левой кнопкой мыши (0)
            if (Input.GetMouseButtonDown(0))
            {
                // Если мы кликнули прямо по самой UI-панели (чтобы текст не закрывался, когда мы по нему кликаем)
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                // Переводим координаты мыши из пикселей экрана в координаты игрового мира
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Пускаем 2D-луч в точку клика
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                // Логика закрытия:
                // Если луч никуда не попал (клик в небо) ИЛИ попал во что-то, что НЕ является островом
                if (hit.collider == null || hit.collider.GetComponent<IslandProperties>() == null)
                {
                    HidePanel();
                }
            }
        }
    }

    // Этот метод будем вызывать при клике на остров
    public void ShowIslandInfo(IslandProperties island)
    {
        targetIsland = island;
        islandInfoPanel.SetActive(true);

        UpdateBasicStats();
        BuildResourceList(); // Списки строим один раз при клике, они не меняются каждый кадр
        BuildEffectsList();
        BuildBuildingsList();
    }

    // Метод для кнопки "Закрыть" (если кликнуть мимо острова)
    public void HidePanel()
    {
        targetIsland = null;
        islandInfoPanel.SetActive(false);
        TooltipSystem.Instance.Hide(); // Прячем тултип на всякий случай
    }

    private void UpdateBasicStats()
    {
        basicStatsText.text =
            $"<b>Высота:</b> {targetIsland.transform.position.y:F1} м.\n" +
            $"<b>Скорость:</b> {targetIsland.CurrentSpeed:F2} м/с\n" +
            $"<b>Масса:</b> {targetIsland.TotalMass}\n" +
            $"<b>Тяга:</b> {targetIsland.TotalLift}";
    }

    private void BuildResourceList()
    {
        // 1. Очищаем старый список (удаляем всех детей в контейнере)
        foreach (Transform child in resourcesAccordion.GetContentArea())
            Destroy(child.gameObject);

        // 2. Спавним новые строчки
        if (targetIsland.resources.Count == 0) return;

        foreach (var res in targetIsland.resources)
        {
            GameObject newRow = Instantiate(listItemPrefab, resourcesAccordion.GetContentArea());

            // Настраиваем текст
            newRow.GetComponent<TextMeshProUGUI>().text = $"- {res.resourceType}: {res.amount}";

            // Настраиваем тултип
            TooltipTrigger tooltip = newRow.GetComponent<TooltipTrigger>();
            tooltip.tooltipMessage = $"Вес единицы: {res.weightPerUnit}\nПодъемная сила: {res.liftPerUnit}\nОбщая масса: {res.amount * res.weightPerUnit}";
        }
    }

    private void BuildEffectsList()
    {
        foreach (Transform child in effectsAccordion.GetContentArea()) 
            Destroy(child.gameObject);

        foreach (var effect in targetIsland.currentEffects)
        {
            GameObject newRow = Instantiate(listItemPrefab, effectsAccordion.GetContentArea());
            newRow.GetComponent<TextMeshProUGUI>().text = $"- {effect.Key}: {effect.Value:F1}";

            TooltipTrigger tooltip = newRow.GetComponent<TooltipTrigger>();
            tooltip.tooltipMessage = $"Текущее влияние атмосферы на этот остров.";
        }
    }

    private void BuildBuildingsList()
    {
        Transform contentArea = buildingsAccordion.GetContentArea();
        foreach (Transform child in contentArea) Destroy(child.gameObject);

        if (targetIsland.buildings.Count == 0) return;

        foreach (var building in targetIsland.buildings)
        {
            GameObject newRow = Instantiate(listItemPrefab, contentArea);

            // Пишем название здания
            newRow.GetComponent<TextMeshProUGUI>().text = $"- {building.buildingName}";

            // Запрашиваем информацию для тултипа напрямую у здания
            TooltipTrigger tooltip = newRow.GetComponent<TooltipTrigger>();
            tooltip.tooltipMessage = building.GetStatusInfo();
        }
    }
}
