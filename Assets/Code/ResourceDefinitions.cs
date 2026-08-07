using UnityEngine;

// 1. Типы ресурсов, которые можно легко пополнять
public enum ResourceType
{
    Rock,
    AntiGravCrystal,
    Coal,
    Iron,
    Gold
}

[System.Serializable]
public struct ResourceCost
{
    public ResourceType resourceType;
    public float amount;
}


// 2. Структура для отображения ресурса в Инспекторе
[System.Serializable]
public class ResourceSlot
{
    public ResourceType resourceType;
    public float amount;

    [Tooltip("Сколько весит 1 единица этого ресурса")]
    public float weightPerUnit = 1f;

    [Tooltip("Какую подъемную силу дает 1 единица (обычно 0, кроме кристаллов)")]
    public float liftPerUnit = 0f;
}

// Интерфейс для ВСЕГО, что имеет вес (Дома, Юниты, Деревья)
public interface IWeightable
{
    float GetWeight();
}

// Интерфейс для ВСЕГО, что тянет остров ВВЕРХ (Гравитационные башни, шары, магические якоря)
public interface ILifting
{
    float GetLiftForce();
}