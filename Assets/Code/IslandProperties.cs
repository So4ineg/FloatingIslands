using System.Collections.Generic;
using UnityEngine;

public class IslandProperties : MonoBehaviour
{
    [Header("Залежи острова")]
    // Теперь в инспекторе будет удобный список, куда можно добавить хоть 10 видов руды
    public List<ResourceSlot> resources = new List<ResourceSlot>();

    [Header("Текущие эффекты")]
    // Словарь, где хранятся текущие уровни радиации, скверны и т.д.
    public Dictionary<EnvEffectType, float> currentEffects = new Dictionary<EnvEffectType, float>();

    [Header("Здания")]
    // Список всех зданий, расположенных на острове
    public List<Building> buildings = new List<Building>();

    [Header("Население острова")]
    public List<Person> populationOnIsland = new List<Person>();

    [Header("Настройки физики")]
    public float verticalSpeed = 0.02f;
    public float hoverAmplitude = 0.3f;
    public float hoverFrequency = 1f;
    public float scaleMultiplier = 0.001f;

    private float currentBaseY;
    private float randomOffset;

    // Текущая вертикальная скорость (положительная - летим вверх, отрицательная - падаем)
    public float CurrentSpeed { get; private set; }

    void Start()
    {
        currentBaseY = transform.position.y;
        randomOffset = Random.Range(0f, Mathf.PI * 2);
        UpdateIslandScale();
    }

    void Update()
    {
        CalculatePhysics();
        UpdateEnvironment();
    }

    // Считаем массу ВСЕХ залежей
    public float GetResourcesMass()
    {
        float mass = 0;
        foreach (var slot in resources)
        {
            mass += slot.amount * slot.weightPerUnit;
        }
        return mass;
    }

    // Считаем подъемную силу ВСЕХ залежей (кристаллов)
    public float GetResourcesLift()
    {
        float lift = 0;
        foreach (var slot in resources)
        {
            lift += slot.amount * slot.liftPerUnit;
        }
        return lift;
    }

    // Считаем массу ВСЕХ зданий и жителей на острове
    public float GetObjectsMass()
    {
        float objectsMass = 0;

        // Находим все компоненты с интерфейсом IWeightable среди дочерних объектов острова
        IWeightable[] objectsOnIsland = GetComponentsInChildren<IWeightable>();

        foreach (var obj in objectsOnIsland)
        {
            objectsMass += obj.GetWeight();
        }
        return objectsMass;
    }

    // Считаем подъемную силу ВСЕХ зданий/объектов на острове
    public float GetObjectsLift()
    {
        float objectsLift = 0;

        // Находим все компоненты с интерфейсом ILifting
        ILifting[] liftingObjects = GetComponentsInChildren<ILifting>();

        foreach (var obj in liftingObjects)
        {
            objectsLift += obj.GetLiftForce();
        }
        return objectsLift;
    }

    // Итоговая масса: Ресурсы + Здания/Юниты
    public float TotalMass => GetResourcesMass() + GetObjectsMass();

    // ИТОГОВАЯ ПОДЪЕМНАЯ СИЛА: Кристаллы внутри острова + Здания-подъемники
    public float TotalLift => GetResourcesLift() + GetObjectsLift();

    public void UpdateIslandScale()
    {
        // Размер острова логично привязывать только к природной массе (ресурсам), 
        // иначе от постройки домика остров будет раздуваться
        float naturalMass = GetResourcesMass();
        float newScale = 1f + (naturalMass * scaleMultiplier);
        newScale = Mathf.Max(0.2f, newScale);

        transform.localScale = new Vector3(newScale, newScale, 1f);
    }

