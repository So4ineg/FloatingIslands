using System.Collections.Generic;
using UnityEngine;

// Класс абстрактный, его нельзя повесить напрямую, но от него наследуются все дома
public abstract class Building : MonoBehaviour, IWeightable, ILifting
{
    [Header("Базовые настройки здания")]
    public string buildingName = "Безымянное здание";
    public float baseWeight = 10f;    // Собственный вес конструкции
    public float baseLiftForce = 0f;  // Собственная подъемная сила (если есть)

    // Ссылка на остров, на котором стоит здание
    public IslandProperties HostIsland { get; private set; }

    protected virtual void Awake()
    {
        // При появлении ищем остров среди родительских объектов
        HostIsland = GetComponentInParent<IslandProperties>();
    }

    protected virtual void Start()
    {
        // Регистрируем здание на острове
        if (HostIsland != null)
        {
            HostIsland.RegisterBuilding(this);
        }
    }

    protected virtual void OnDestroy()
    {
        // При сносе/уничтожении здания снимаем его с учета
        if (HostIsland != null)
        {
            HostIsland.UnregisterBuilding(this);
        }
    }

    // Реализация интерфейса IWeightable
    public virtual float GetWeight() => baseWeight;

    // Реализация интерфейса ILifting
    public virtual float GetLiftForce() => baseLiftForce;

    // Абстрактный метод: каждое здание должно само рассказать, что писать в тултип
    public abstract string GetStatusInfo();
}