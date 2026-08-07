using System.Collections.Generic;
using UnityEngine;

public class WarehouseBuilding : Building
{
    [Header("Параметры Склада")]
    public float maxCapacity = 500f; // Максимальная вместимость (в единицах)

    // Текущие ресурсы на складе
    public Dictionary<ResourceType, float> storedResources = new Dictionary<ResourceType, float>();

    public float CurrentTotalAmount
    {
        get
        {
            float total = 0f;
            foreach (var kvp in storedResources) total += kvp.Value;
            return total;
        }
    }

    public float AvailableCapacity => Mathf.Max(0f, maxCapacity - CurrentTotalAmount);
    public bool IsFull => AvailableCapacity <= 0f;

    // Добавление ресурса на склад. Возвращает излишек, который не поместился
    public float AddResource(ResourceType type, float amount)
    {
        float spaceLeft = AvailableCapacity;
        if (spaceLeft <= 0f) return amount; // Склад полн

        float amountToAdd = Mathf.Min(spaceLeft, amount);
        float overflow = amount - amountToAdd;

        if (!storedResources.ContainsKey(type))
            storedResources[type] = 0f;

        storedResources[type] += amountToAdd;
        return overflow;
    }

    // Списание ресурса со склада
    public bool ConsumeResource(ResourceType type, float amount)
    {
        if (!storedResources.ContainsKey(type) || storedResources[type] < amount)
            return false;

        storedResources[type] -= amount;
        return true;
    }

    // Динамический вес: базовый конструктив + масса всех лежащих на нем материалов
    public override float GetWeight()
    {
        float resourcesWeight = 0f;
        foreach (var kvp in storedResources)
        {
            // Умножаем количество на плотность ресурса из ResourceDefinitions
            resourcesWeight += kvp.Value * ResourceDefinitions.GetDensity(kvp.Key);
        }
        return baseWeight + resourcesWeight;
    }

    public override string GetStatusInfo()
    {
        string info = $"<b>{buildingName}</b>\nЗанято: {CurrentTotalAmount:F0}/{maxCapacity:F0}\n";
        foreach (var kvp in storedResources)
        {
            if (kvp.Value > 0)
                info += $"{kvp.Key}: {kvp.Value:F1}\n";
        }
        return info;
    }
}