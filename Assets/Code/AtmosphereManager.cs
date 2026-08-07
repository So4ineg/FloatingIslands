using System.Collections.Generic;
using UnityEngine;

public class AtmosphereManager : MonoBehaviour
{
    // Синглтон для быстрого доступа из любого скрипта
    public static AtmosphereManager Instance;

    [Header("Настройки атмосферы")]
    [Tooltip("Условная середина мира (экватор высоты), где сопротивление равно 0")]
    public float middleY = 0f;
    public float densityGradient = 10f;

    [Header("Настройки слоев и эффектов")]
    // Список всех эффектов в мире, который мы будем настраивать в Инспекторе
    public List<LayerEffectConfig> layerEffects = new List<LayerEffectConfig>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Этот метод рассчитывает силу сопротивления в зависимости от высоты
    public float GetAtmosphereForce(float currentY)
    {
        // Насколько остров отклонился от идеальной середины
        float distanceFromMiddle = currentY - middleY;

        // Сила направлена в ПРОТИВОПОЛОЖНУЮ сторону от отклонения.
        // Если остров высоко (distance > 0), сила будет отрицательной (тянет вниз).
        // Если остров низко (distance < 0), сила будет положительной (выталкивает вверх).
        return -distanceFromMiddle * densityGradient;
    }

    // Парсинг эффектов для конкретной высоты
    public Dictionary<EnvEffectType, float> GetEffectsAtHeight(float currentY)
    {
        var effectsAtHeight = new Dictionary<EnvEffectType, float>();

        foreach (var config in layerEffects)
        {
            // Формула: Базовое значение + (Высота * Градиент)
            float calculatedIntensity = config.baseValue + (currentY * config.gradientPerY);

            // Ограничиваем рамками
            calculatedIntensity = Mathf.Clamp(calculatedIntensity, config.minValue, config.maxValue);

            // Записываем в словарь
            effectsAtHeight[config.effectType] = calculatedIntensity;
        }

        return effectsAtHeight;
    }
}