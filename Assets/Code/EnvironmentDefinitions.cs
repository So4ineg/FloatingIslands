using UnityEngine;

// Список всех возможных эффектов (легко пополнять)
public enum EnvEffectType
{
    SolarRadiation, // Солнечная радиация (растет вверх)
    SolarPower,      // Солнечный свет для панелей (растет вверх)
    Corruption,     // Лавкрафтианская скверна (растет вниз)
    AirTemperature  // Температура (например, холодно и наверху, и в самом низу)
}

// Структура для настройки того, как эффект ведет себя на разных высотах
[System.Serializable]
public struct LayerEffectConfig
{
    public EnvEffectType effectType;

    [Tooltip("Значение эффекта на экваторе (Y = 0)")]
    public float baseValue;

    [Tooltip("Как меняется эффект с каждым метром высоты. \nПоложительное - растет вверх.\nОтрицательное - растет вниз (чем ниже, тем сильнее).")]
    public float gradientPerY;

    [Tooltip("Минимальное значение (чтобы радиация не уходила в минус)")]
    public float minValue;

    [Tooltip("Максимальное значение (кап эффекта)")]
    public float maxValue;
}

// Структура, которую будет получать остров (готовый результат)
public struct CurrentEnvEffect
{
    public EnvEffectType effectType;
    public float intensity;
}
