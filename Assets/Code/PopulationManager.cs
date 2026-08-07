using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance { get; private set; }

    [Header("Глобальная база данных населения")]
    public List<Person> globalPopulation = new List<Person>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Метод создания нового человека и спавна его на остров
    public Person CreateAndSpawnPerson(string name, string gender, int age, SocialClass socialClass, IslandProperties targetIsland)
    {
        Person newPerson = new Person(name, gender, age, socialClass);

        // Регистрируем в глобальной базе
        globalPopulation.Add(newPerson);

        // Отправляем на остров
        if (targetIsland != null)
        {
            targetIsland.AddPersonToIsland(newPerson);
        }

        return newPerson;
    }

    // Удаление человека из мира (гибель/эмиграция)
    public void RemovePerson(Person person)
    {
        if (person.homeBuilding != null) person.homeBuilding.EvictResident(person);
        if (person.workBuilding != null) person.workBuilding.DismissWorker(person);
        if (person.currentIsland != null) person.currentIsland.RemovePersonFromIsland(person);

        globalPopulation.Remove(person);
    }
}