    private void CalculatePhysics()
    {
        // Считаем нашу собственную силу (Тяга минус Вес)
        float baseNetForce = TotalLift - TotalMass;

        // Получаем влияние атмосферы на текущей высоте
        float atmosphereForce = 0f;
        if (AtmosphereManager.Instance != null)
        {
            atmosphereForce = AtmosphereManager.Instance.GetAtmosphereForce(currentBaseY);
        }

        // ИТОГОВАЯ СИЛА: наша собственная тяга + давление атмосферы
        float finalForce = baseNetForce + atmosphereForce;

        // СОХРАНЯЕМ СКОРОСТЬ (без Time.deltaTime, чтобы это были "метры в секунду")
        CurrentSpeed = finalForce * verticalSpeed;

        // Применяем движение
        currentBaseY += CurrentSpeed * Time.deltaTime;

        // Дыхание острова (синусоида)
        float hoverOffset = Mathf.Sin(Time.time * hoverFrequency + randomOffset) * hoverAmplitude;
        transform.position = new Vector3(transform.position.x, currentBaseY + hoverOffset, transform.position.z);
    }

    private void UpdateEnvironment()
    {
        // ПОЛУЧАЕМ ЭФФЕКТЫ СЛОЕВ от Менеджера
        if (AtmosphereManager.Instance != null)
        {
            currentEffects = AtmosphereManager.Instance.GetEffectsAtHeight(currentBaseY);
        }
    }

    // ПУБЛИЧНЫЙ МЕТОД для зданий/юнитов, чтобы узнать уровень угрозы или бонуса
    public float GetEffectIntensity(EnvEffectType type)
    {
        if (currentEffects.ContainsKey(type))
        {
            return currentEffects[type];
        }
        return 0f;
    }

    // Встроенный метод Unity, срабатывает при клике мышкой по коллайдеру объекта
    private void OnMouseDown()
    {
        if (IslandUIManager.Instance != null)
        {
            IslandUIManager.Instance.ShowIslandInfo(this);
        }
    }

    // Метод регистрации
    public void RegisterBuilding(Building building)
    {
        if (!buildings.Contains(building))
        {
            buildings.Add(building);
        }
    }

    // Метод отмены регистрации при сносе
    public void UnregisterBuilding(Building building)
    {
        if (buildings.Contains(building))
        {
            buildings.Remove(building);
        }
    }

    // Прибытие жителя на остров
    public void AddPersonToIsland(Person person)
    {
        if (!populationOnIsland.Contains(person))
        {
            populationOnIsland.Add(person);
            person.currentIsland = this;

            // Сразу пытаемся найти ему жилье и работу на этом острове
            DistributePopulation();
        }
    }

    // Убытие жителя с острова (пересел на корабль/улетел)
    public void RemovePersonFromIsland(Person person)
    {
        if (populationOnIsland.Contains(person))
        {
            if (person.homeBuilding != null) person.homeBuilding.EvictResident(person);
            if (person.workBuilding != null) person.workBuilding.DismissWorker(person);

            populationOnIsland.Remove(person);
            person.currentIsland = null;
        }
    }

    // Автоматический распределитель ( расселить бездомных, трудоустроить безработных)
    public void DistributePopulation()
    {
        // 1. Ищем все жилые дома и работы на острове
        List<ResidentialHouse> houses = new List<ResidentialHouse>();
        List<WorkplaceBuilding> workplaces = new List<WorkplaceBuilding>();

        foreach (var b in buildings)
        {
            if (b is ResidentialHouse house) houses.Add(house);
            if (b is WorkplaceBuilding work) workplaces.Add(work);
        }

        // 2. Расселяем бездомных
        foreach (var person in populationOnIsland)
        {
            if (person.IsHomeless)
            {
                foreach (var house in houses)
                {
                    if (house.HasFreeBeds)
                    {
                        house.SettleResident(person);
                        break;
                    }
                }
            }

            // 3. Устраиваем безработных
            if (person.IsUnemployed)
            {
                foreach (var work in workplaces)
                {
                    if (work.HasVacancies && work.requiredClass == person.socialClass)
                    {
                        work.HireWorker(person);
                        break;
                    }
                }
            }
        }
    }

    // Подсчет статистики для UI
    public int GetHomelessCount() => populationOnIsland.FindAll(p => p.IsHomeless).Count;
    public int GetUnemployedCount() => populationOnIsland.FindAll(p => p.IsUnemployed).Count;

    public float GetAverageMood()
    {
        if (populationOnIsland.Count == 0) return 100f;
        float total = 0;
        foreach (var p in populationOnIsland) total += p.mood;
        return total / populationOnIsland.Count;
    }
}