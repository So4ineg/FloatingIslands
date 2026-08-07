using System;
using System.Collections.Generic;
using UnityEngine;

public enum SocialClass
{
    Worker,     // Рабочий
    Engineer,   // Инженер
    Aristocrat  // Аристократ
}

public enum PersonTag
{
    Sick,       // Болен
    Happy,      // Счастлив
    Mad,        // Безумен / Бунтарь
    Starving,   // Голодает
    Inspired    // Вдохновлен (повышенный КПД)
}

[Serializable]
public class Person
{
    [Header("Паспортные данные")]
    public string id;
    public string fullName;
    public string gender; // "Мужской", "Женский"
    public int age;
    public SocialClass socialClass;

    [Header("Локация и Связи")]
    // Текущий остров или корабль (можно расширить интерфейсом ILocation)
    public IslandProperties currentIsland;
    public ResidentialHouse homeBuilding;  // Ссылка на дом (null = бездомный)
    public WorkplaceBuilding workBuilding; // Ссылка на работу (null = безработный)

    [Header("Состояние")]
    [Range(0, 100)]
    public float mood = 70f; // Настроение (0 - бунт/безумие, 100 - эйфория)
    public List<PersonTag> activeTags = new List<PersonTag>();

    // Быстрые проверки
    public bool IsHomeless => homeBuilding == null;
    public bool IsUnemployed => workBuilding == null;

    public Person(string name, string gender, int age, SocialClass socialClass)
    {
        this.id = Guid.NewGuid().ToString();
        this.fullName = name;
        this.gender = gender;
        this.age = age;
        this.socialClass = socialClass;
        this.mood = 70f;
    }

    public void AddTag(PersonTag tag)
    {
        if (!activeTags.Contains(tag)) activeTags.Add(tag);
    }

    public void RemoveTag(PersonTag tag)
    {
        if (activeTags.Contains(tag)) activeTags.Remove(tag);
    }
}